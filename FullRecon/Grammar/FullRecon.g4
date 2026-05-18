grammar FullRecon;

toplevel: command SEMI toplevel
		| EOF;
command	: term										#command_term
		| LCID binder								#command_binder;
binder: COLON type									#binder_type;
type: arrowtype										#type_arrowtype;
atype: LPAREN type RPAREN							#at_type
	 | UCID											#at_ucid
	 | BOOL											#at_bool
	 | NAT											#at_nat;
arrowtype: atype ARROW arrowtype					#arrowtype_arrow
		 | atype									#arrowtype_atype;
term	: appterm									#term_appterm
		| IF term THEN term ELSE term				#term_ift
		| LET LCID EQ term IN term					#term_ll
		| LET USCORE EQ term IN term				#term_lu
		| LAMBDA LCID DOT term						#term_lambda_untyped
		| LAMBDA LCID COLON type DOT term			#term_lambda_typed
		| LAMBDA USCORE COLON type DOT term			#term_lambda_uscore;
appterm : aterm										#appterm_aterm
		| appterm aterm								#appterm_app
		| SUCC aterm								#appterm_succ
		| PRED aterm								#appterm_pred
		| ISZERO aterm								#appterm_iszero;
aterm : LPAREN term RPAREN							#aterm_paren
      | LCID 										#aterm_lcid
      | TRUE										#aterm_true
      | FALSE										#aterm_false
      | INTV										#aterm_intv;



/** Keywords **/

IF		: 'if';
THEN	: 'then';
ELSE	: 'else';
TRUE	: 'true';
FALSE	: 'false';
BOOL	: 'Bool';
LAMBDA	: 'lambda';
LET		: 'let';
IN		: 'in';
SUCC	: 'succ';
PRED	: 'pred';
ISZERO	: 'iszero';
NAT		: 'Nat';

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

INTV	: [0-9]+;
UCID : [A-Z][a-zA-Z0-9_']*;
LCID : [a-z][a-zA-Z0-9_']*;

WS      : ' ' -> skip;
TAB      : '\t' -> skip;
NL      : '\r' -> skip;
NL1		: '\n' -> skip;
COMMENT : '/*' .*? '*/' -> skip;
