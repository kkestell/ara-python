import os

from llvmlite import ir

from syntax import Return, IntLiteral, BinaryExpression, _Atom as Atom, If, BoolLiteral, VariableReference, Negate, \
    FloatLiteral, VariableDeclaration, Assignment, FunctionCall


def _make_type(t):
    if t == "i64":
        return ir.IntType(64)
    if t == "f64":
        return ir.DoubleType()
    elif t == "void":
        return ir.VoidType()
    elif t == "bool":
        return ir.IntType(1)
    else:
        raise Exception("Unknown type")


def _make_parameter_types(node):
    if node.parameters is not None:
        return [_make_type(p.type) for p in node.parameters]
    return []


def _return(node, builder, symbols):
    if node.expression is None:
        builder.ret_void()
    else:
        e = _expression(node.expression, builder, symbols)
        if e.type.is_pointer:
            v = builder.load(e)
            builder.ret(v)
        else:
            builder.ret(e)


def _atom(node, builder, symbols):
    if type(node) is IntLiteral:
        return ir.Constant(ir.IntType(64), node.value)
    if type(node) is FloatLiteral:
        return ir.Constant(ir.DoubleType(), node.value)
    elif type(node) is BoolLiteral:
        return ir.Constant(ir.IntType(1), 1 if node.value is True else 0)
    elif type(node) is VariableReference:
        if node.name in symbols["locals"]:
            return builder.load(symbols["locals"][node.name])
        elif node.name in symbols["parameters"]:
            return symbols["parameters"][node.name]["argument"]
    elif type(node) is FunctionCall:
        args = [_expression(a, builder, symbols) for a in node.arguments]
        return builder.call(symbols["functions"][node.name], args)
    elif type(node) is Negate:
        return builder.neg(_expression(node.atom, builder, symbols))


def _binary_expression(node, builder, symbols):
    rhs = _expression(node.right, builder, symbols)
    lhs = _expression(node.left, builder, symbols)
    if node.type == "i64":
        if node.op == "/":
            return builder.sdiv(lhs, rhs)
        elif node.op == "*":
            return builder.mul(lhs, rhs)
        elif node.op == "+":
            return builder.add(lhs, rhs)
        elif node.op == "-":
            return builder.sub(lhs, rhs)
        elif node.op == "%":
            return builder.sub(lhs, builder.mul(rhs, builder.sdiv(lhs, rhs)))
        else:
            return builder.icmp_signed(node.op, lhs, rhs)
    elif node.type == "f64":
        if node.op == "/":
            return builder.fdiv(lhs, rhs)
        elif node.op == "*":
            return builder.fmul(lhs, rhs)
        elif node.op == "+":
            return builder.fadd(lhs, rhs)
        elif node.op == "-":
            return builder.fsub(lhs, rhs)
        else:
            return builder.fcmp_ordered(node.op, lhs, rhs)


def _expression(node, builder, symbols):
    if issubclass(type(node), Atom):
        return _atom(node, builder, symbols)
    elif type(node) is BinaryExpression:
        return _binary_expression(node, builder, symbols)


def _variable_declaration(node, builder, symbols):
    e = _expression(node.expression, builder, symbols)
    p = builder.alloca(e.type, 1)
    builder.store(e, p)
    symbols["locals"][node.name] = p


def _if(node, builder, symbols):
    e = _expression(node.predicate, builder, symbols)
    with builder.if_then(e):
        _block(node.then, builder, symbols.copy())


def _assignment(node, builder, symbols):
    e = _expression(node.expression, builder, symbols)
    p = symbols["locals"][node.name]
    builder.store(e, p)


def _block(block, builder, symbols):
    for s in block.statements:
        if type(s) is Return:
            _return(s, builder, symbols)
        elif type(s) is VariableDeclaration:
            _variable_declaration(s, builder, symbols)
        elif type(s) is If:
            _if(s, builder, symbols)
        elif type(s) is Assignment:
            _assignment(s, builder, symbols)


def _function_definition(module, node, functions):
    return_type = _make_type(node.return_type)
    parameter_types = _make_parameter_types(node)
    function_type = ir.FunctionType(return_type, parameter_types)
    function = ir.Function(module, function_type, name=node.name)
    block = function.append_basic_block(name="entry")
    builder = ir.IRBuilder(block)
    parameters = {}
    for idx, p in enumerate(node.parameters):
        parameters[p.name] = {"type": p.type, "argument": function.args[idx]}
    _block(node.block, builder, {"parameters": parameters, "locals": {}, "functions": functions})
    return function


def build_ir(ast, debug=False):
    module = ir.Module(name=ast.root.module.name)
    module.triple = "x86_64-unknown-linux-gnu"
    functions = {}
    for f in ast.root.function_definitions:
        function = _function_definition(module, f, functions)
        functions[f.name] = function
    output = str(module)
    if debug:
        with open("ir.ll", "w") as fp:
            fp.write(output)
        os.system("llc ir.ll")
    return output


def make_binary(ir_code, out="test"):
    with open("tmp.ll", "w") as ll:
        ll.write(ir_code)
    os.system("llc -filetype=obj tmp.ll -o tmp.o")
    #os.system("llc tmp.ll -O0 -o tmp.s")
    os.system(f"clang tmp.o -o {out}")
    os.remove("tmp.o")
    #os.remove("tmp.s")
    os.remove("tmp.ll")
