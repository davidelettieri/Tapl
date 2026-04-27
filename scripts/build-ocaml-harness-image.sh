#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
REPO_ROOT=$(cd -- "${SCRIPT_DIR}/.." && pwd)
IMAGE_NAME=${TAPL_OCAML_HARNESS_IMAGE_NAME:-tapl-ocaml-harness}
DOCKERFILE_PATH="${REPO_ROOT}/docker/ocaml-harness/Dockerfile"
BUILD_CONTEXT="${REPO_ROOT}/docker/ocaml-harness"

if [[ ! -f "${DOCKERFILE_PATH}" ]]; then
    echo "Dockerfile not found: ${DOCKERFILE_PATH}" >&2
    exit 1
fi

docker build \
    -t "${IMAGE_NAME}" \
    -f "${DOCKERFILE_PATH}" \
    "${BUILD_CONTEXT}"

echo "Built OCaml harness image: ${IMAGE_NAME}"