# Implementation Map

Use this reference to choose the right C# template and to find the concrete anchors that already define the repo's implementation style.

## Primary Template Choice

Use `FullSimple/` when the target language has any of these traits:
- multiple syntax families or many AST node types
- explicit type syntax and nontrivial typing rules
- multiple visitor classes
- several binding kinds
- records, variants, references, recursive types, or advanced type operators

Use a smaller template only when the target language is genuinely smaller:
- `SimpleBool/` for small typed lambda-calculus variants
- `LetExercise/` when the grammar and top-level shape are closer to let-oriented examples
- `Untyped/` for untyped lambda-calculus variants
- `Arith/` only for minimal arithmetic-style languages

## Concrete Repo Anchors

### Complex typed language baseline

- `FullSimple/Functions.cs`: parse, process, and binding-check flow
- `FullSimple/Core/`: evaluation, typing, shifting, substitution
- `FullSimple/Syntax/`: terms, types, bindings, printing, context extensions
- `FullSimple/Visitors/`: top-level visitor set for richer grammars
- `FullSimple.Tests/ShiftingTests.cs`: regression-style traversal test pattern

### Smaller language baselines

- `SimpleBool/`: compact typed-language structure
- `LetExercise/`: simpler top-level visitor structure
- `Untyped/`: untyped lambda-calculus structure and tests
- `Arith/`: minimal single-visitor and term-only style

## Public API Pattern To Preserve

Match the existing C# entrypoint pattern:
- `Parse(string)` returns a function that consumes `Context`
- `ProcessCommand(Context, ICommand)` handles evaluation and bindings
- `Process(string)` delegates to `CommandRunner.Process`

Do not invent a new public entry model for the new language unless the repo already requires it.

## Shared-Layer Guidance

Before changing `Common/`, decide whether the need is:
- a language-local concern that should stay in the new project, or
- a truly reusable abstraction already duplicated elsewhere

If the `Common/` change could affect existing languages, stop and ask the user before editing it.

## Solution and Project Wiring

Adding a new language usually requires new project references in more than one place:
- `Tapl.slnx`
- `Harness.Runner/Harness.Runner.csproj`
- `Demo/Demo.csproj`

Mirror the style of existing project references rather than introducing new build conventions.