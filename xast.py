import sys
from abc import abstractmethod, ABC
from dataclasses import dataclass
from typing import List

from lark import ast_utils, Transformer, v_args
from lark.tree import Meta, Tree


class AstNode(ast_utils.Ast):
    @abstractmethod
    def pretty(self):
        pass


@dataclass
class Type(AstNode, ast_utils.WithMeta):
    meta: Meta
    value: str

    def pretty(self):
        return {
            "node": "type",
            "value": self.value
        }


class _Statement(AstNode):
    pass


class _Expression(AstNode):
    @property
    @abstractmethod
    def native_type(self):
        pass


class _Atom(_Expression):
    pass


@dataclass
class IntLiteral(_Atom, ast_utils.WithMeta):
    meta: Meta
    value: int

    @property
    def native_type(self):
        return type(int)

    def pretty(self):
        return {
            "node": "int_literal",
            "value": self.value
        }


@dataclass
class StringLiteral(_Atom, ast_utils.WithMeta):
    meta: Meta
    value: str

    @property
    def native_type(self):
        return type(str)

    def pretty(self):
        return {
            "node": "string_literal",
            "value": self.value
        }


@dataclass
class VariableReference(_Atom, ast_utils.WithMeta):
    meta: Meta
    name: str

    @property
    def native_type(self):
        return type(str)

    def pretty(self):
        return {
            "node": "variable_reference",
            "name": self.name
        }


@dataclass
class BinaryOp(_Expression, ast_utils.WithMeta):
    meta: Meta
    left: _Expression
    op: str
    right: _Expression

    @property
    def native_type(self):
        assert (self.left.native_type == self.right.native_type)
        return self.left.native_type

    def pretty(self):
        return {
            "node": "binary_op",
            "left": self.left.pretty(),
            "op": self.op,
            "right": self.right.pretty()
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
            "expression": self.expression.pretty()
        }


@dataclass
class If(_Statement, ast_utils.WithMeta):
    meta: Meta
    condition: _Expression
    then: List[_Statement]


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
    type: Type

    def pretty(self):
        return {
            "node": "parameter",
            "name": self.name,
            "type": self.type.pretty()
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
class Function(AstNode, ast_utils.WithMeta):
    meta: Meta
    name: str
    parameters: List[Parameter]
    return_type: Type
    block: Block

    def pretty(self):
        return {
            "node": "function",
            "name": self.name,
            "parameters": [x.pretty() for x in self.parameters],
            "return_type": self.return_type.pretty(),
            "block": self.block.pretty()
        }


@dataclass
class SourceFile(AstNode, ast_utils.WithMeta):
    meta: Meta
    module: Module
    functions: List[Function]

    def pretty(self):
        return {
            "node": "source_file",
            "module": self.module.pretty(),
            "functions": [x.pretty() for x in self.functions]
        }


class AstTransformer(Transformer):
    def STRING(self, s):
        return s[1:-1]

    def INT(self, n):
        return int(n)

    def NAME(self, s):
        return str(s)

    def TYPE(self, s):
        return str(s)

    def parameters(self, p):
        return p

    def statements(self, s):
        return s

    def functions(self, f):
        return f

    @v_args(inline=True)
    def start(self, x):
        return x


def build_ast(parse_tree: Tree):
    this_module = sys.modules[__name__]
    transformer = ast_utils.create_transformer(this_module, AstTransformer())
    return transformer.transform(parse_tree)
