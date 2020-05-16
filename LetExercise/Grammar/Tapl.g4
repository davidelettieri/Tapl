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
		| 'let' VAR '=' term 'in' term		#let 
		| LAMBDA VAR ':' type DOT term		#abs;
type	: type '->' type					#arrow
		| 'Bool'							#bool;

BIND	: 'BIND';
VAR		: [a-z];
LAMBDA	: '\\';
DOT		: '.';
WS      : ' ' -> skip;
NL      : '\r' -> skip;
NL1		: '\n' -> skip;
