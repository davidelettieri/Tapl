# FullSimple Alignment And Fix Plan

## Goal

Align the C# `FullSimple` implementation with the OCaml reference implementation in `TAPL/fullsimple`, fix the confirmed semantic and runtime issues, and leave the C# port with enough regression coverage to prevent future drift.

This plan is ordered to minimize rework:

1. Fix infrastructure that blocks faithful execution flow.
2. Add tests before each semantic repair so changes are falsifiable.
3. Repair typechecker soundness issues before printer or parser polish.
4. Close user-visible runtime gaps.
5. Decide explicitly which remaining differences from OCaml are intentional.

## Reference Surfaces

Use these files as the primary ground truth during implementation.

### OCaml reference

- `TAPL/fullsimple/lib/core.ml`
- `TAPL/fullsimple/lib/syntax.ml`
- `TAPL/fullsimple/lib/parser.mly`
- `TAPL/fullsimple/bin/main.ml`

### C# implementation

- `Tapl/Common/CommandRunner.cs`
- `Tapl/Common/TopLevelCommandComposer.cs`
- `Tapl/FullSimple/Core/Evaluation.cs`
- `Tapl/FullSimple/Core/Typing.cs`
- `Tapl/FullSimple/Core/Shifting.cs`
- `Tapl/FullSimple/Core/Substitution.cs`
- `Tapl/FullSimple/Syntax/Printing.cs`
- `Tapl/FullSimple/Syntax/ContextExtensions.cs`
- `Tapl/FullSimple/Functions.cs`
- `Tapl/FullSimple/Grammar/FullSimple.g4`
- `Tapl/FullSimple/Visitors/*.cs`
- `Tapl/FullSimple.Tests/*`
- `Tapl/Demo/fullsimple.txt`

## Confirmed Issues To Address

These are the issues already identified and should drive the initial test matrix.

1. `CommandRunner.Process` always starts from a fresh context and cannot support incremental processing.
2. `Typing.TypeOf` does not enforce same-result-type checking across `case` branches.
3. `Typing.TypeEqual` compares records incorrectly by position and ignores record labels.
4. `Typing.TypeEqual` uses the wrong field from `TypeVar` when checking type abbreviations.
5. `Typing.TypeOf` is missing the `Inert` case.
6. `Printing.PrintTerm` is missing several valid term forms.
7. `Printing.PrintType` is missing at least `TypeFloat` and has malformed variant formatting.
8. Grammar and command-surface parity with OCaml is incomplete, especially around `import` and bare type-variable binders.
9. FullSimple test coverage is far too narrow to protect semantics.

## Working Principles

Apply these rules while implementing the fixes.

1. Keep changes behavior-focused and small.
2. Add or update focused tests before or together with each fix.
3. Validate after each small step instead of landing a large semantic batch.
4. Prefer parity with OCaml unless there is a documented reason not to.
5. If a behavior stays different by choice, document it explicitly in `FullSimple/README.md`.

## Phase 0: Prepare A Regression Matrix

### Objective

Create a concrete checklist of programs and expected outcomes before changing code.

### Work

1. Extract a curated set of representative programs from `Tapl/Demo/fullsimple.txt`.
2. Split them into categories:
   - should parse and evaluate successfully
   - should parse and typecheck successfully
   - should fail typechecking
   - should print a specific normal form and type
3. Create a temporary table in a developer note or directly in test cases mapping:
   - source snippet
   - expected final term form
   - expected final type
   - expected failure message category if applicable
4. Include targeted snippets for:
   - records with reordered labels
   - variant `case` branches with different result types
   - `inert`
   - `letrec`
   - `fix`
   - type abbreviations
   - term abbreviations
   - floats and `timesfloat`

### Deliverable

A small set of known-good examples that can be turned into automated tests.

## Phase 1: Generalize CommandRunner.Process

### Objective

Bring command execution flow closer to the OCaml driver so later features are not built on a too-narrow API.

### Current Problem

`Common/CommandRunner.cs` hardcodes `new Context()` for both parse-time and execution-time context. That only supports closed one-shot processing and prevents alignment with the OCaml driver's context-threading behavior.

### Target Behavior

Support both of these modes:

1. Process from an explicit initial context.
2. Process from an empty context for convenience.

### Implementation Steps

1. Add an overload or new primary signature to `CommandRunner.Process` that accepts an initial `Context`.
2. Use that incoming context for both:
   - parser function application
   - command execution loop initialization
3. Preserve the current simple call site by keeping a convenience wrapper that passes `new Context()`.
4. Confirm command order still matches source order.
5. Review `TopLevelCommandComposer.Compose` and current immutable stack traversal to ensure no order regression is introduced.
6. Update `FullSimple/Functions.cs` only as needed to call the new overload or keep the old wrapper.

### Tests

Add tests in `FullSimple.Tests` or a new `Common.Tests` area covering:

1. Processing from an empty context.
2. Processing from a pre-populated context.
3. Sequential commands that depend on previous bindings.
4. Preservation of source command order.

### Validation

Run the narrow test file for the runner changes.

## Phase 2: Build A FullSimple Behavior Test Harness

### Objective

Give `FullSimple` real behavior coverage before changing semantics.

### Work

1. Add a new test file such as `FullSimpleBehaviorTests.cs`.
2. Create small helper methods for:
   - parsing source into commands
   - processing commands and capturing final context
   - calling `Typing.TypeOf` directly for hand-built terms where useful
   - asserting thrown exception types or messages
3. Keep tests focused and independent.
4. Prefer direct construction of ASTs for narrow semantics tests when parser details are irrelevant.
5. Use parser-driven tests for integration scenarios and grammar-sensitive behaviors.

### Minimum Test Set

1. `case` with same-result-type branches should typecheck.
2. `case` with different-result-type branches should fail.
3. Record equality with same labels and same types should succeed.
4. Record equality with different labels but same field types should fail.
5. Record equality should not depend on label order if the OCaml semantics treat the record type structurally by labels.
6. `inert[Bool]` should type as `Bool`.
7. `timesfloat` should type as `Float` and print correctly.
8. A valid variant/tag/case example should parse, typecheck, evaluate, and print.
9. A valid `letrec` example should parse, typecheck, and evaluate.
10. Top-level term abbreviation lookup should evaluate as in OCaml.

### Validation

Run only the new FullSimple behavior tests first.

## Phase 3: Fix Typechecker Soundness

### Objective

Repair the highest-impact semantic bugs in `FullSimple/Core/Typing.cs`.

### Step 3.1: Restore `case` branch agreement checking

#### Problem

The C# implementation computes all branch result types but does not enforce that they are equal.

#### Implementation

1. Re-enable the check in `TypeOfVariant`.
2. Ensure the first branch type is used as the comparison baseline.
3. Throw a consistent exception when branch result types differ.
4. Match the OCaml behavior as closely as possible.

#### Tests

1. Two branches both returning `Bool` should succeed.
2. One branch returning `Bool` and one returning `Nat` should fail.

### Step 3.2: Fix record type equality

#### Problem

The current implementation zips field sequences and ignores labels. That is not aligned with the OCaml record-type comparison logic.

#### Implementation

1. Replace positional comparison with label-aware comparison.
2. First check field count.
3. For each field in the right-hand type, look up the same label in the left-hand type.
4. Recursively compare field types using `TypeEqual`.
5. Ensure missing labels return false rather than crash.

#### Tests

1. Same labels and same types should be equal.
2. Same types but different labels should not be equal.
3. Same labels in different order should be equal if the OCaml logic allows order-insensitive structural equality.

### Step 3.3: Fix type abbreviation lookup for `TypeVar`

#### Problem

The current code checks `TypeVar.N`, which is context length, not the de Bruijn index.

#### Implementation

1. Audit all `TypeVar` handling in `Typing.cs`.
2. Use the index field consistently when checking and resolving type abbreviations.
3. Confirm this matches the OCaml `TyVar(i,_)` logic.
4. Re-test any type-abbreviation scenarios after the change.

#### Tests

1. A type abbreviation bound in context should resolve correctly.
2. Nested contexts should still resolve the intended abbreviation.

### Step 3.4: Add `Inert` typing

#### Problem

`Inert` is produced by the grammar and visitors but rejected by the typechecker.

#### Implementation

1. Add a `case Inert inert:` path in `TypeOf`.
2. Return the annotated type directly.
3. Confirm no extra simplification is required beyond reference behavior.

#### Tests

1. `inert[Bool]` should type as `Bool`.
2. `inert[{x:Bool}]` should type as the annotated record type.

### Validation For Phase 3

Run the targeted typing tests after each sub-step, then run the whole `FullSimple.Tests` project.

## Phase 4: Verify Evaluation Semantics And Repair If Needed

### Objective

Confirm the evaluator is aligned where the typechecker now allows programs through, and fix any exposed mismatches.

### Areas To Inspect

1. `Var` lookup for term abbreviations.
2. `Tag` and `Case` evaluation.
3. Record field evaluation order.
4. Projection from records.
5. `Fix` on abstractions.
6. Numeric evaluation rules.
7. `Ascribe` elimination on values.

### Implementation Steps

1. Add evaluation tests before changing evaluator code.
2. Compare each evaluator rule in `Evaluation.cs` with the corresponding OCaml rule.
3. If a mismatch is found, fix one rule at a time.
4. Re-run only the tests tied to that rule after each change.

### Suggested Tests

1. Top-level bound term abbreviation evaluates when referenced.
2. `case <label=value> as Variant of ...` substitutes correctly.
3. Record fields evaluate left to right one field at a time.
4. `fix` unfolds only for abstractions.
5. `pred 0`, `pred (succ nv)`, and `iszero` behave like OCaml.

### Validation

Run FullSimple behavior tests plus any focused evaluator tests.

## Phase 5: Repair Printing So Valid Programs Complete End To End

### Objective

Remove user-visible crashes and malformed output from `FullSimple/Syntax/Printing.cs`.

### Current Problems

The printer is missing support for some valid AST nodes and has at least one malformed output path for variant types.

### Implementation Steps

1. Audit every `ITerm` implementation under `FullSimple/Syntax/Terms` against the switch in `PrintTerm`.
2. Add missing term cases, especially:
   - `Tag`
   - `Let` if it can still appear in printed output
   - `Fix`
   - `TimesFloat`
   - `IsZero`
   - `Inert`
3. Audit every `IType` implementation against the switch in `PrintType`.
4. Add missing `TypeFloat` handling.
5. Fix nested `PrintType` calls in variant and record formatting so labels do not produce duplicate colons.
6. Decide whether printed syntax should aim for exact OCaml pretty-printer parity or just readable equivalent syntax.
7. Keep formatting changes consistent across terms and types.

### Tests

1. Each term constructor should have at least one print test.
2. Each type constructor should have at least one print test.
3. End-to-end processing of representative demo snippets should not throw in the print phase.

### Validation

Run print-focused tests and then end-to-end FullSimple behavior tests.

## Phase 6: Close Grammar And Command-Surface Gaps

### Objective

Decide whether the C# grammar should fully match the OCaml parser, then implement or document the remaining differences.

### Gap 6.1: `import`

#### Current State

The OCaml parser supports `import`; the C# grammar does not.

#### Decision Point

Choose one:

1. Implement `import` support now.
2. Defer it and document that the C# port intentionally does not support imports yet.

#### If Implemented

1. Extend `FullSimple.g4` with an `import` command.
2. Extend the command visitor and command model as needed.
3. Use the generalized `CommandRunner.Process` as the execution base.
4. Decide how file loading and path resolution should work in the C# repo.

### Gap 6.2: bare type binders / `TyVarBind`

#### Current State

The OCaml parser supports an empty `TyBinder` alternative producing `TyVarBind`; the C# grammar only supports `= type`.

#### Decision Point

Choose one:

1. Implement the empty `TyBinder` alternative.
2. Keep it unsupported and document it.

#### If Implemented

1. Update grammar.
2. Update `TyBinderVisitor`.
3. Add tests for uppercase type-variable declarations without abbreviations.

### Gap 6.3: REPL-style continuation

#### Current State

The generalized command runner makes this possible structurally but does not by itself create a REPL.

#### Decision Point

Decide whether REPL-like incremental processing is a target for this project or just a beneficial architectural alignment.

### Validation

Add tests only for whichever grammar-level features are intentionally supported.

## Phase 7: Review Context, Shifting, And Substitution For Final Parity

### Objective

Do a final consistency pass on the de Bruijn plumbing after semantic fixes are in place.

### Work

1. Revisit `ContextExtensions.GetBinding` and `GetTypeFromContext`.
2. Compare `BindingShift`, `TypeShift`, and `TermShift` logic against the OCaml `syntax.ml` reference.
3. Confirm substitution helpers are still correct under the new tests.
4. Add targeted tests if any subtle mismatch is discovered.

### Why This Comes Late

These areas are lower-risk than the confirmed `Typing.cs` issues and already have at least some test coverage. They should be reviewed after the higher-confidence defects are repaired.

## Phase 8: Documentation And Final Cleanup

### Objective

Make the outcome explicit for future work.

### Work

1. Update `FullSimple/README.md` with:
   - implemented features
   - any intentional deviations from OCaml
   - how to run the relevant tests
2. Add brief notes near complex parity-sensitive logic only where needed.
3. Remove dead code or commented-out logic left behind by the fixes.

## Suggested Execution Order

Implement in this order unless a focused test exposes a dependency.

1. Phase 1: generalize `CommandRunner.Process`
2. Phase 2: build behavior tests
3. Phase 3.1: `case` branch type agreement
4. Phase 3.2: record type equality
5. Phase 3.3: type abbreviation lookup
6. Phase 3.4: `Inert` typing
7. Phase 4: evaluator confirmation and repairs
8. Phase 5: printer coverage and formatting fixes
9. Phase 6: grammar and command-surface parity decisions
10. Phase 7: context and shifting audit
11. Phase 8: documentation cleanup

## Test Strategy Summary

Keep the test pyramid shallow and behavior-focused.

### Narrow unit tests

- `Typing.TypeEqual`
- `Typing.TypeOf`
- `Evaluation.Eval`
- `Printing.PrintTerm`
- `Printing.PrintType`
- `CommandRunner.Process`

### Integration tests

- parse + process + final context
- parse + type + eval + print
- selected scenarios from `Demo/fullsimple.txt`

### Regression cases that must exist

1. Mismatched `case` branch result types
2. Record label mismatch with same field types
3. Valid record comparison with reordered labels
4. `inert[Type]`
5. A float expression using `timesfloat`
6. A variant/tag/case round-trip example
7. A `letrec` example
8. A top-level abbreviation lookup example

## Acceptance Criteria

The work is complete when all of the following are true.

1. `CommandRunner.Process` can execute from a supplied initial context.
2. `FullSimple` has behavior tests covering typing, evaluation, and printing of its main features.
3. `case` expressions with mismatched branch result types are rejected.
4. Record type equality respects labels and matches OCaml semantics.
5. Type abbreviations resolve using the correct de Bruijn index logic.
6. `Inert` terms typecheck correctly.
7. Valid FullSimple terms and types no longer fail in the printer due to missing switch cases.
8. Any unsupported OCaml features are documented intentionally.

## Practical Implementation Notes

1. Do not change parser, typechecker, evaluator, and printer all at once.
2. When a test exposes a bug in multiple layers, fix the most local controlling layer first.
3. Prefer adding one regression test per bug before broader refactors.
4. Avoid broad cleanup during semantic fixes.
5. Keep OCaml parity checks local and explicit by comparing one rule at a time.

## Recommended First Commit Sequence

If you want to make progress incrementally, this sequence minimizes risk.

1. Generalize `CommandRunner.Process` and add runner tests.
2. Add FullSimple behavior tests for `case`, records, and `inert`.
3. Fix `Typing.cs` issues one by one.
4. Add printer coverage tests.
5. Fix printer crashes.
6. Decide and implement or document grammar gaps.
