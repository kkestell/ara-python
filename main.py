from parsing import parse
from code_generation import build_ir, make_binary
from syntax import build_ast

program = '''
module main

fn main() -> int {
  var x = 1
  return x
}
'''

parse_tree = parse(program)
print(parse_tree.pretty())

ast = build_ast(parse_tree)
print(ast.pretty())

ir = build_ir(ast, debug=True)
print(ir)

make_binary(ir)
