dotnet tool install --global dotnet-sonarscanner
dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-coverage

dotnet tool update --global dotnet-sonarscanner
dotnet tool update --global coverlet.console
dotnet tool update --global dotnet-coverage

dotnet sonarscanner begin /d:sonar.login=admin /d:sonar.password=admin /k:"platform-kernel" /d:sonar.host.url="http://localhost:9001" /s:"$(pwd)/SonarQube.Analysis.xml"

dotnet build --no-incremental --tl

dotnet test --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal

dotnet sonarscanner end /d:sonar.login=admin /d:sonar.password=admin
