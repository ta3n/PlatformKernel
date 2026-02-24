#!/bin/bash

dotnet tool install --global dotnet-sonarscanner
dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-coverage

dotnet tool update --global dotnet-sonarscanner
dotnet tool update --global coverlet.console
dotnet tool update --global dotnet-coverage

echo "SonarQube beginning..."
dotnet sonarscanner begin /k:"platform-kernel" /s:"$(pwd)/SonarQube.Analysis.xml" /d:sonar.scanner.scanAll=false

echo "DotNet building..."
dotnet build --no-incremental --tl

echo "Dotnet testing..."

dotnet test --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal

echo "SonarQube ending..."
dotnet sonarscanner end

echo "Sonar analysis done!"
