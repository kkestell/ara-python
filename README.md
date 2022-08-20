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
fn main(argc: int) -> int {
  var x = 1
  return x
}


```
</td><td>

```llvm
define i32 @"main"(i32 %".1")
{
entry:
  %"x" = add i32 0, 1
  ret i32 %"x"
}
```
</td></tr>
<tr></tr>
<tr><td>

```
fn main() -> int {
  return 1 + 2
}


```
</td><td>

```llvm
define i32 @"main"()
{
entry:
  ret i32 add (i32 1, i32 2)
}
```
</td></tr>
<tr></tr>
<tr><td>

```
fn main() -> int {
  var x = 0
  return x
}


```
</td><td>

```llvm
define i32 @"main"()
{
entry:
  %"x" = add i32 0, 0
  ret i32 %"x"
}
```
</td></tr>
<tr></tr>
<tr><td>

```
fn main() -> int {
  var x = 1 / 2 + 3 * 4 - 5
  return x
}


```
</td><td>

```llvm
define i32 @"main"()
{
entry:
  %"x" = add i32 0, sub (i32 add (i32 udiv (i32 1, i32 2), i32 mul (i32 3, i32 4)), i32 5)
  ret i32 %"x"
}
```
</td></tr>
<tr></tr>
<tr><td>

```
fn main() -> int {
  if (true) {
    return 0
  }
  return 1
}




```
</td><td>

```llvm
define i32 @"main"()
{
entry:
  br i1 1, label %"entry.if", label %"entry.endif"
entry.if:
  ret i32 0
entry.endif:
  ret i32 1
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
$ llc ir.ll
$ cat ir.s
```

```
        .text
        .file   "ir.ll"
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
$ clang -o a.out test.o
$ ./a.out 
```