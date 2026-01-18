#!/bin/bash

set -e
declare -a skip_projects=(
  "src/liberty-asp/Liberty.Application.Test/Liberty.Application.Test.csproj"
  "src/liberty-asp/Liberty.ApplicationShared.Test/Liberty.ApplicationShared.Test.csproj"
)
find . -type f -name "*.Test.csproj" | while read -r proj; do
  skip=false
  for skip_proj in "${skip_projects[@]}"; do
    if [[ "$skip_proj" == "$proj" ]]; then
      skip=true
      break
    fi
  done
  if $skip; then
    echo "⏩ Skipping $proj"
    continue
  fi
  echo "▶️ Running tests for $proj"
  dotnet test "$proj" --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
done
