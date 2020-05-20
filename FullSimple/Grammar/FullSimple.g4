grammar FullSimple;

toplevel: command ';' toplevel | EOF		;
command	: term | UCID tybinder | LCID binder;
binder: COLON type | EQ term				;
type: arrowtype;
atype: LPAREN type RPAREN 
	 | UCID
	 | BOOL
	 | LT fieldtypes GT
	 | USTRING
	 | UUNIT
	 | LCURLY fieldtypes RCURLY
	 | UFLOAT
	 | NAT;
tybinder: EQ type;
fieldtypes: nefieldtypes;
nefieldtypes: fieldtype 
			| fieldtype COMMA nefieldtypes;
fieldtype : LCID COLON type
          | type;
arrowtype: atype ARROW arrowtype | atype;
term	:  appterm
		| IF term THEN term ELSE term
		| CASE term OF cases
		| LAMBDA LCID COLON type DOT term
		| LAMBDA USCORE COLON type DOT term
		| LET LCID EQ term IN term
		| LET USCORE EQ term IN term
		| LETREC LCID COLON type EQ term IN term;
appterm : pathterm
		| appterm pathterm
		| FIX pathterm
		| TIMESFLOAT pathterm pathterm
		| SUCC pathterm
		| PRED pathterm
		| ISZERO pathterm;
ascribeterm: aterm AS type
		   | aterm;
pathterm : pathterm DOT LCID
		 | pathterm DOT INTV
		 | ascribeterm;
termseq: term 
	   | term SEMI termseq;
aterm : LPAREN termseq RPAREN
      | INERT LSQUARE type RSQUARE
      | TRUE
      | FALSE
      | LT LCID EQ term GT AS type
      | LCID
      | STRINGV
      | UNIT
      | LCURLY fields RCURLY
      | FLOATV
      | INTV;
cases : case
      | case VBAR cases;
case : LT LCID EQ LCID GT DDARROW appterm;
fields : nefields;
nefields : field
         | field COMMA nefields;
field : LCID EQ term
      | term;


UCID : [A-Z][a-zA-Z]*;
LCID : [a-z]+;

/** Keywords **/

TYPE	: 'type';
INERT	: 'inert';
IF		: 'if';
THEN	: 'then';
ELSE	: 'else';
TRUE	: 'true';
FALSE	: 'false';
BOOL	: 'Bool';
CASE	: 'case';
OF		: 'of';
AS		: 'as';
LAMBDA	: 'lambda';
LET		: 'let';
IN		: 'in';
FIX		: 'fix';
LETREC	: 'letrec';
USTRING	: 'String';
UNIT	: 'unit';
UUNIT	: 'Unit';
TIMESFLOAT	: 'timesfloat';
UFLOAT	: 'Float';
SUCC	: 'succ';
PRED	: 'pred';
ISZERO	: 'iszero';
NAT		: 'Nat';

STRINGV: DQUOTE [a-zA-Z]+ DQUOTE;

/** Symbols **/
USCORE : '_';
APOSTROPHE : '\'';
DQUOTE : '"';
BANG : '!';
HASH : '#';
TRIANGLE : '$';
STAR : '*';
VBAR : '|';
DOT : '.';
SEMI : ';';
COMMA : ',';
SLASH : '/';
COLON : ':';
COLONCOLON : '::';
EQ : '=';
EQEQ : '==';
LSQUARE : '[';
LT : '<';
LCURLY : '{';
LPAREN : '(';
LEFTARROW : '<-';
LCURLYBAR : '{|';
LSQUAREBAR : '[|';
RCURLY : '}';
RPAREN : ')';
RSQUARE : ']';
GT : '>';
BARRCURLY : '|}';
BARGT : '|>';
BARRSQUARE : '|]';

/** Special compound symbols **/
COLONEQ : ':=';
ARROW  : '->';
DARROW : '=>';
DDARROW : '==>';

FLOATV	: [1-9]+(.[1-9]+)?;
INTV	: [1-9]+;
VAR		: [a-z];
WS      : ' ' -> skip;
NL      : '\r' -> skip;
NL1		: '\n' -> skip;
