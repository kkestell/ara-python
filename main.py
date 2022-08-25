from parsing import parse
from code_generation import build_ir, make_binary
from syntax import build_ast

program = '''
module main

fn is_even(x: i64) -> bool {
    return x % 2 == 0
}

fn main() -> i64 {
  var r = 0
  if is_even(2) {
    r = 1
  }
  return r
}
'''

parse_tree = parse(program)
print(parse_tree.pretty())

ast = build_ast(parse_tree)
print(ast.pretty())

ir = build_ir(ast, debug=True)
print(ir)

make_binary(ir)
