#!/bin/bash

set -e

echo "📦 Ensuring restore & build done before test..."
dotnet restore Liberty.Reservation.slnf
dotnet build Liberty.Reservation.slnf --no-restore --tl --no-incremental -warnaserror -maxcpucount

dotnet test src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj --no-build --filter "FullyQualifiedName=Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests.BookingFlowIntTest.CreateTestDataForBookingFlow"

echo "▶️ Detecting available CPU cores..."
if command -v nproc >/dev/null 2>&1; then
  CPU_COUNT=$(nproc)
else
  CPU_COUNT=$(getconf _NPROCESSORS_ONLN)
fi
echo "🧠 Detected $CPU_COUNT CPU cores"

echo "▶️ Defining test projects list..."
declare -a projects=(
  "src/services/employee/Liberty.Reservation.Employee.WebAPI.Test/Liberty.Reservation.Employee.WebAPI.Test.csproj"
  "src/services/manager/Liberty.Reservation.Manager.WebAPI.Test/Liberty.Reservation.Manager.WebAPI.Test.csproj"
  "src/services/manager/Liberty.Reservation.Manager.File.WebAPI.Test/Liberty.Reservation.Manager.File.WebAPI.Test.csproj"
  "src/services/manager/Liberty.Reservation.Manager.Distribution.WebAPI.Test/Liberty.Reservation.Manager.Distribution.WebAPI.Test.csproj"
  "src/services/site/Liberty.Reservation.Site.WebAPI.Test/Liberty.Reservation.Site.WebAPI.Test.csproj"
  "src/services/site/Liberty.Reservation.Site.File.WebAPI.Test/Liberty.Reservation.Site.File.WebAPI.Test.csproj"
  "src/services/user/Liberty.Reservation.User.WebAPI.Test/Liberty.Reservation.User.WebAPI.Test.csproj"
  "src/services/user/Liberty.Reservation.User.File.WebAPI.Test/Liberty.Reservation.User.File.WebAPI.Test.csproj"
  "src/services/mail/Liberty.Reservation.Mail.Worker.Test/Liberty.Reservation.Mail.Worker.Test.csproj"
)

echo "🚀 Running tests in parallel using xargs -P $CPU_COUNT..."

run_test() {
  proj="$1"
  echo "▶️ Running test for: $proj"
  start=$(date +%s)
  dotnet test "$proj" --no-restore --no-build --verbosity minimal
  end=$(date +%s)
  echo "✅ Finished $proj in $((end - start))s"
}

export -f run_test

# Convert array to newline and pass to xargs
printf "%s\n" "${projects[@]}" | xargs -P "$CPU_COUNT" -I {} bash -c 'run_test "$@"' _ {}

echo "✅ All tests completed."
