# Tapl

Some of the implementation from the TAPL Book. The FullSimple example is the most correct and complete. I did not give much attention to printing.

## Grammar generation

ANTLR code generation now runs in Docker instead of requiring ANTLR to be installed on the host machine.

- Linux: run `./gen-all.sh` from the repository root, or `./gen.sh` inside a grammar directory.
- Windows: run `./gen-all.ps1` from the repository root, or `./gen.ps1` inside a grammar directory.
- Optional: set `ANTLR_DOCKER_IMAGE` to override the default Docker image name (`tapl-antlr-generator`).
- Linux note: the bash helper defaults to the bind-mount suffix `:Z` for SELinux-compatible container runtimes. Override it with `ANTLR_DOCKER_VOLUME_SUFFIX` if your Docker setup needs a different mount option.
- Linux note: the bash helper uses `--user` on Docker and `--userns keep-id` on Podman-style runtimes. Set `ANTLR_DOCKER_USE_HOST_USER=true|false` if you need to override that behavior.

The scripts build the generator image from `docker/antlr-generator/Dockerfile` and then run ANTLR inside the container with the repository mounted into `/workspace`.

## TAPL comparison harness

An initial cross-implementation harness is available for the TAPL languages that currently have C# ports:

- `arith`
- `simplebool`
- `untyped`
- `letexercise`
- `fullsimple`
- `fulluntyped`

The harness runs the OCaml TAPL implementation inside Docker, runs the C# implementation through the `Harness.Runner` console app, and compares stdout, stderr, and exit code. When the outputs differ, it reports the first differing line and prints a unified diff.

Run a comparison with:

```bash
./scripts/compare-tapl.sh simplebool fixtures/simplebool/simple_if.f
```

Run every fixture for a language with either the default fixture directory:

```bash
./scripts/compare-tapl.sh simplebool
```

or an explicit fixture directory:

```bash
./scripts/compare-tapl.sh simplebool fixtures/simplebool
```

By default the script runs the preconfigured OCaml image `public.ecr.aws/w9u4a6r8/ocaml:latest` directly. Override it with `TAPL_OCAML_HARNESS_IMAGE` if you want to test against a different image.

If you want a local image you can build yourself, the repository includes `docker/ocaml-harness/Dockerfile` plus a helper script:

```bash
./scripts/build-ocaml-harness-image.sh
```

That builds a local image named `tapl-ocaml-harness` by default. To make the harness use it:

```bash
TAPL_OCAML_HARNESS_IMAGE=tapl-ocaml-harness ./scripts/compare-tapl.sh simplebool
```

You can also open a shell in that image for manual OCaml-side testing with just the `TAPL` folder mounted:

```bash
docker run --rm -it \
	-v "$PWD/TAPL:/workspace/TAPL:Z" \
	-v tapl-dune-cache:/cache \
	-w /workspace/TAPL \
	tapl-ocaml-harness
```

From there you can move into a language folder and run it interactively, for example:

```bash
cd fullsimple
opam exec -- dune build . --build-dir /cache/fullsimple --profile release
opam exec -- dune exec --build-dir /cache/fullsimple --profile release fullsimple -- repl
```

If you want the full repository mounted instead of only `TAPL`, use:

```bash
docker run --rm -it \
	-v "$PWD:/workspace:Z" \
	-v tapl-dune-cache:/cache \
	-w /workspace/TAPL \
	tapl-ocaml-harness
```

If your Docker setup does not need the `:Z` bind-mount suffix, remove it from the command.

The repository is mounted into `/workspace`, and Dune build output is written into a named container volume (`tapl-dune-cache` by default) instead of the checkout itself. That avoids bind-mount permission issues and keeps OCaml build artifacts cached across repeated runs. Override the cache volume name with `TAPL_DUNE_CACHE_VOLUME` if needed.

Initial shared fixtures live under `fixtures/<language>/`. The files are intentionally small and single-purpose so output differences are easier to diagnose before moving on to larger TAPL examples.
