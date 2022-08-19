import json
from parser import parse
from codegen import generate_code
from xast import build_ast

program = '''
module foo

fn bar(foo: int) -> void {
}

fn main(argc: int) -> int {
  var x = 1 + 2 * 3
  return 0
}
'''
parse_tree = parse(program)
print(parse_tree.pretty())
ast = build_ast(parse_tree)
print(json.dumps(ast.pretty(), indent=2))
code = generate_code(ast)
print(code)
