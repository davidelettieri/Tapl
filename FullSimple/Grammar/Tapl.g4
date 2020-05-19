grammar Tapl;

toplevel: command ';' toplevel | EOF		;
command	: (bind|term)						;
bind	: BIND VAR							;
term	: '(' term ')'						#par
		| VAR								#var
		| term term							#app
		| 'if' term 'then' term 'else' term #ift
		| 'true'							#true
		| 'false'							#false
		| LAMBDA VAR ':' type DOT term		#abs;
type	: type '->' type					#arrow
		| 'Bool'							#bool;

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
FLOAT	: 'Float';
SUCC	: 'succ';
PRED	: 'pred';
ISZERO	: 'iszero';
NAT		: 'Nat';

/** Symbols **/
USCORE : "_";
APOSTROPHE : "'";
DQUOTE : "\"";
BANG : "!";
HASH : "#";
TRIANGLE : "$";
STAR : "*";
VBAR : "|";
DOT : ".";
SEMI : ";";
COMMA : ",";
SLASH : "/";
COLON : ":";
COLONCOLON : "::";
EQ : "=";
EQEQ : "==";
LSQUARE : "[";
LT : "<";
LCURLY : "{";
LPAREN : "(";
LEFTARROW : "<-";
LCURLYBAR : "{|";
LSQUAREBAR : "[|";
RCURLY : "}";
RPAREN : ")";
RSQUARE : "]";
GT : ">";
BARRCURLY : "|}";
BARGT : "|>";
BARRSQUARE : "|]";

/** Special compound symbols **/
COLONEQ : ":=";
ARROW  : "->";
DARROW : "=>";
DDARROW : "==>";

BIND	: 'BIND';
VAR		: [a-z];
WS      : ' ' -> skip;
NL      : '\r' -> skip;
NL1		: '\n' -> skip;
