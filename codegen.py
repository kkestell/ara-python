from typing import cast, List

from llvmlite import ir

from xast import SourceFile, Type, Return, IntLiteral, Assignment, Function, BinaryOp, \
    _Expression as Expression, _Atom as Atom, If, BoolLiteral, Block, _Statement, VariableReference


class CodeGenerator:
    def __init__(self):
        self.module = None

    @staticmethod
    def make_type(t: Type):
        if t.value == "int":
            return ir.IntType(32)
        elif t.value == "void":
            return ir.VoidType()
        elif t.value == "bool":
            return ir.IntType(1)
        else:
            raise Exception("Unknown type")

    @staticmethod
    def make_parameter_types(function_node: Function):
        if function_node.parameters is not None:
            return [CodeGenerator.make_type(p.type) for p in function_node.parameters]
        return ()

    def generate_return(self, node, builder, symbols):
        if node.expression is None:
            builder.ret_void()
        else:
            builder.ret(self.expand_expression(node.expression, builder, symbols))

    def expand_expression(self, node, builder, symbols):
        if issubclass(type(node), Atom):
            if type(node) is IntLiteral:
                return ir.Constant(ir.IntType(32), node.value)
            elif type(node) is BoolLiteral:
                return ir.Constant(ir.IntType(1), 1 if node.value is True else 0)
            elif type(node) is VariableReference:
                return symbols[node.name]
        if type(node) is BinaryOp:
            binary_op = cast(BinaryOp, node)
            rhs = self.expand_expression(binary_op.right, builder, symbols)
            lhs = self.expand_expression(binary_op.left, builder, symbols)
            if binary_op.op == "/":
                return lhs.udiv(rhs)
            elif binary_op.op == "*":
                return lhs.mul(rhs)
            elif binary_op.op == "+":
                return lhs.add(rhs)
            elif binary_op.op == "-":
                return lhs.sub(rhs)

    def generate_assignment(self, node, builder, symbols):
        e = self.expand_expression(node.expression, builder, symbols)
        v = builder.add(ir.Constant(ir.IntType(32), 0), e, node.name)
        symbols[node.name] = v

    def generate_if(self, node, builder, symbols):
        p = self.expand_expression(node.predicate, builder, symbols)
        with builder.if_then(p):
            self.generate_block(node.then, builder, symbols.copy())

    def generate_block(self, block, builder, symbols=None):
        if symbols is None:
            symbols = {}
        for s in block.statements:
            if type(s) is Return:
                self.generate_return(s, builder, symbols)
            elif type(s) is Assignment:
                self.generate_assignment(s, builder, symbols)
            elif type(s) is If:
                self.generate_if(s, builder, symbols)

    def generate_function(self, node):
        return_type = self.make_type(node.return_type)
        parameter_types = CodeGenerator.make_parameter_types(node)
        function_type = ir.FunctionType(return_type, parameter_types)
        function = ir.Function(self.module, function_type, name=node.name)
        block = function.append_basic_block(name="entry")
        builder = ir.IRBuilder(block)
        self.generate_block(node.block, builder)

    def generate_ir(self, source_file_node: SourceFile):
        self.module = ir.Module(name=source_file_node.module.name)
        self.module.triple = "x86_64-unknown-linux-gnu"
        for function in source_file_node.functions:
            self.generate_function(function)
            pass
        return str(self.module)


def generate_ir(source_file_node: SourceFile):
    return CodeGenerator().generate_ir(source_file_node)
