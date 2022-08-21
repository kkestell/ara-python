import os

from llvmlite import ir

from syntax import Return, IntLiteral, Assignment, BinaryOp, _Atom as Atom, If, \
    BoolLiteral, VariableReference


def _make_type(t):
    if t.value == "int":
        return ir.IntType(32)
    elif t.value == "void":
        return ir.VoidType()
    elif t.value == "bool":
        return ir.IntType(1)
    else:
        raise Exception("Unknown type")


def _make_parameter_types(node):
    if node.parameters is not None:
        return [_make_type(p.type) for p in node.parameters]
    return []


def _generate_return(node, builder, symbols):
    if node.expression is None:
        builder.ret_void()
    else:
        builder.ret(builder.load(_expand_expression(node.expression, builder, symbols)))


def _expand_expression(node, builder, symbols):
    if issubclass(type(node), Atom):
        if type(node) is IntLiteral:
            return ir.Constant(ir.IntType(32), node.value)
        elif type(node) is BoolLiteral:
            return ir.Constant(ir.IntType(1), 1 if node.value is True else 0)
        elif type(node) is VariableReference:
            return symbols[node.name]
    if type(node) is BinaryOp:
        rhs = _expand_expression(node.right, builder, symbols)
        lhs = _expand_expression(node.left, builder, symbols)
        if node.op == "/":
            return lhs.udiv(rhs)
        elif node.op == "*":
            return lhs.mul(rhs)
        elif node.op == "+":
            return lhs.add(rhs)
        elif node.op == "-":
            return lhs.sub(rhs)


def _generate_assignment(node, builder, symbols):
    e = _expand_expression(node.expression, builder, symbols)
    p = builder.alloca(ir.IntType(32), 1)
    builder.store(e, p)
    symbols[node.name] = p


def _generate_if(node, builder, symbols):
    p = _expand_expression(node.predicate, builder, symbols)
    with builder.if_then(p):
        _generate_block(node.then, builder, symbols.copy())


def _generate_block(block, builder, symbols):
    for s in block.statements:
        if type(s) is Return:
            _generate_return(s, builder, symbols)
        elif type(s) is Assignment:
            _generate_assignment(s, builder, symbols)
        elif type(s) is If:
            _generate_if(s, builder, symbols)


def _generate_function(module, node):
    return_type = _make_type(node.return_type)
    parameter_types = _make_parameter_types(node)
    function_type = ir.FunctionType(return_type, parameter_types)
    function = ir.Function(module, function_type, name=node.name)
    block = function.append_basic_block(name="entry")
    builder = ir.IRBuilder(block)
    _generate_block(node.block, builder, {})


def build_ir(ast, debug=False):
    module = ir.Module(name=ast.root.module.name)
    module.triple = "x86_64-unknown-linux-gnu"
    for function in ast.root.functions:
        _generate_function(module, function)
        pass
    output = str(module)
    if debug:
        with open("ir.ll", "w") as fp:
            fp.write(output)
        os.system("llc ir.ll")
    return output


def make_binary(ir, out="test"):
    with open("tmp.ll", "w") as ll:
        ll.write(ir)
    os.system("llc -filetype=obj tmp.ll -o tmp.o")
    os.system(f"clang tmp.o -o {out}")
    os.remove("tmp.o")
    os.remove("tmp.ll")
