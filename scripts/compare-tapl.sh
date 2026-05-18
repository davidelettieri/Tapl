#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
REPO_ROOT=$(cd -- "${SCRIPT_DIR}/.." && pwd)
LANGUAGE_SELECTOR=$(printf '%s' "${1:-all}" | tr '[:upper:]' '[:lower:]')
SOURCE_INPUT=${2:-}
IMAGE_NAME=${TAPL_OCAML_HARNESS_IMAGE:-public.ecr.aws/w9u4a6r8/ocaml:latest}
VOLUME_SUFFIX=${TAPL_DOCKER_VOLUME_SUFFIX:-:Z}
if [[ -n "${TAPL_DOCKER_USE_HOST_USER:-}" ]]; then
    USE_HOST_USER=${TAPL_DOCKER_USE_HOST_USER}
elif [[ "${GITHUB_ACTIONS:-}" == "true" ]]; then
    USE_HOST_USER=false
else
    USE_HOST_USER=true
fi
SUPPORTED_LANGUAGES=(arith simplebool untyped fullsimple fullpoly fulluntyped fullref fullerror fullupdate)

readonly SCRIPT_DIR
readonly REPO_ROOT
readonly LANGUAGE_SELECTOR
readonly SOURCE_INPUT
readonly IMAGE_NAME
readonly VOLUME_SUFFIX
readonly USE_HOST_USER
readonly SUPPORTED_LANGUAGES

usage() {
    echo "Usage: $0 [all|subfolder] [source-file-or-fixture-directory]" >&2
}

runner_output_path() {
    printf '%s\n' "${REPO_ROOT}/Harness.Runner/bin/Debug/net10.0/Harness.Runner.dll"
}

usage_supported() {
    printf 'Supported languages:' >&2
    local language
    for language in "${SUPPORTED_LANGUAGES[@]}"; do
        printf ' %s' "${language}" >&2
    done
    printf '\n' >&2
}

is_supported_language() {
    local candidate=$1
    local language

    for language in "${SUPPORTED_LANGUAGES[@]}"; do
        if [[ "${language}" == "${candidate}" ]]; then
            return 0
        fi
    done

    return 1
}

selected_languages() {
    if [[ "${LANGUAGE_SELECTOR}" == "all" ]]; then
        printf '%s\n' "${SUPPORTED_LANGUAGES[@]}"
        return 0
    fi

    if ! is_supported_language "${LANGUAGE_SELECTOR}"; then
        echo "Unsupported language: ${LANGUAGE_SELECTOR}" >&2
        usage
        usage_supported
        exit 1
    fi

    printf '%s\n' "${LANGUAGE_SELECTOR}"
}

resolve_source_target() {
    local language=$1
    local base_target

    if [[ -z "${SOURCE_INPUT}" ]]; then
        printf '%s\n' "${REPO_ROOT}/fixtures/${language}"
        return 0
    fi

    base_target=$(realpath -m "${SOURCE_INPUT}")

    if [[ "${LANGUAGE_SELECTOR}" != "all" ]]; then
        printf '%s\n' "${base_target}"
        return 0
    fi

    if [[ -f "${base_target}" ]]; then
        echo "The 'all' mode expects a fixture root directory, not a file: ${base_target}" >&2
        exit 1
    fi

    printf '%s\n' "${base_target}/${language}"
}

ensure_source_target() {
    local source_target=$1

    if [[ ! -e "${source_target}" ]]; then
        echo "Fixture target not found: ${source_target}" >&2
        exit 1
    fi

    if [[ "${source_target}" != "${REPO_ROOT}"/* ]]; then
        echo "Fixture target must be inside the repository so Docker can see it: ${source_target}" >&2
        exit 1
    fi
}

collect_fixture_files() {
    local source_target=$1

    if [[ -f "${source_target}" ]]; then
        printf '%s\n' "${source_target}"
        return 0
    fi

    if [[ ! -d "${source_target}" ]]; then
        echo "Fixture target must be a file or directory: ${source_target}" >&2
        return 1
    fi

    find "${source_target}" -maxdepth 1 -type f -name '*.f' | sort
}

ensure_ocaml_folder() {
    local language=$1
    local ocaml_dir="${REPO_ROOT}/TAPL-ocaml/${language}"

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
        "${REPO_ROOT}/FullRef" \
        "${REPO_ROOT}/FullSimple" \
        "${REPO_ROOT}/FullPoly" \
        "${REPO_ROOT}/FullUntyped" \
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

sanitize_runtime_stderr() {
    local stderr_file=$1

    sed -i '/^Emulate Docker CLI using podman\./d' "${stderr_file}"
}

docker_run_prefix() {
    local -n out_ref=$1

    out_ref=(
        docker
        run
        --rm
        -v "${REPO_ROOT}:/workspace${VOLUME_SUFFIX}"
    )

    if [[ "${USE_HOST_USER}" == "true" ]] && command -v id >/dev/null 2>&1; then
        out_ref+=(--user "$(id -u):$(id -g)")
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
    local language=$1
    local source_file=$2
    local stdout_file=$3
    local stderr_file=$4
    local container_source="/workspace/${source_file#"${REPO_ROOT}/"}"
    local build_dir="/tmp/tapl-${language}"
    local -a docker_args

    docker_run_prefix docker_args
    docker_args+=(
        -w "/workspace/TAPL-ocaml/${language}"
        --entrypoint sh
        "${IMAGE_NAME}"
        -lc
        "set -eu && mkdir -p ${build_dir} && opam exec -- dune build . --build-dir ${build_dir} --profile release && opam exec -- dune exec --build-dir ${build_dir} --profile release ${language} -- ${container_source}"
    )

    "${docker_args[@]}" >"${stdout_file}" 2>"${stderr_file}"
}

run_csharp() {
    local language=$1
    local source_file=$2
    local stdout_file=$3
    local stderr_file=$4

    dotnet run --no-build --project "${REPO_ROOT}/Harness.Runner/Harness.Runner.csproj" -- "${language}" "${source_file}" >"${stdout_file}" 2>"${stderr_file}"
}

compare_fixture() {
    local language=$1
    local source_file=$2
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
    run_ocaml "${language}" "${source_file}" "${ocaml_stdout}" "${ocaml_stderr}"
    ocaml_exit=$?
    run_csharp "${language}" "${source_file}" "${csharp_stdout}" "${csharp_stderr}"
    csharp_exit=$?
    set -e

    sanitize_runtime_stderr "${ocaml_stderr}"

    echo "Compared ${language} using ${source_file#"${REPO_ROOT}/"}."
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

compare_language() {
    local language=$1
    local source_target=$2
    local fixture_file
    local fixture_count=0
    local failure_count=0

    ensure_source_target "${source_target}"
    ensure_ocaml_folder "${language}"

    while IFS= read -r fixture_file; do
        if [[ -z "${fixture_file}" ]]; then
            continue
        fi

        fixture_count=$((fixture_count + 1))

        if ! compare_fixture "${language}" "${fixture_file}"; then
            failure_count=$((failure_count + 1))
        fi

        echo
    done < <(collect_fixture_files "${source_target}")

    if [[ ${fixture_count} -eq 0 ]]; then
        echo "No fixture files found for ${language} under ${source_target}." >&2
        return 1
    fi

    if [[ ${failure_count} -ne 0 ]]; then
        echo "${failure_count} of ${fixture_count} fixture comparisons failed for ${language}." >&2
        return 1
    fi

    echo "All ${fixture_count} fixture comparisons passed for ${language}."
}

main() {
    if [[ $# -gt 2 ]]; then
        usage
        exit 1
    fi

    ensure_csharp_runner_built

    local language
    local source_target
    local failure_count=0

    while IFS= read -r language; do
        if [[ -z "${language}" ]]; then
            continue
        fi

        source_target=$(resolve_source_target "${language}")
        echo "=== ${language} ==="

        if ! compare_language "${language}" "${source_target}"; then
            failure_count=$((failure_count + 1))
        fi

        echo
    done < <(selected_languages)

    if [[ ${failure_count} -ne 0 ]]; then
        echo "${failure_count} language runs failed." >&2
        exit 1
    fi
}

main
