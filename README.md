# Ara

## Example

### Source

```rust
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
```

### Parse Tree

```
start
  source_file
    module	main
    function_definitions
      function_definition
        is_even
        parameters
          parameter
            x
            i64
        bool
        block
          statements
            return
              binary_expression
                binary_expression
                  variable_reference	x
                  %
                  int_literal	2
                ==
                int_literal	0
      function_definition
        main
        parameters	None
        i64
        block
          statements
            variable_declaration
              r
              int_literal	0
            if
              function_call
                is_even
                arguments
                  int_literal	2
              block
                statements
                  assignment
                    r
                    int_literal	1
            return
              variable_reference	r
```

### AST

```json
{
  "node": "source_file",
  "module": {
    "node": "module",
    "name": "main"
  },
  "function_definitions": [
    {
      "node": "function_definition",
      "name": "is_even",
      "parameters": [
        {
          "node": "parameter",
          "name": "x",
          "type": "i64"
        }
      ],
      "return_type": "bool",
      "block": {
        "node": "block",
        "statements": [
          {
            "node": "return",
            "expression": {
              "node": "binary_expression",
              "type": "i64",
              "left": {
                "node": "binary_expression",
                "type": "i64",
                "left": {
                  "node": "variable_reference",
                  "type": "i64",
                  "name": "x"
                },
                "op": "%",
                "right": {
                  "node": "int_literal",
                  "type": "i64",
                  "value": 2
                }
              },
              "op": "==",
              "right": {
                "node": "int_literal",
                "type": "i64",
                "value": 0
              }
            }
          }
        ]
      }
    },
    {
      "node": "function_definition",
      "name": "main",
      "parameters": [],
      "return_type": "i64",
      "block": {
        "node": "block",
        "statements": [
          {
            "node": "variable_declaration",
            "name": "r",
            "expression": {
              "node": "int_literal",
              "type": "i64",
              "value": 0
            }
          },
          {
            "node": "if",
            "predicate": {
              "type": null,
              "node": "function_call",
              "name": "is_even",
              "arguments": [
                {
                  "node": "int_literal",
                  "type": "i64",
                  "value": 2
                }
              ]
            },
            "then": {
              "node": "block",
              "statements": [
                {
                  "node": "assignment",
                  "name": "r",
                  "expression": {
                    "node": "int_literal",
                    "type": "i64",
                    "value": 1
                  }
                }
              ]
            }
          },
          {
            "node": "return",
            "expression": {
              "node": "variable_reference",
              "type": "i64",
              "name": "r"
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
; ModuleID = "main"
target triple = "x86_64-unknown-linux-gnu"
target datalayout = ""

define i1 @"is_even"(i64 %".1")
{
entry:
  %".3" = sdiv i64 %".1", 2
  %".4" = mul i64 2, %".3"
  %".5" = sub i64 %".1", %".4"
  %".6" = icmp eq i64 %".5", 0
  ret i1 %".6"
}

define i64 @"main"()
{
entry:
  %".2" = alloca i64, i32 1
  store i64 0, i64* %".2"
  %".4" = call i1 @"is_even"(i64 2)
  br i1 %".4", label %"entry.if", label %"entry.endif"
entry.if:
  store i64 1, i64* %".2"
  br label %"entry.endif"
entry.endif:
  %".8" = load i64, i64* %".2"
  ret i64 %".8"
}
```

### Run

```
$ ./test
$ echo $?
1
```