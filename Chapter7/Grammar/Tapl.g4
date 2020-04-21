grammar Tapl;

term	: '(' term ')'						#par
		| VAR								#var
		| term term							#app
		| LAMBDA VAR DOT term				#abs;

VAR		: [a-z];
LAMBDA	: '\\';
DOT		: '.';
