---
name: tapl-port-language
description: 'Port one language from TAPL-ocaml into a new C# implementation with fixtures, harness parity, Demo support, and repo integration. Use when adding a missing TAPL language, matching OCaml behavior, updating scripts and runner wiring, and finishing only with green harness results.'
argument-hint: '<language-name>'
user-invocable: true
disable-model-invocation: false
---

# TAPL Port Language

Use this skill when you need to add one new C# TAPL language implementation from the OCaml reference in `TAPL-ocaml/<language>`.

This skill is for end-to-end delivery, not design-only work. The output must be:
- a new C# language project
- a new C# unit test project
- a new fixture set under `fixtures/<language>`
- full repo wiring updates for grammar generation, harness execution, Demo support, and solution membership
- a green focused harness run for the new language

## Required Inputs

- Target language name, matching a folder under `TAPL-ocaml/`
- Whether the target already has any partial C# implementation in the repo
- Any known language-specific constraints from the user

If the target language is not named, ask once for the exact folder name.

## Hard Guardrails

- Do not modify files under `TAPL-ocaml/`.
- Do not alter an existing C# language implementation except for shared registration surfaces needed to add the new language.
- Do not reuse or rename an existing language folder.
- Do not change semantics away from observed OCaml behavior unless the user explicitly approves a deviation.
- If a required change in `Common/` could break an existing language, stop and ask for confirmation before editing it.
- If the target language already exists in C#, stop and ask whether to extend it or treat the task as a bug-fix task instead.

## Execution Workflow

1. Confirm the target and choose the nearest template.
Inspect `TAPL-ocaml/<language>` and compare it to the current C# implementations.

Decision rule:
- Use `FullSimple/` as the default template for typed or structurally rich languages: multiple visitors, complex bindings, records, variants, references, type abbreviations, subtyping, recursive types, or several syntax families.
- Use `SimpleBool/`, `LetExercise/`, `Untyped/`, or `Arith/` only when the language is materially smaller and that choice reduces complexity without hiding required behavior.

2. Build a parity map before editing.
Write down the OCaml-to-C# mapping for:
- grammar entry points
- term shapes
- type shapes
- bindings and commands
- evaluation rules
- typing rules
- shifting and substitution helpers
- printing behavior
- top-level command processing

Highlight any suspected OCaml bug separately, but keep the port behavior aligned with OCaml unless the user approves otherwise.

3. Create the new language project.
Add a new project folder named after the target language. Mirror the chosen template's structure closely.

Expected output usually includes:
- `AssemblyInfo.cs`
- `Functions.cs`
- `Grammar/`
- `Core/`
- `Syntax/`
- `Visitors/` when required by grammar complexity

Public API expectations:
- expose the parse and process entrypoints in `Functions.cs`
- preserve the existing repo pattern of `Parse`, `ProcessCommand`, and `Process`
- keep the top-level flow compatible with `CommandRunner.Process`

4. Add the test project.
Create `<Language>.Tests/` as a separate project and add focused unit tests for the most failure-prone logic first.

Minimum expectations:
- parser coverage for representative surface forms
- evaluator coverage for key reduction paths
- typing coverage for typed languages
- at least one regression-style test for shifting, substitution, visitors, or another traversal-sensitive operation when the language uses de Bruijn indices or rich syntax

5. Add fixtures for harness parity.
Create `fixtures/<language>/` and add a representative fixture suite.

Fixture expectations:
- follow the `fixtures/fullsimple/` pattern for value-producing and well-typed term coverage
- for each supported non-error term form, include both an unevaluated printing case and an evaluated result case in the same fixture when possible
- prefer the exact two-command shape: first `lambda _:Unit. <term>;` and then `<term>;`
- use the first line to verify printing and typing of the unevaluated term, and the second line to verify evaluation and printing of the result
- apply this pattern broadly enough that every term constructor and important derived form is exercised at least once through the harness
- cover successful evaluation paths
- cover type errors or runtime errors when the language has them
- cover syntax that stresses binding depth, substitution, and printing. Cover all terms defined by the language 
- keep fixtures small and diagnostic
- prefer multiple focused fixtures over a single large script

Use `fixtures/fullsimple/` as the default harness-fixture example. Error fixtures may remain single-purpose when the expression is intended to fail before both lines can run.

6. Update every required integration surface.
Treat this as mandatory and exhaustive. The repo uses duplicated language lists and project references.

Always inspect and update the relevant files from `./references/integration-checklist.md`.

At minimum, this usually means:
- add the new projects to `Tapl.slnx`
- add grammar generation to both `gen-all.sh` and `scripts/gen-all.sh`
- add project references and dispatch logic in `Harness.Runner/`
- add the language to `scripts/compare-tapl.sh`
- add Demo wiring, project reference, and sample source file support in `Demo/`

7. Validate locally and iterate narrowly.
Use the focused validation sequence from `./references/validation-matrix.md`.

Required completion gate:
- the new language builds
- the new unit tests pass
- the focused TAPL harness comparison for the new language is green

If a validation fails, repair the same local slice first. Do not widen scope until the focused check passes or the failure proves the wrong abstraction was chosen.

8. Report the result.
Summarize:
- what was added
- which template was used and why
- every repo integration surface touched
- harness result and unit-test result
- any remaining parity gaps
- any suspected OCaml bugs, clearly separated from C# defects

## Completion Criteria

Do not consider the task done until all of the following are true:
- a new C# language project exists
- a new unit test project exists
- fixtures exist under `fixtures/<language>`
- Demo support exists for the new language
- all mandatory registration surfaces are updated
- focused harness comparison is green for the new language
- suspected OCaml bugs are documented separately when present

## References

- Template and file map: `./references/implementation-map.md`
- Mandatory repo touchpoints: `./references/integration-checklist.md`
- Validation sequence and finish gate: `./references/validation-matrix.md`

## Example Prompts

- `/tapl-port-language tyarith`
- `Use tapl-port-language to port tyarith from TAPL-ocaml into a new C# project with harness parity.`
- `Port fullerror from TAPL-ocaml into C# using the tapl-port-language skill. Preserve OCaml behavior, add fixtures, update scripts, and stop only for risky Common changes.`