# Integration Checklist

This repo has several hardcoded language lists and dispatch tables. Every new language must be wired into all relevant surfaces during the same change.

## Mandatory Surfaces

### Solution membership

- `Tapl.slnx`
Add the new language project and the new test project.

### Grammar generation

- `gen-all.sh`
- `scripts/gen-all.sh`
Add the new grammar generation script to both files.

### Harness runner

- `Harness.Runner/Program.cs`
Add the language to usage text and dispatch logic.

- `Harness.Runner/Harness.Runner.csproj`
Add a project reference for the new language.

### Harness comparison script

- `scripts/compare-tapl.sh`
Add the new language to the supported language list and keep any build-source tracking in sync if the script uses explicit project folders.

### Demo support

- `Demo/Program.cs`
Add the language to the sample chooser and dispatch map.

- `Demo/Demo.csproj`
Add the new project reference and ensure the sample source file is copied to the output.

- `Demo/<language>.txt`
Add a representative sample source file.

### Fixtures

- `fixtures/<language>/`
Add focused fixture files used by the harness.

Fixture authoring rule:
- follow the `fullsimple` convention for successful terms: each focused fixture should usually contain two commands, first `lambda _:Unit. <term>;` and then `(lambda _:Unit. <term>) unit;`
- use the first command to lock down pretty-printing and type-printing of the original term
- use the second command to lock down evaluation plus pretty-printing of the resulting value
- ensure the fixture set covers every supported term constructor and important composite form at least once
- keep error fixtures focused on the failing behavior when the two-command pattern does not apply

## Usually Required

- `<Language>/<Language>.csproj`
- `<Language>.Tests/<Language>.Tests.csproj`
- `README.md` updates only if the new language changes documented workflow or support expectations

## Do Not Skip

- Keep file and folder naming aligned with existing project conventions.
- Re-run the checklist after code changes if you touch build or runner wiring late in the task.
- Treat duplicated language lists as failure-prone. Confirm all of them were updated before finishing.