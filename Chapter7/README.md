# An ML implementation of the Lambda Calculus.

Implementation from Pierce https://www.cis.upenn.edu/~bcpierce/tapl/checkers/untyped.tar.gz

The book is missing quite a look with respect to the actual implementation. 

In the OCAML implementation the parser in producing what in C# could be defined as ```Func<Context, (ImmutableStack<ICommand>, Context)>```.

The `ICommand` has two different implementation
- Bind: this is used to add a variable name to a naming context
- Eval: this is used to evaluate a Term with respect to a naming context

The naming context is required to handle terms with free variables and the bind command is used to add names to the context. 

What's strange is how the parser is defined and used, as end result we want to evaluate some terms (at least that's what I want) but we end up with commands and context and we don't use some of the context we build. This comments are obscure but let's follow through the test file in the untyped implementation to understand what is done. Please note that I changed the bind syntax and the lambda syntax in
- BIND x -> binds the name 'x'
- \x.[body] -> is a lambda abstraction

Here the test file content:

```
BIND x;
x;
\x. x;
(\x. x) (\x. x x); 
```

Notice that we have 4 commands: 
1. BIND
2. EVAL of a variable
3. EVAL of lambda abstraction
4. EVAL of application

Since the parser produces a `Func<Context, (ImmutableStack<ICommand>, Context)>`, we need to provide a Context in order to retrieve the list of commands. Let's just pick the empty naming context, which is a reasonable choice at the beginning of the execution. Now we end up with a `(ImmutableStack<ICommand>, Context)>` so a tuple consisting of a list of commands and a context. What's next? **We ignore the context** and fold (aggregate in C#/Linq world) the list of commands. 

The folding functions is `ProcessCommand` defined as
```
public static Context ProcessCommand(Context ctx, ICommand c)
{
    switch (c)
    {
        case Eval e:
            var t = Eval(ctx, e.Term);
            Console.WriteLine(PrintTerm(ctx, t));
            return ctx;
        case Bind b:
            Console.WriteLine($"Bind {b.Name}");
            return ctx.AddBinding(b.Name, new Binding());
        default:
            throw new InvalidOperationException();
    }
}
```
As seen in the definition we need again a context to start the computation and we use, again, the empty context. I still have to understand:
- why do we need a Context to get the list of commands.
- why do we produce a Context together with the list of commands and not only the commands considering that we discard that context.
- why has been decided that parsing something like 'a b' is not possible. Parsing is strictly related to the context that is built during the parse itself. Since we are using nameless terms we cannot parse terms with free variables without a naming context.