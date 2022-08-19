# Ara

## Examples

### Source

```
module foo

fn main(argc: int) -> int {
  int x = 1 + 2 * 3
  return 0
}
```

### Parse Tree

```
start
  source_file
    module	foo
    functions
      function
        bar
        parameters
          parameter
            foo
            type	int
        type	void
        block
          statements
      function
        main
        parameters
          parameter
            argc
            type	int
        type	int
        block
          statements
            assignment
              x
              binary_op
                int_literal	1
                +
                binary_op
                  int_literal	2
                  *
                  int_literal	3
            return
              int_literal	0
```

### AST

```json
{
  "node": "source_file",
  "module": {
    "node": "module",
    "name": "foo"
  },
  "functions": [
    {
      "node": "function",
      "name": "main",
      "parameters": [
        {
          "node": "parameter",
          "name": "argc",
          "type": {
            "node": "type",
            "value": "int"
          }
        }
      ],
      "return_type": {
        "node": "type",
        "value": "int"
      },
      "block": {
        "node": "block",
        "statements": [
          {
            "node": "assignment",
            "name": "x",
            "expression": {
              "node": "binary_op",
              "left": {
                "node": "int_literal",
                "value": 1
              },
              "op": "+",
              "right": {
                "node": "binary_op",
                "left": {
                  "node": "int_literal",
                  "value": 2
                },
                "op": "*",
                "right": {
                  "node": "int_literal",
                  "value": 3
                }
              }
            }
          },
          {
            "node": "return",
            "expression": {
              "node": "int_literal",
              "value": 0
            }
          }
        ]
      }
    }
  ]
}
```

### LLVM IR

```llvm
; ModuleID = "foo"
target triple = "unknown-unknown-unknown"
target datalayout = ""

define i32 @"main"(i32 %".1")
{
entry:
  %"x" = add i32 0, add (i32 1, i32 mul (i32 2, i32 3))
  ret i32 i32 0
}
```
