#!/bin/bash

dotnet tool install --global dotnet-ef
dotnet ef migrations script --idempotent -o "SqlScripts/migrations_$(date +%Y%m%d%H%M%S).sql"
