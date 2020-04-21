# Tapl
I'm trying to implement the snippets from all the chapters "An ML implementation of *" using C#. Each chapter will have 2 project, one for the implementation, one for the tests. 

The OCaml implementation can be found at the book website https://www.cis.upenn.edu/~bcpierce/tapl/, I'm not strictly following that implementation since I'm looking at the code inside the book, moreover I'll use ANTLR to parse the text.

I'm not sure how I will handle code repetition, since chapters build one upon another, so I will probably change the structure of the solution as needed. 

So far I'm ignoring the info about the position in the source file/string where the node is been generated. In the original implementation the term type is a discriminated union type with each cases representing a different term such as lambda abstraction or application.

