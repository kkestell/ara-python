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
    | 'return' expression?                                             # returnStatement
    | 'if' '(' expression ')' block                                    # ifStatement
    | expression                                                       # expressionStatement
    ;

expression
    : '(' expression ')'                                               # parensExpression
    | op=('+' | '-') expression                                        # unaryExpression
    | left=expression op=('*' | '/' | '%') right=expression            # infixExpression
    | left=expression op=('+' | '-') right=expression                  # infixExpression
    | left=expression op=('<' | '<=' | '>' | '>=') right=expression    # infixExpression
    | left=expression op=('==' | '!=') right=expression                # infixExpression
    | name '(' arguments+=expression* (',' arguments+=expression)* ')' # functionCall
    | value=STRING                                                     # stringLiteral
    | value=atom                                                       # valueExpression
    ;

atom
    : number
    | bool
    | name
    ;

number
    : value=SCIENTIFIC_NUMBER
    ;

bool
    : 'true'  # true
    | 'false' # false
    ;

ID
    : [a-zA-Z][a-zA-Z0-9_]*
    ;

name
   : value=ID
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
