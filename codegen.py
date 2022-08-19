from typing import cast

from llvmlite import ir

from xast import SourceFile, Type, Return, IntLiteral, Assignment, Function, BinaryOp, \
    _Expression as Expression, _Atom as Atom


class CodeGenerator:
    def __init__(self):
        self.module = None

    @staticmethod
    def make_type(t: Type):
        if t.value == "int":
            return ir.IntType(32)
        elif t.value == "void":
            return ir.VoidType()
        else:
            raise Exception("Unknown type")

    @staticmethod
    def make_parameter_types(function_node: Function):
        if function_node.parameters is not None:
            return [CodeGenerator.make_type(p.type) for p in function_node.parameters]
        return ()

    def generate_return(self, return_node: Return, builder: ir.IRBuilder):
        if return_node.expression is None:
            builder.ret_void()
        else:
            expression = self.expand_expression(return_node.expression, builder)
            builder.ret(ir.Constant(ir.IntType(32), expression))

    def expand_expression(self, expression_node: Expression, builder: ir.IRBuilder):
        if issubclass(type(expression_node), Atom):
            if type(expression_node) is IntLiteral:
                atom = cast(IntLiteral, expression_node)
                return ir.Constant(ir.IntType(32), atom.value)
        if type(expression_node) is BinaryOp:
            binary_op = cast(BinaryOp, expression_node)
            rhs = self.expand_expression(binary_op.right, builder)
            lhs = self.expand_expression(binary_op.left, builder)
            if binary_op.op == "/":
                return lhs.udiv(rhs)
            elif binary_op.op == "*":
                return lhs.mul(rhs)
            elif binary_op.op == "+":
                return lhs.add(rhs)
            elif binary_op.op == "-":
                return lhs.sub(rhs)

    def generate_assignment(self, assignment_node: Assignment, builder: ir.IRBuilder):
        expression = self.expand_expression(assignment_node.expression, builder)
        builder.add(ir.Constant(ir.IntType(32), 0), expression, assignment_node.name)

    def generate_function(self, function_node: Function):
        return_type = self.make_type(function_node.return_type)
        parameter_types = CodeGenerator.make_parameter_types(function_node)
        function_type = ir.FunctionType(return_type, parameter_types)
        function = ir.Function(self.module, function_type, name=function_node.name)
        block = function.append_basic_block(name="entry")
        builder = ir.IRBuilder(block)
        for statement in function_node.block.statements:
            if type(statement) is Return:
                self.generate_return(cast(Return, statement), builder)
            elif type(statement) is Assignment:
                self.generate_assignment(cast(Return, statement), builder)

    def generate_code(self, source_file_node: SourceFile):
        self.module = ir.Module(name=source_file_node.module.name)
        for function in source_file_node.functions:
            self.generate_function(function)
            pass
        return str(self.module)


def generate_code(source_file_node: SourceFile):
    return CodeGenerator().generate_code(source_file_node)
