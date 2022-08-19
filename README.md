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
target triple = "x86_64-unknown-linux-gnu"
target datalayout = ""

define i32 @"main"(i32 %".1")
{
entry:
  %"x" = add i32 0, add (i32 1, i32 mul (i32 2, i32 3))
  ret i32 i32 0
}
```

### Assembly

```
$ llc ir.ll
$ cat ir.s
```

```
	.text
	.file	"ir.ll"
	.globl	main                        # -- Begin function main
	.p2align	4, 0x90
	.type	main,@function
main:                                   # @main
	.cfi_startproc
# %bb.0:                                # %entry
	movl	$42, %eax
	retq
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
	.cfi_endproc
                                        # -- End function
	.section	".note.GNU-stack","",@progbits
```

### Link & Run

```
$ clang -o a.out test.o
$ ./a.out 
$ echo $?
42
```