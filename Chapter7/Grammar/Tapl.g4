grammar Tapl;

toplevel: command ';' toplevel | EOF	;
command	: (bind|term)			;
bind	: BIND VAR				;
term	: '(' term ')'			#par
		| VAR					#var
		| term term				#app
		| LAMBDA VAR DOT term	#abs;

BIND	: 'BIND';
VAR		: [a-z];
LAMBDA	: '\\';
DOT		: '.';
WS      : ' ' -> skip;
NL      : '\r' -> skip;
NL1		: '\n' -> skip;
