#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)

bash "${SCRIPT_DIR}/../Arith/Grammar/gen.sh"
bash "${SCRIPT_DIR}/../FullSimple/Grammar/gen.sh"
bash "${SCRIPT_DIR}/../LetExercise/Grammar/gen.sh"
bash "${SCRIPT_DIR}/../SimpleBool/Grammar/gen.sh"
bash "${SCRIPT_DIR}/../Untyped/Grammar/gen.sh"