start                       : source_file

source_file                 : module function_definitions

module                      : "module" NAME

function_definitions        : [function_definition+]

function_definition         : "fn" NAME "(" parameters ")" "->" TYPE block

parameters                  : [parameter ( "," parameter )*]

parameter                   : NAME ":" TYPE

block                       : "{" statements "}"

statements                  : [statement+]

?statement                  : if
                            | return
                            | variable_declaration
                            | assignment

if                          : "if" expression block

return                      : "return" [expression]

variable_declaration        : "var" NAME "=" expression

assignment                  : NAME "=" expression

?expression                 : relational_expression

?relational_expression      : equality_expression
                            | relational_expression RELATIONAL_OP equality_expression       -> binary_expression

?equality_expression        : additive_expression
                            | equality_expression EQUALITY_OP additive_expression           -> binary_expression

?additive_expression        : multiplicative_expression
                            | additive_expression ADDITIVE_OP multiplicative_expression     -> binary_expression

?multiplicative_expression  : atom
                            | multiplicative_expression MULTIPLICATIVE_OP atom              -> binary_expression

?atom                       : INT                                                           -> int_literal
                            | FLOAT                                                         -> float_literal
                            | STRING                                                        -> string_literal
                            | BOOL                                                          -> bool_literal
                            | "-" atom                                                      -> negate
                            | NAME                                                          -> variable_reference
                            | "(" additive_expression ")"

TYPE                        : "void" | "i64" | "f64" | "bool"
BOOL                        : "true" | "false"
ADDITIVE_OP                 : "+" | "-"
MULTIPLICATIVE_OP           : "*" | "/" | "%"
RELATIONAL_OP               : ">" | ">=" | "<" | "<="
EQUALITY_OP                 : "==" | "!="

%import common.ESCAPED_STRING -> STRING
%import common.SH_COMMENT     -> COMMENT
%import common.CNAME          -> NAME
%import common.INT
%import common.FLOAT
%import common.WS

%ignore WS
%ignore COMMENT