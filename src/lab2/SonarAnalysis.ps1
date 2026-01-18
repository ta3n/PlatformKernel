dotnet tool install --global dotnet-sonarscanner
dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-coverage

dotnet tool update --global dotnet-sonarscanner
dotnet tool update --global coverlet.console
dotnet tool update --global dotnet-coverage

dotnet sonarscanner begin /d:sonar.login=admin /d:sonar.password=admin /k:"liberty-reservation-asp" /d:sonar.host.url="http://localhost:9001" /s:"$(pwd)/SonarQube.Analysis.xml"

dotnet build --no-incremental --tl

# dotnet test src/services/employee/Liberty.Reservation.Employee.WebAPI.Test/Liberty.Reservation.Employee.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --filter "FullyQualifiedName=Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests.BookingFlowIntTest.CreateTestDataForBookingFlow"
# dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/manager/Liberty.Reservation.Manager.File.WebAPI.Test/Liberty.Reservation.Manager.File.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/site/Liberty.Reservation.Site.WebAPI.Test/Liberty.Reservation.Site.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/site/Liberty.Reservation.Site.File.WebAPI.Test/Liberty.Reservation.Site.File.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/user/Liberty.Reservation.User.WebAPI.Test/Liberty.Reservation.User.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/user/Liberty.Reservation.User.File.WebAPI.Test/Liberty.Reservation.User.File.WebAPI.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal
# dotnet test src/services/mail/Liberty.Reservation.Mail.Worker.Test/Liberty.Reservation.Mail.Worker.Test.csproj --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal

dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --filter "FullyQualifiedName=Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests.BookingFlowIntTest.CreateTestDataForBookingFlow"
dotnet test --collect:"XPlat Code Coverage;Format=opencover" --no-build --verbosity minimal

dotnet sonarscanner end /d:sonar.login=admin /d:sonar.password=admin
