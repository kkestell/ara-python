import dataclasses
import json
import re
import sys
from abc import abstractmethod
from dataclasses import dataclass
from keyword import iskeyword
from typing import List

from lark import ast_utils, Transformer
from lark.tree import Meta


class AstNode(ast_utils.Ast):
    @abstractmethod
    def pretty(self):
        pass


class _Statement(AstNode):
    @abstractmethod
    def pretty(self):
        pass


class _Expression(AstNode):
    @abstractmethod
    def pretty(self):
        pass


class _Atom(_Expression):
    type: type = None

    @abstractmethod
    def pretty(self):
        pass


@dataclass
class Negate(_Atom, ast_utils.WithMeta):
    meta: Meta
    atom: _Atom

    def pretty(self):
        return {
            "type": self.type,
            "node": "negate",
            "atom": self.atom.pretty()
        }


@dataclass
class FunctionCall(_Atom, ast_utils.WithMeta):
    meta: Meta
    name: str
    arguments: List[_Expression]

    def pretty(self):
        return {
            "type": self.type,
            "node": "function_call",
            "name": self.name,
            "arguments": [x.pretty() for x in self.arguments]
        }


@dataclass
class IntLiteral(_Atom, ast_utils.WithMeta):
    meta: Meta
    value: int

    def pretty(self):
        return {
            "node": "int_literal",
            "type": self.type,
            "value": self.value
        }


@dataclass
class FloatLiteral(_Atom, ast_utils.WithMeta):
    meta: Meta
    value: float

    def pretty(self):
        return {
            "node": "float_literal",
            "type": self.type,
            "value": self.value
        }


@dataclass
class StringLiteral(_Atom, ast_utils.WithMeta):
    meta: Meta
    value: str

    def pretty(self):
        return {
            "node": "string_literal",
            "type": self.type,
            "value": self.value
        }


@dataclass
class BoolLiteral(_Atom, ast_utils.WithMeta):
    meta: Meta
    value: bool

    def pretty(self):
        return {
            "node": "bool_literal",
            "type": self.type,
            "value": self.value
        }


@dataclass
class VariableReference(_Atom, ast_utils.WithMeta):
    meta: Meta
    name: str

    def pretty(self):
        return {
            "node": "variable_reference",
            "type": self.type,
            "name": self.name
        }


@dataclass
class BinaryExpression(_Expression, ast_utils.WithMeta):
    meta: Meta
    left: _Expression
    op: str
    right: _Expression

    def pretty(self):
        return {
            "node": "binary_expression",
            "type": self.type,
            "left": self.left.pretty(),
            "op": self.op,
            "right": self.right.pretty()
        }


@dataclass
class VariableDeclaration(_Statement, ast_utils.WithMeta):
    meta: Meta
    name: str
    expression: _Expression

    def pretty(self):
        return {
            "node": "variable_declaration",
            "name": self.name,
            "expression": self.expression.pretty()
        }


@dataclass
class Assignment(_Statement, ast_utils.WithMeta):
    meta: Meta
    name: str
    expression: _Expression

    def pretty(self):
        return {
            "node": "assignment",
            "name": self.name,
            "expression": self.expression.pretty()
        }


@dataclass
class Return(_Statement, ast_utils.WithMeta):
    meta: Meta
    expression: _Expression

    def pretty(self):
        return {
            "node": "return",
            "expression": self.expression.pretty() if self.expression is not None else None
        }


@dataclass
class Block(AstNode, ast_utils.WithMeta):
    meta: Meta
    statements: List[_Statement]

    def pretty(self):
        return {
            "node": "block",
            "statements": [x.pretty() for x in self.statements]
        }


@dataclass
class If(_Statement, ast_utils.WithMeta):
    meta: Meta
    predicate: _Expression
    then: Block

    def pretty(self):
        return {
            "node": "if",
            "predicate": self.predicate.pretty(),
            "then": self.then.pretty()
        }


@dataclass
class Module(AstNode, ast_utils.WithMeta):
    meta: Meta
    name: str

    def pretty(self):
        return {
            "node": "module",
            "name": self.name
        }


@dataclass
class Parameter(AstNode, ast_utils.WithMeta):
    meta: Meta
    name: str
    type: str

    def pretty(self):
        return {
            "node": "parameter",
            "name": self.name,
            "type": self.type
        }


@dataclass
class FunctionDefinition(AstNode, ast_utils.WithMeta):
    meta: Meta
    name: str
    parameters: List[Parameter]
    return_type: str
    block: Block

    def pretty(self):
        return {
            "node": "function_definition",
            "name": self.name,
            "parameters": [x.pretty() for x in self.parameters],
            "return_type": self.return_type,
            "block": self.block.pretty()
        }


@dataclass
class SourceFile(AstNode, ast_utils.WithMeta):
    meta: Meta
    module: Module
    function_definitions: List[FunctionDefinition]

    def pretty(self):
        return {
            "node": "source_file",
            "module": self.module.pretty(),
            "function_definitions": [x.pretty() for x in self.function_definitions]
        }


class Ast(object):
    def __init__(self, root):
        self.root = root
    
    def pretty(self):
        return json.dumps(self.root.pretty(), indent=2)


class AstTransformer(Transformer):
    def STRING(self, x):
        return x[1:-1]

    def INT(self, x):
        return int(x)

    def FLOAT(self, x):
        return float(x)

    def NAME(self, x):
        return str(x)

    def BOOL(self, x):
        return bool(x)

    def TYPE(self, x):
        return str(x)

    def parameters(self, x):
        if x[0] is None:
            return []
        return x

    def arguments(self, x):
        if x[0] is None:
            return []
        return x

    def statements(self, x):
        return x

    def function_definitions(self, x):
        return x

    def start(self, x):
        return x[0]


class AstVisitor(object):
    def visit(self, node):
        if isinstance(node, list):
            for c in node:
                self.visit(c)
        elif dataclasses.is_dataclass(node):
            meth_name = self._camel_to_snake(type(node).__name__)
            if iskeyword(meth_name):
                meth_name = f"{meth_name}_"
            enter_meth_name = f"enter_{meth_name}"
            exit_meth_name = f"exit_{meth_name}"
            if hasattr(self, enter_meth_name):
                meth = getattr(self, enter_meth_name)
                meth(node)
            for f in dataclasses.fields(type(node)):
                child = getattr(node, f.name)
                self.visit(child)
            if hasattr(self, exit_meth_name):
                meth = getattr(self, exit_meth_name)
                meth(node)

    @staticmethod
    def _camel_to_snake(name):
        name = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', name)
        return re.sub('([a-z0-9])([A-Z])', r'\1_\2', name).lower()


class TypeExpressions(AstVisitor):
    def __init__(self):
        self.symbols = {}

    def enter_function_definition(self, node):
        for p in node.parameters:
            self.symbols[p.name] = p.type

    def exit_int_literal(self, node):
        node.type = "i64"

    def exit_float_literal(self, node):
        node.type = "f64"

    def exit_bool_literal(self, node):
        node.type = "bool"

    def exit_binary_expression(self, node):
        assert(node.left.type == node.right.type)
        node.type = node.left.type

    def exit_negate(self, node):
        node.type = node.atom.type

    def exit_variable_declaration(self, node):
        node.type = node.expression.type
        self.symbols[node.name] = node.type

    def exit_return_(self, node):
        node.type = node.expression.type

    def exit_variable_reference(self, node):
        node.type = self.symbols[node.name]


def build_ast(parse_tree):
    this_module = sys.modules[__name__]
    transformer = ast_utils.create_transformer(this_module, AstTransformer())
    ast = Ast(transformer.transform(parse_tree))
    for f in ast.root.function_definitions:
        TypeExpressions().visit(f)
    return ast
