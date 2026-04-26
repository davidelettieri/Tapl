#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 2 ]]; then
    echo "Usage: $0 <grammar-directory> <grammar-file>" >&2
    exit 1
fi

SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
REPO_ROOT=$(cd -- "${SCRIPT_DIR}/.." && pwd)
GRAMMAR_DIR_REL=${1%/}
GRAMMAR_FILE=$2
GRAMMAR_NAME=${GRAMMAR_FILE%.g4}
GRAMMAR_DIR="${REPO_ROOT}/${GRAMMAR_DIR_REL}"
DOCKERFILE_PATH="${REPO_ROOT}/docker/antlr-generator/Dockerfile"
IMAGE_NAME=${ANTLR_DOCKER_IMAGE:-tapl-antlr-generator}
VOLUME_SUFFIX=${ANTLR_DOCKER_VOLUME_SUFFIX:-:Z}
USE_HOST_USER=${ANTLR_DOCKER_USE_HOST_USER:-auto}
IS_PODMAN=false

if [[ ! -d "${GRAMMAR_DIR}" ]]; then
    echo "Grammar directory not found: ${GRAMMAR_DIR}" >&2
    exit 1
fi

if [[ ! -f "${GRAMMAR_DIR}/${GRAMMAR_FILE}" ]]; then
    echo "Grammar file not found: ${GRAMMAR_DIR}/${GRAMMAR_FILE}" >&2
    exit 1
fi

rm -f \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}.interp" \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}.tokens" \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}BaseVisitor.cs" \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}Lexer.cs" \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}Lexer.interp" \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}Lexer.tokens" \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}Parser.cs" \
    "${GRAMMAR_DIR}/${GRAMMAR_NAME}Visitor.cs"

docker build -t "${IMAGE_NAME}" -f "${DOCKERFILE_PATH}" "${REPO_ROOT}"

if [[ "${USE_HOST_USER}" == "auto" ]]; then
    if docker --version 2>&1 | grep -qi podman; then
        IS_PODMAN=true
        USE_HOST_USER=true
    else
        USE_HOST_USER=true
    fi
elif docker --version 2>&1 | grep -qi podman; then
    IS_PODMAN=true
fi

DOCKER_ARGS=(
    run
    --rm
    -v "${REPO_ROOT}:/workspace${VOLUME_SUFFIX}"
)

if [[ "${USE_HOST_USER}" == "true" ]]; then
    if [[ "${IS_PODMAN}" == "true" ]]; then
        DOCKER_ARGS+=(--userns keep-id)
    elif command -v id >/dev/null 2>&1; then
        DOCKER_ARGS+=(--user "$(id -u):$(id -g)")
    fi
fi

DOCKER_ARGS+=(
    --entrypoint
    sh
    "${IMAGE_NAME}"
    -lc
    "set -eu && mkdir -p /tmp/antlr-out && cp /workspace/${GRAMMAR_DIR_REL}/${GRAMMAR_FILE} /tmp/${GRAMMAR_FILE} && java -jar /opt/antlr/antlr-4.13.1-complete.jar -Dlanguage=CSharp -o /tmp/antlr-out /tmp/${GRAMMAR_FILE} -visitor -no-listener && cd /tmp/antlr-out && tar -cf - ."
)

docker "${DOCKER_ARGS[@]}" | tar -xf - -C "${GRAMMAR_DIR}"