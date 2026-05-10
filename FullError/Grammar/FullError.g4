grammar FullError;

toplevel: command SEMI toplevel
		| EOF;
command	: term										#command_term
		| UCID tybinder								#command_tybinder
		| LCID binder								#command_binder;
binder: COLON type									#binder_type
		| EQ term									#binder_term;
type: arrowtype										#type_arrowtype;
atype: LPAREN type RPAREN							#at_type
	 | TBOT											#at_bot
	 | TTOP											#at_top
	 | BOOL											#at_bool
	 | UCID											#at_ucid;
arrowtype: atype ARROW arrowtype					#arrowtype_arrow
		 | atype									#arrowtype_atype;
term	: appterm									#term_appterm
		| IF term THEN term ELSE term				#term_ift
		| LAMBDA LCID COLON type DOT term			#term_llcid
		| LAMBDA USCORE COLON type DOT term			#term_luc
		| TRY term OTHERWISE term					#term_try;
appterm : aterm										#appterm_aterm
		| appterm aterm								#appterm_app;
aterm : LPAREN term RPAREN							#aterm_paren
      | LCID 										#aterm_lcid
      | TRUE										#aterm_true
      | FALSE										#aterm_false
      | ERROR										#aterm_error;
tybinder:											#tybinder_empty
		| EQ type									#tybinder_type;



/** Keywords **/

TBOT	: 'Bot';
TTOP	: 'Top';
IF		: 'if';
THEN	: 'then';
ELSE	: 'else';
TRUE	: 'true';
FALSE	: 'false';
BOOL	: 'Bool';
LAMBDA	: 'lambda';
ERROR	: 'error';
TRY		: 'try';
OTHERWISE: 'with';
TYPE	: 'type';

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
UCID : [A-Z][a-zA-Z0-9]*;
LCID : [a-z][a-zA-Z0-9]*;

WS      : ' ' -> skip;
TAB      : '\t' -> skip;
NL      : '\r' -> skip;
NL1		: '\n' -> skip;
COMMENT : '/*' .*? '*/' -> skip;
