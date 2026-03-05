include .env
export

.PHONY: build
build:
	dotnet restore
	dotnet build --no-restore --tl --no-incremental -warnaserror -maxcpucount
	dotnet list package --vulnerable
	du -sh
	du -sh *

.PHONY: build-and-create-test-data-booking-flow
build-and-create-test-data-booking-flow: build
	dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --filter "FullyQualifiedName=Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests.BookingFlowIntTest.CreateTestDataForBookingFlow"

.PHONY: clean
clean:
	dotnet clean
	find . -iname "bin" -print0 | xargs -0 rm -rf
	find . -iname "obj" -print0 | xargs -0 rm -rf
	find . -iname "node_modules" -print0 | xargs -0 rm -rf
	find . -iname "TestResults" -print0 | xargs -0 rm -rf
	find . -iname ".sonarqube" -print0 | xargs -0 rm -rf
	du -sh
	du -sh *

.PHONY: run-app-host
run-app-host:
	dotnet restore src/modules/Liberty.Reservation.AppHost/Liberty.Reservation.AppHost.csproj
	dotnet build src/modules/Liberty.Reservation.AppHost/Liberty.Reservation.AppHost.csproj --tl
	dotnet run --project src/modules/Liberty.Reservation.AppHost/Liberty.Reservation.AppHost.csproj --launch-profile "http"

.PHONY: test
test:
	dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --filter "FullyQualifiedName=Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests.BookingFlowIntTest.CreateTestDataForBookingFlow"
	dotnet test --logger:"console;verbosity=normal"

.PHONY: test-sequence
test-sequence:
	dotnet test src/services/employee/Liberty.Reservation.Employee.WebAPI.Test/Liberty.Reservation.Employee.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal
	dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --filter "FullyQualifiedName=Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests.BookingFlowIntTest.CreateTestDataForBookingFlow"
	dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal
	dotnet test src/services/manager/Liberty.Reservation.Manager.File.WebAPI.Test/Liberty.Reservation.Manager.File.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal
	dotnet test src/services/site/Liberty.Reservation.Site.WebAPI.Test/Liberty.Reservation.Site.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal
	dotnet test src/services/site/Liberty.Reservation.Site.File.WebAPI.Test/Liberty.Reservation.Site.File.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal
	dotnet test src/services/user/Liberty.Reservation.User.WebAPI.Test/Liberty.Reservation.User.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal
	dotnet test src/services/user/Liberty.Reservation.User.File.WebAPI.Test/Liberty.Reservation.User.File.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal
	dotnet test src/services/mail/Liberty.Reservation.Mail.Worker.Test/Liberty.Reservation.Mail.Worker.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal

.PHONY: test-employee-service
test-employee-service:
	dotnet test src/services/employee/Liberty.Reservation.Employee.WebAPI.Test/Liberty.Reservation.Employee.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal

.PHONY: test-manager-service
test-manager-service:
	dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal

.PHONY: test-site-service
test-site-service:
	dotnet test src/services/site/Liberty.Reservation.Site.WebAPI.Test/Liberty.Reservation.Site.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal

.PHONY: test-site-file-service
test-site-file-service:
	dotnet test src/services/site/Liberty.Reservation.Site.File.WebAPI.Test/Liberty.Reservation.Site.File.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal

.PHONY: test-user-service
test-user-service:
	dotnet test src/services/user/Liberty.Reservation.User.WebAPI.Test/Liberty.Reservation.User.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal

.PHONY: test-user-file-service
test-user-file-service:
	dotnet test src/services/user/Liberty.Reservation.User.File.WebAPI.Test/Liberty.Reservation.User.File.WebAPI.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect:"XPlat Code Coverage" --no-build --verbosity minimal

.PHONY: sonar-scanner
sonar-scanner: clean
	${CONTAINER} compose -f docker/sonar.yml up -d
	sh ./sonar-analysis.sh

.PHONY: sonar-scanner-without-test
sonar-scanner-without-test: clean
	${CONTAINER} compose -f docker/sonar.yml up -d
	sh ./sonar-analysis-without-test.sh

.PHONY: container-build
container-build:
	${CONTAINER} compose build

.PHONY: container-build-no-cache
container-build-no-cache:
	${CONTAINER} compose build --no-cache

.PHONY: container-down
container-down:
	${CONTAINER} compose down

.PHONY: container-up-dev
container-up-dev: container-up-dev-db container-up-dev-services

.PHONY: container-up-dev-db
container-up-dev-db:
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-db
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-redis
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-redis-commander
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-db-migration
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-pgadmin4
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-seq

.PHONY: container-up-dev-services
container-up-dev-services:
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-employee-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-manager-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-manager-file-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-user-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-user-file-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-site-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-site-file-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-mail-worker
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.dev.yml up -d --build liberty-reservation-booking-worker

.PHONY: container-up-test
container-up-test: container-up-test-db container-up-test-services

.PHONY: container-up-test-db
container-up-test-db:
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-db
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-redis
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-redis-commander
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-db-migration
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-pgadmin4

.PHONY: container-up-test-services
container-up-test-services:
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-employee-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-manager-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-manager-file-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-user-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-user-file-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-site-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-site-file-webapi
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-mail-worker
	${CONTAINER} compose -f docker-compose.yml -f docker-compose.override.test.yml up -d --build liberty-reservation-booking-worker
