#!/usr/bin/env bash
# ./test.sh                    -> full suite
# ./test.sh PerftTests         -> filter: dotnet test --filter "FullyQualifiedName~PerftTests"
set -euo pipefail
IMG=mcr.microsoft.com/dotnet/sdk:8.0
CMD="dotnet test ChessEngine.sln"
[ $# -gt 0 ] && CMD="$CMD --filter FullyQualifiedName~$1"
docker run --rm -v "$PWD":/src -w /src -v chess-nuget:/root/.nuget $IMG bash -c "$CMD"
