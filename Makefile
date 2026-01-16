include .env
export

.PHONY: build
build:
	dotnet restore
	dotnet build --no-restore --tl --no-incremental -warnaserror -maxcpucount
	dotnet list package --vulnerable
	du -sh
	du -sh *

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
