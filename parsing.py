from lark import Lark


def parse(program: str):
    return Lark.open('ara.g', propagate_positions=True).parse(program)
