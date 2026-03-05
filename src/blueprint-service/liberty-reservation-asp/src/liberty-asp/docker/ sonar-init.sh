#!/bin/bash

SONAR_URL="http://localhost:9001"
SONAR_PASSWORD="Admin123456a@"

# echo "Waiting additional 20 seconds for SonarQube readiness..."
# sleep 20

echo "Checking SonarQube Status API response..."
STATUS_JSON=$(curl -s $SONAR_URL/api/system/status)
echo "SonarQube API response: $STATUS_JSON"

echo "Changing default admin password..."
curl -u admin:admin -X POST \
  "$SONAR_URL/api/users/change_password?login=admin&previousPassword=admin&password=$SONAR_PASSWORD"

echo "Creating new Quality Gate: Liberty-Quality-Gate..."
curl -u admin:$SONAR_PASSWORD -X POST \
  "$SONAR_URL/api/qualitygates/create?name=Liberty-Quality-Gate"

echo "Adding condition: Code Coverage >= 65%"
curl -u admin:$SONAR_PASSWORD -X POST \
  "$SONAR_URL/api/qualitygates/create_condition" \
  --data-urlencode "gateName=Liberty-Quality-Gate" \
  --data-urlencode "metric=coverage" \
  --data-urlencode "op=LT" \
  --data-urlencode "error=65"

echo "Adding condition: Duplicated Lines <= 20%"
curl -u admin:$SONAR_PASSWORD -X POST \
  "$SONAR_URL/api/qualitygates/create_condition" \
  --data-urlencode "gateName=Liberty-Quality-Gate" \
  --data-urlencode "metric=duplicated_lines_density" \
  --data-urlencode "op=GT" \
  --data-urlencode "error=20"

echo "Setting Liberty-Quality-Gate as default..."
curl -u admin:$SONAR_PASSWORD -X POST \
  "$SONAR_URL/api/qualitygates/set_as_default" \
  --data-urlencode "name=Liberty-Quality-Gate"

echo "Initialization complete."
