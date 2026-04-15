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