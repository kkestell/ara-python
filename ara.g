start: source_file
source_file: module functions
module: "module" NAME
functions: [function+]
function: "fn" NAME "(" parameters ")" "->" type block
parameters: [parameter ( "," parameter )*]
parameter: NAME ":" type
block: "{" statements "}"
statements: [statement+]
?statement: if
          | return
          | assignment
if: "if" expression block
return: "return" [expression]
assignment: "var" NAME "=" expression
type: TYPE
?expression: sum
?sum: product
    | sum PLUS product -> binary_op
    | sum MINUS product -> binary_op
?product: atom
        | product MULTIPLY atom -> binary_op
        | product DIVIDE atom -> binary_op
?atom: INT         -> int_literal
     | STRING      -> string_literal
     | "-" atom    -> neg
     | NAME        -> variable_reference
     | "(" sum ")"

TYPE: "int" | "void"
PLUS: "+"
MINUS: "-"
MULTIPLY: "*"
DIVIDE: "/"

%import common.ESCAPED_STRING -> STRING
%import common.SH_COMMENT     -> COMMENT
%import common.CNAME          -> NAME
%import common.INT
%import common.WS

%ignore WS
%ignore COMMENT