import json
from parser import parse
from codegen import generate_ir
from compiler import compile
from xast import build_ast

program = '''
module foo
fn main() -> int {
  var x = 1 + 2 * 3
  return x
}
'''
parse_tree = parse(program)
print(parse_tree.pretty())
ast = build_ast(parse_tree)
print(json.dumps(ast.pretty(), indent=2))
ir = generate_ir(ast)
with open("ir.ll", "w") as ll:
    ll.write(ir)
print(ir)
compile(ir)
