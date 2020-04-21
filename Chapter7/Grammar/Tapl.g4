grammar Tapl;

term	: '(' term ')'						#par
		| VAR								#var
		| <assoc=right> LAMBDA VAR DOT term	#abs
		| term term							#app;

VAR		: [a-z];
LAMBDA	: '\\';
DOT		: '.';
