# Ara

## Example

### Source

```
module main

fn main() -> i64 {
  var x = 1
  var y = 1.2576543
  if y < 1.0 {
    x = 2
  }
  return x * 2
}
```

### Parse Tree

```
start
  source_file
    module	main
    function_definitions
      function_definition
        main
        parameters	None
        i64
        block
          statements
            variable_declaration
              x
              int_literal	1
            variable_declaration
              y
              float_literal	1.2576543
            if
              binary_expression
                variable_reference	y
                <
                float_literal	1.0
              block
                statements
                  assignment
                    x
                    int_literal	2
            return
              binary_expression
                variable_reference	x
                *
                int_literal	2
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
      "name": "main",
      "parameters": [],
      "return_type": "i64",
      "block": {
        "node": "block",
        "statements": [
          {
            "node": "variable_declaration",
            "name": "x",
            "expression": {
              "node": "int_literal",
              "type": "i64",
              "value": 1
            }
          },
          {
            "node": "variable_declaration",
            "name": "y",
            "expression": {
              "node": "float_literal",
              "type": "f64",
              "value": 1.2576543
            }
          },
          {
            "node": "if",
            "predicate": {
              "node": "binary_expression",
              "type": "f64",
              "left": {
                "node": "variable_reference",
                "type": "f64",
                "name": "y"
              },
              "op": "<",
              "right": {
                "node": "float_literal",
                "type": "f64",
                "value": 1.0
              }
            },
            "then": {
              "node": "block",
              "statements": [
                {
                  "node": "assignment",
                  "name": "x",
                  "expression": {
                    "node": "int_literal",
                    "type": "i64",
                    "value": 2
                  }
                }
              ]
            }
          },
          {
            "node": "return",
            "expression": {
              "node": "binary_expression",
              "type": "i64",
              "left": {
                "node": "variable_reference",
                "type": "i64",
                "name": "x"
              },
              "op": "*",
              "right": {
                "node": "int_literal",
                "type": "i64",
                "value": 2
              }
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

define i64 @"main"()
{
entry:
  %".2" = alloca i64, i32 1
  store i64 1, i64* %".2"
  %".4" = alloca double, i32 1
  store double 0x3ff41f5a1d82c7c6, double* %".4"
  %".6" = load double, double* %".4"
  %".7" = fcmp olt double %".6", 0x3ff0000000000000
  br i1 %".7", label %"entry.if", label %"entry.endif"
entry.if:
  store i64 2, i64* %".2"
  br label %"entry.endif"
entry.endif:
  %".11" = load i64, i64* %".2"
  %".12" = mul i64 %".11", 2
  ret i64 %".12"
}
```

### Run

```
$ ./test
$ echo $?
2
```