# Ara

## Code Generation

<table>
<thead><tr><th>Source</th><th>IR</th></tr></thead>
<tbody>
<tr></tr>
<tr><td width="50%">

```
fn main() -> void {
  return
}

```
</td><td>

```llvm
define void @"main"()
{
entry:
}
```
</td></tr>
<tr></tr>
<tr><td>

```
fn main() -> int {
  return 0
}


```
</td><td>

```llvm
define i32 @"main"()
{
entry:
  ret i32 0
}
```
</td></tr>
<tr></tr>
<tr><td>

```
fn main() -> int {
  var x = 1
  return x
}


```
</td><td>

```llvm
define i32 @"main"()
{
entry:
  %".2" = alloca i32, i32 1
  store i32 1, i32* %".2"
  %".4" = load i32, i32* %".2"
  ret i32 %".4"
}
```
</td></tr>
</table>

## Another Example

### Source

```
module foo

fn main() -> int {
  int x = 1 + 2 * 3
  return x
}
```

### LLVM IR

```llvm
; ModuleID = "foo"
target triple = "x86_64-unknown-linux-gnu"
target datalayout = ""

define i32 @"main"()
{
entry:
  %"x" = add i32 0, add (i32 1, i32 mul (i32 2, i32 3))
  ret i32 %"x"
}
```

### Assembly

```
        .text
        .file   "tmp.ll"
        .globl  main                    # -- Begin function main
        .p2align        4, 0x90
        .type   main,@function
main:                                   # @main
        .cfi_startproc
# %bb.0:                                # %entry
        movl    $7, %eax
        retq
.Lfunc_end0:
        .size   main, .Lfunc_end0-main
        .cfi_endproc
                                        # -- End function
        .section        ".note.GNU-stack","",@progbits
```

### Link & Run

```
$ ./test
$ echo $?
7
```