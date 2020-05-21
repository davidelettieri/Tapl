grammar FullSimple;

toplevel: command SEMI toplevel | EOF		;
command	: term						
		| UCID tybinder				
		| LCID binder				;
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
term	:  appterm									#term_appterm
		| IF term THEN term ELSE term				#term_ift
		| CASE term OF cases						#term_caseOf
		| LAMBDA LCID COLON type DOT term			#term_llcid
		| LAMBDA USCORE COLON type DOT term			#term_luc
		| LET LCID EQ term IN term					#term_ll
		| LET USCORE EQ term IN term				#term_lu
		| LETREC LCID COLON type EQ term IN term	#term_letrec;
appterm : pathterm									#appterm_path
		| appterm pathterm							#appterm_app_path
		| FIX pathterm								#appterm_fix
		| TIMESFLOAT pathterm pathterm				#appterm_times
		| SUCC pathterm								#appterm_succ
		| PRED pathterm								#appterm_pred
		| ISZERO pathterm							#appterm_iszero;
ascribeterm: aterm AS type							#ascribeterm_aaa
		   | aterm									#ascribeterm_a;		
pathterm : pathterm DOT LCID						#pathterm_lcid
		 | pathterm DOT INTV						#pathterm_intv
		 | ascribeterm								#pathterm_asterm;
termseq: term										#termseq_term
	   | term SEMI termseq							#termseq_termseq;
aterm : LPAREN termseq RPAREN						#aterm_paren
      | INERT LSQUARE type RSQUARE					#aterm_inert
      | TRUE										#aterm_true
      | FALSE										#aterm_false
      | LT LCID EQ term GT AS type					#aterm_lt
      | LCID										#aterm_lcid
      | STRINGV										#aterm_stringv
      | UNIT										#aterm_unit
      | LCURLY fields RCURLY						#aterm_fields
      | FLOATV										#aterm_floatv
      | INTV										#aterm_intv;
cases : case
      | case VBAR cases;
case : LT LCID EQ LCID GT DDARROW appterm;
fields : nefields?;
nefields : field									#nefields_field
         | field COMMA nefields						#nefields_field_comma_nefields;
field : LCID EQ term								#field_lcid
      | term										#field_term;


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
