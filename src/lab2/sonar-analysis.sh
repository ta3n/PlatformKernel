#!/bin/bash

dotnet tool install --global dotnet-sonarscanner
dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-coverage

dotnet tool update --global dotnet-sonarscanner
dotnet tool update --global coverlet.console
dotnet tool update --global dotnet-coverage

echo "SonarQube beginning..."
dotnet sonarscanner begin /k:"liberty-reservation-asp" /s:"$(pwd)/SonarQube.Analysis.xml" /d:sonar.scanner.scanAll=false

echo "DotNet building..."
dotnet build --no-incremental --tl

echo "Dotnet testing..."

# dotnet test src/services/employee/Liberty.Reservation.Employee.WebAPI.Test/Liberty.Reservation.Employee.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/manager/Liberty.Reservation.Manager.File.WebAPI.Test/Liberty.Reservation.Manager.File.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/manager/Liberty.Reservation.Manager.Distrubtion.WebAPI.Test/Liberty.Reservation.Manager.Distrubtion.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/site/Liberty.Reservation.Site.WebAPI.Test/Liberty.Reservation.Site.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/site/Liberty.Reservation.Site.File.WebAPI.Test/Liberty.Reservation.Site.File.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/user/Liberty.Reservation.User.WebAPI.Test/Liberty.Reservation.User.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/user/Liberty.Reservation.User.File.WebAPI.Test/Liberty.Reservation.User.File.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/mail/Liberty.Reservation.Mail.Worker.Test/Liberty.Reservation.Mail.Worker.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal

dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --filter "FullyQualifiedName=Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests.BookingFlowIntTest.CreateTestDataForBookingFlow"
dotnet test --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal

echo "SonarQube ending..."
dotnet sonarscanner end

echo "Sonar analysis done!"
