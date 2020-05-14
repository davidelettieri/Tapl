grammar Tapl;

term	: '(' term ')'					#par
		| IF term THEN term ELSE term	#IfThenElse
		| SUCC term						#Succ
		| PRED term						#Pred
		| ISZERO term					#IsZero
		| TRUE							#True
		| FALSE							#False
		| ZERO							#Zero;

v : nv			#numericvalue;

nv : SUCC nv | ZERO;


ZERO	: '0';
SUCC	: 'succ';
PRED	: 'pred';
ISZERO	: 'iszero';
TRUE	: 'true';
FALSE	: 'false';
IF		: 'if';
THEN	: 'then';
ELSE	: 'else';
WS      : ' ' -> skip;