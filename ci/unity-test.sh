#!/usr/bin/env bash
set -euo pipefail

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
RESULTS_PATH="${PROJECT_PATH}/ci/test-results"
LOG_PATH="${RESULTS_PATH}/editor.log"
UNITY_PATH=${UNITY_PATH:-"/Applications/Unity/Hub/Editor/2022.3.11f1/Unity.app/Contents/MacOS/Unity"}

mkdir -p "${RESULTS_PATH}"

echo "Running Unity tests in batchmode..."
"${UNITY_PATH}" \
  -batchmode \
  -nographics \
  -projectPath "${PROJECT_PATH}" \
  -runTests \
  -testResults "${RESULTS_PATH}/TestResults.xml" \
  -logFile "${LOG_PATH}" \
  -quit

echo "Test results: ${RESULTS_PATH}/TestResults.xml"
echo "Editor log: ${LOG_PATH}"
