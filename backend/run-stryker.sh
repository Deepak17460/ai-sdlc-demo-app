#!/usr/bin/env bash
set -euo pipefail
export PATH="$PATH:$HOME/.dotnet/tools"
cd "$(dirname "$0")"
echo "Running Stryker from $(pwd)"
dotnet-stryker --config-file stryker-config.json
