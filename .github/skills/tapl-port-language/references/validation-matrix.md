# Validation Matrix

Use the narrowest validation that can falsify the current change. Do not finish on build success alone.

## Required Validation Order

1. Grammar generation when the language adds or changes ANTLR outputs.
Run the repo's grammar generation path and confirm generated files are present.

2. Focused build.
Build the new language project, the new test project, `Harness.Runner`, and `Demo` if it was updated.

3. Focused unit tests.
Run the new test project first. Add targeted regression tests for any bug found during implementation.

4. Focused harness comparison.
Run the harness for the target language only. The finish gate is parity with OCaml for the fixture set under `fixtures/<language>/`.

Harness fixture expectation for successful terms:
- the fixture suite should follow the `fullsimple` pattern of pairing an unevaluated term wrapped as `lambda _:Unit. <term>;` with its evaluated form `(lambda _:Unit. <term>) unit;`
- this is required so the harness checks both printing of the original term and printing of the evaluated result
- before finishing, confirm that all supported term forms are covered by this paired fixture style unless the case is intentionally an error-only fixture

5. Optional spot checks.
If the harness mismatch suggests a printing-only difference or a top-level command mismatch, run one direct sample through the harness runner and inspect the output path locally before widening scope.

## Green Harness Definition

The harness is green only when the focused comparison for the new language matches the OCaml reference on:
- exit status
- standard output
- standard error

For successful fixtures, green also implies the paired unevaluated and evaluated commands both match OCaml formatting and result behavior.

## Failure Handling

- If unit tests fail, fix the local implementation or tests before touching broader registration surfaces.
- If harness output diverges, classify the mismatch as one of: parsing, evaluation, typing, printing, or error formatting.
- If the mismatch appears to expose an OCaml bug, preserve OCaml behavior by default, note the evidence, and ask before intentionally deviating.
- If a needed `Common/` change risks breaking existing languages, stop and ask for confirmation before proceeding.

## Final Report Requirements

Before ending, report:
- exact validation commands run
- unit-test outcome
- focused harness outcome
- any remaining known gaps
- any suspected OCaml bugs separated from C# implementation issues