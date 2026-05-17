grammar FullUpdate;

toplevel: command SEMI toplevel
		| EOF;

command	: LCURLY UCID COMMA LCID RCURLY EQ term		#command_somebind
		| UCID tybinder								#command_tybinder
		| LCID binder								#command_binder
		| term										#command_term;

binder	: COLON type								#binder_type
		| EQ term									#binder_term;

tybinder: COLONCOLON kind							#tybinder_kn
		| LEQ type									#tybinder_leq
		| tyabbargs EQ type							#tybinder_abbtype
		|											#tybinder_empty;

tyabbargs: UCID okind tyabbargs					#tyabbargs_ucid
		  |											#tyabbargs_empty;

kind	: akind DARROW kind						#kind_darrow
		| akind										#kind_akind;

akind	: STAR										#akind_star
		| LPAREN kind RPAREN						#akind_paren;

okind	: COLONCOLON kind							#okind_kn
		|											#okind_empty;

type	: LAMBDA UCID okind DOT type				#type_abs
		| ALL UCID otype DOT type					#type_all
		| arrowtype									#type_arrowtype;

otype	: LEQ type									#otype_leq
		| COLONCOLON kind							#otype_kn
		|											#otype_empty;

arrowtype: apptype ARROW arrowtype					#arrowtype_arrow
		 | apptype									#arrowtype_atype;

apptype : apptype atype								#apptype_app
		| atype										#apptype_atype;

atype	: LPAREN type RPAREN						#at_type
		| UCID										#at_ucid
		| BOOL										#at_bool
		| USTRING									#at_ustring
		| UUNIT										#at_uunit
		| UFLOAT									#at_ufloat
		| NAT										#at_nat
		| TTOP LSQUARE kind RSQUARE					#at_top_kn
		| TTOP										#at_top
		| LCURLY SOME UCID otype COMMA type RCURLY	#at_some
		| LCURLY fieldtypes RCURLY					#at_record;

fieldtypes	: nefieldtypes							#fieldtypes_ne
			| /* empty */							#fieldtypes_empty;

nefieldtypes: fieldtype COMMA nefieldtypes			#nefieldtypes_cons
			| fieldtype								#nefieldtypes_one;

fieldtype	: HASH LCID COLON type					#fieldtype_hash_lcid
			| HASH type								#fieldtype_hash_type
			| LCID COLON type						#fieldtype_lcid
			| type									#fieldtype_type;

term	: LAMBDA UCID otype DOT term				#term_tabs
		| LAMBDA LCID COLON type DOT term			#term_llcid
		| LAMBDA USCORE COLON type DOT term			#term_luc
		| LET LCURLY UCID COMMA LCID RCURLY EQ term IN term	#term_unpack
		| LET LCID EQ term IN term					#term_ll
		| LET USCORE EQ term IN term				#term_lu
		| LETREC LCID COLON type EQ term IN term	#term_letrec
		| IF term THEN term ELSE term				#term_ift
		| appterm LEFTARROW LCID EQ term			#term_update
		| appterm									#term_appterm;

appterm : appterm LSQUARE type RSQUARE				#appterm_tapp
		| appterm pathterm							#appterm_app
		| FIX pathterm								#appterm_fix
		| TIMESFLOAT pathterm pathterm				#appterm_times
		| SUCC pathterm								#appterm_succ
		| PRED pathterm								#appterm_pred
		| ISZERO pathterm							#appterm_iszero
		| pathterm									#appterm_path;

ascribeterm: aterm AS type							#ascribeterm_aaa
		   | aterm									#ascribeterm_a;

pathterm: pathterm DOT LCID						#pathterm_lcid
		| pathterm DOT INTV							#pathterm_intv
		| ascribeterm								#pathterm_asterm;

termseq : term SEMI termseq						#termseq_seq
		| term										#termseq_term;

aterm	: LPAREN termseq RPAREN					#aterm_paren
		| INERT LSQUARE type RSQUARE				#aterm_inert
		| TRUE										#aterm_true
		| FALSE										#aterm_false
		| LCURLY STAR type COMMA term RCURLY AS type	#aterm_pack
		| LCURLY fields RCURLY					#aterm_fields
		| LCID										#aterm_lcid
		| STRINGV									#aterm_stringv
		| UNIT										#aterm_unit
		| FLOATV									#aterm_floatv
		| INTV										#aterm_intv;

fields	: nefields								#fields_ne
		| /* empty */							#fields_empty;

nefields: field COMMA nefields					#nefields_cons
		| field									#nefields_one;

field	: HASH LCID EQ term						#field_hash_lcid
		| HASH term								#field_hash_term
		| LCID EQ term							#field_lcid
		| term									#field_term;


/** Keywords **/

TYPE		: 'type';
INERT		: 'inert';
IF			: 'if';
THEN		: 'then';
ELSE		: 'else';
TRUE		: 'true';
FALSE		: 'false';
BOOL		: 'Bool';
AS			: 'as';
LAMBDA		: 'lambda';
LET			: 'let';
IN			: 'in';
FIX			: 'fix';
LETREC		: 'letrec';
ALL			: 'All';
SOME		: 'Some';
USTRING		: 'String';
UNIT		: 'unit';
UUNIT		: 'Unit';
TIMESFLOAT	: 'timesfloat';
UFLOAT		: 'Float';
SUCC		: 'succ';
PRED		: 'pred';
ISZERO		: 'iszero';
NAT			: 'Nat';
TTOP		: 'Top';

STRINGV		: '"' .+? '"';

/** Symbols **/
USCORE		: '_';
APOSTROPHE	: '\'';
DQUOTE		: '"';
BANG		: '!';
HASH		: '#';
TRIANGLE	: '$';
STAR		: '*';
VBAR		: '|';
DOT			: '.';
SEMI		: ';';
COMMA		: ',';
SLASH		: '/';
COLON		: ':';
COLONCOLON	: '::';
EQ			: '=';
EQEQ		: '==';
LSQUARE		: '[';
LT			: '<';
LCURLY		: '{';
LPAREN		: '(';
LEFTARROW	: '<-';
LEQ			: '<:';
LCURLYBAR	: '{|';
LSQUAREBAR	: '[|';
RCURLY		: '}';
RPAREN		: ')';
RSQUARE		: ']';
GT			: '>';
BARRCURLY	: '|}';
BARGT		: '|>';
BARRSQUARE	: '|]';

/** Special compound symbols **/
COLONEQ		: ':=';
ARROW		: '->';
DARROW		: '=>';
DDARROW		: '==>';

FLOATV	: [0-9].[0-9]+;
INTV	: [0-9]+;
UCID	: [A-Z][a-zA-Z0-9_']*;
LCID	: [a-z][a-zA-Z0-9_']*;

WS		: [ \t\r\n]+ -> skip;
COMMENT	: '/*' .*? '*/' -> skip;
