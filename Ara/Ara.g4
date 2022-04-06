grammar Ara;

program
    : (functions+=function)*
    ;

function
    : type name '(' parameters+=parameter* (',' parameters+=parameter)* ')' block
    ;

parameter
    : type name
    ;

block
    : '{' statements+=statement* '}'
    ;

statement
    : type name '=' expression                                         # variableInitialization
    | type name                                                        # variableDeclaration
    | 'return' expression?                                             # return
    | 'if' '(' expression ')' block                                    # if
    | name '=' expression                                              # assignment
    ;

expression
    : '(' expression ')'                                               # parens
    | op=('+' | '-') expression                                        # unary
    | left=expression op=('*' | '/' | '%') right=expression            # infix
    | left=expression op=('+' | '-') right=expression                  # infix
    | left=expression op=('<' | '<=' | '>' | '>=') right=expression    # infix
    | left=expression op=('==' | '!=') right=expression                # infix
    | name '(' arguments+=expression* (',' arguments+=expression)* ')' # functionCall
    | value=atom                                                       # atomExpr
    ;

atom
    : number
    | bool
    | string
    | name
    ;

number
    : value=SCIENTIFIC_NUMBER
    ;

bool
    : 'true'  # true
    | 'false' # false
    ;

string
    : value=STRING
    ;

name
    : value=ID
    ;

ID
    : [a-zA-Z][a-zA-Z0-9_]*
    ;

STRING
    : '"' (ESC | ~( '\\' | '"' ))* '"'
    ;

ESC
    : '\\' ('n' | 'r')
    ;

type
    : value=ID
    ;

SCIENTIFIC_NUMBER
   : NUMBER (('E' | 'e') ('+' | '-')? NUMBER)?
   ;

fragment NUMBER
   : ('0' .. '9') + ('.' ('0' .. '9') +)?
   ;

OP_ADD
   : '+'
   ;

OP_SUB
   : '-'
   ;

OP_MUL
   : '*'
   ;

OP_DIV
    : '/'
    ;

OP_MOD
    : '%'
    ;

OP_EQ
    : '=='
    ;

OP_NE
    : '!='
    ;

OP_LT
    : '<'
    ;

OP_LT_EQ
    : '<='
    ;

OP_GT
    : '>'
    ;

OP_GT_EQ
    : '>='
    ;

WS
    : (' ' | '\t' | '\n')+ -> skip
    ;
