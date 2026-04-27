#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
    echo "Usage: $0 <subfolder> [source-file-or-fixture-directory]" >&2
    exit 1
fi

SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
REPO_ROOT=$(cd -- "${SCRIPT_DIR}/.." && pwd)
SUBFOLDER=$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')
SOURCE_TARGET=$(realpath -m "${2:-"${REPO_ROOT}/fixtures/${SUBFOLDER}"}")
IMAGE_NAME=${TAPL_OCAML_HARNESS_IMAGE:-public.ecr.aws/w9u4a6r8/ocaml:latest}
DUNE_CACHE_VOLUME=${TAPL_DUNE_CACHE_VOLUME:-tapl-dune-cache}
VOLUME_SUFFIX=${TAPL_DOCKER_VOLUME_SUFFIX:-:Z}
if [[ -n "${TAPL_DOCKER_USE_HOST_USER:-}" ]]; then
    USE_HOST_USER=${TAPL_DOCKER_USE_HOST_USER}
elif [[ "${GITHUB_ACTIONS:-}" == "true" ]]; then
    USE_HOST_USER=false
else
    USE_HOST_USER=auto
fi
IS_PODMAN=false

readonly SCRIPT_DIR
readonly REPO_ROOT
readonly SUBFOLDER
readonly SOURCE_TARGET
readonly IMAGE_NAME
readonly DUNE_CACHE_VOLUME
readonly VOLUME_SUFFIX
readonly USE_HOST_USER

runner_output_path() {
    printf '%s\n' "${REPO_ROOT}/Harness.Runner/bin/Debug/net10.0/Harness.Runner.dll"
}

usage_supported() {
    echo "Supported languages: arith, simplebool, untyped, letexercise, fullsimple" >&2
}

ensure_supported_language() {
    case "${SUBFOLDER}" in
        arith|simplebool|untyped|letexercise|fullsimple)
            return 0
            ;;
        *)
            echo "Unsupported language: ${SUBFOLDER}" >&2
            usage_supported
            exit 1
            ;;
    esac
}

ensure_source_target() {
    if [[ ! -e "${SOURCE_TARGET}" ]]; then
        echo "Fixture target not found: ${SOURCE_TARGET}" >&2
        exit 1
    fi

    if [[ "${SOURCE_TARGET}" != "${REPO_ROOT}"/* ]]; then
        echo "Fixture target must be inside the repository so Docker can see it: ${SOURCE_TARGET}" >&2
        exit 1
    fi
}

collect_fixture_files() {
    if [[ -f "${SOURCE_TARGET}" ]]; then
        printf '%s\n' "${SOURCE_TARGET}"
        return 0
    fi

    if [[ ! -d "${SOURCE_TARGET}" ]]; then
        echo "Fixture target must be a file or directory: ${SOURCE_TARGET}" >&2
        return 1
    fi

    find "${SOURCE_TARGET}" -maxdepth 1 -type f -name '*.f' | sort
}

ensure_ocaml_folder() {
    local ocaml_dir="${REPO_ROOT}/TAPL/${SUBFOLDER}"

    if [[ ! -d "${ocaml_dir}" ]]; then
        echo "OCaml TAPL folder not found: ${ocaml_dir}" >&2
        exit 1
    fi
}

runner_sources_newer_than_output() {
    local output_path=$1

    find \
        "${REPO_ROOT}/Common" \
        "${REPO_ROOT}/Arith" \
        "${REPO_ROOT}/SimpleBool" \
        "${REPO_ROOT}/Untyped" \
        "${REPO_ROOT}/LetExercise" \
        "${REPO_ROOT}/FullSimple" \
        "${REPO_ROOT}/Harness.Runner" \
        -type f \
        \( -name '*.cs' -o -name '*.csproj' -o -name '*.g4' \) \
        -newer "${output_path}" \
        -print -quit | grep -q .
}

ensure_csharp_runner_built() {
    local output_path
    output_path=$(runner_output_path)

    if [[ ! -f "${output_path}" ]] || runner_sources_newer_than_output "${output_path}"; then
        dotnet build --no-restore "${REPO_ROOT}/Harness.Runner/Harness.Runner.csproj"
    fi
}

cleanup() {
    if [[ -n "${1:-}" && -d "${1}" ]]; then
        rm -rf "${1}"
    fi
}

is_podman_cli() {
    local docker_version
    local docker_path
    local resolved_path

    docker_version=$(docker --version 2>&1 || true)
    if [[ "${docker_version}" == *podman* || "${docker_version}" == *Podman* ]]; then
        return 0
    fi

    docker_path=$(command -v docker 2>/dev/null || true)
    if [[ -z "${docker_path}" ]]; then
        return 1
    fi

    resolved_path=$(readlink -f "${docker_path}" 2>/dev/null || printf '%s' "${docker_path}")
    [[ "${resolved_path}" == *podman* || "${resolved_path}" == *Podman* ]]
}

sanitize_runtime_stderr() {
    local stderr_file=$1

    sed -i '/^Emulate Docker CLI using podman\./d' "${stderr_file}"
}

detect_user_mode() {
    if [[ "${USE_HOST_USER}" == "auto" ]]; then
        if is_podman_cli; then
            IS_PODMAN=true
        fi

        return 0
    fi

    if is_podman_cli; then
        IS_PODMAN=true
    fi
}

docker_run_prefix() {
    local -n out_ref=$1

    out_ref=(
        docker
        run
        --rm
        -v "${REPO_ROOT}:/workspace${VOLUME_SUFFIX}"
        -v "${DUNE_CACHE_VOLUME}:/cache"
    )

    if [[ "${USE_HOST_USER}" == "true" || "${USE_HOST_USER}" == "auto" ]]; then
        if command -v id >/dev/null 2>&1; then
            out_ref+=(--user "$(id -u):$(id -g)")
        fi
    fi
}

first_diff_line() {
    awk '
        NR == FNR {
            left[FNR] = $0
            left_count = FNR
            next
        }
        {
            if (!(FNR in left) || left[FNR] != $0) {
                print FNR
                found = 1
                exit
            }
        }
        END {
            if (!found && left_count != FNR) {
                if (left_count < FNR) {
                    print left_count + 1
                } else {
                    print FNR + 1
                }
            }
        }
    ' "$1" "$2"
}

print_diff() {
    local label=$1
    local left_file=$2
    local right_file=$3

    echo
    echo "${label} diff:"

    if diff --help 2>&1 | grep -q -- '--color'; then
        diff --color=always -u --label "ocaml/${label}" --label "csharp/${label}" "${left_file}" "${right_file}" || true
        return 0
    fi

    diff -u --label "ocaml/${label}" --label "csharp/${label}" "${left_file}" "${right_file}" || true
}

run_ocaml() {
    local source_file=$1
    local stdout_file=$2
    local stderr_file=$3
    local container_source="/workspace/${source_file#"${REPO_ROOT}/"}"
    local build_dir="/cache/${SUBFOLDER}"
    local -a docker_args

    docker_run_prefix docker_args
    docker_args+=(
        -w "/workspace/TAPL/${SUBFOLDER}"
        --entrypoint sh
        "${IMAGE_NAME}"
        -lc
        "set -eu && mkdir -p ${build_dir} && opam exec -- dune build . --build-dir ${build_dir} --profile release && opam exec -- dune exec --build-dir ${build_dir} --profile release ${SUBFOLDER} -- ${container_source}"
    )

    "${docker_args[@]}" >"${stdout_file}" 2>"${stderr_file}"
}

run_csharp() {
    local source_file=$1
    local stdout_file=$2
    local stderr_file=$3

    dotnet run --no-build --project "${REPO_ROOT}/Harness.Runner/Harness.Runner.csproj" -- "${SUBFOLDER}" "${source_file}" >"${stdout_file}" 2>"${stderr_file}"
}

compare_fixture() {
    local source_file=$1
    local temp_dir
    temp_dir=$(mktemp -d)
    local ocaml_stdout="${temp_dir}/ocaml.stdout"
    local ocaml_stderr="${temp_dir}/ocaml.stderr"
    local csharp_stdout="${temp_dir}/csharp.stdout"
    local csharp_stderr="${temp_dir}/csharp.stderr"
    local ocaml_exit=0
    local csharp_exit=0
    local failed=false

    trap 'cleanup "${temp_dir:-}"' RETURN

    set +e
    run_ocaml "${source_file}" "${ocaml_stdout}" "${ocaml_stderr}"
    ocaml_exit=$?
    run_csharp "${source_file}" "${csharp_stdout}" "${csharp_stderr}"
    csharp_exit=$?
    set -e

    sanitize_runtime_stderr "${ocaml_stderr}"

    echo "Compared ${SUBFOLDER} using ${source_file#"${REPO_ROOT}/"}."
    echo "OCaml exit code: ${ocaml_exit}"
    echo "C# exit code: ${csharp_exit}"

    if [[ ${ocaml_exit} -ne ${csharp_exit} ]]; then
        echo "Exit codes differ."
        failed=true
    fi

    if ! cmp -s "${ocaml_stdout}" "${csharp_stdout}"; then
        report_stream_mismatch stdout "${ocaml_stdout}" "${csharp_stdout}"
        failed=true
    fi

    if ! cmp -s "${ocaml_stderr}" "${csharp_stderr}"; then
        report_stream_mismatch stderr "${ocaml_stderr}" "${csharp_stderr}"
        failed=true
    fi

    if [[ "${failed}" == "true" ]]; then
        return 1
    fi

    echo "Outputs match."
    return 0
}

report_stream_mismatch() {
    local label=$1
    local left_file=$2
    local right_file=$3
    local line_number

    line_number=$(first_diff_line "${left_file}" "${right_file}")
    echo "${label} differs at line ${line_number:-1}."
    print_diff "${label}" "${left_file}" "${right_file}"
}

main() {
    ensure_supported_language
    ensure_source_target
    ensure_ocaml_folder
    detect_user_mode
    ensure_csharp_runner_built

    local fixture_file
    local fixture_count=0
    local failure_count=0

    while IFS= read -r fixture_file; do
        if [[ -z "${fixture_file}" ]]; then
            continue
        fi

        fixture_count=$((fixture_count + 1))

        if ! compare_fixture "${fixture_file}"; then
            failure_count=$((failure_count + 1))
        fi

        echo
    done < <(collect_fixture_files)

    if [[ ${fixture_count} -eq 0 ]]; then
        echo "No fixture files found for ${SUBFOLDER} under ${SOURCE_TARGET}." >&2
        exit 1
    fi

    if [[ ${failure_count} -ne 0 ]]; then
        echo "${failure_count} of ${fixture_count} fixture comparisons failed." >&2
        exit 1
    fi

    echo "All ${fixture_count} fixture comparisons passed for ${SUBFOLDER}."
}

main