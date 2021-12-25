mainbin := GameImpl/bin/Debug/net5.0/BuildSimpleProject

cobertura := GameImpl.Tests/bin/cobertura.xml
coberturahtml := GameImpl/bin/cobertura.html
strykerhtml := GameImpl.Tests/bin/stryker.html
pycobertura := env/bin/pycobertura
dep := Nez/patch-applied MonoGame/patch-applied

SOURCES = $(shell find GameImpl -path GameImpl/obj -prune -o -type f -name "*.cs")
TESTS = $(shell find GameImpl.Tests -path GameImpl.Tests/obj -prune -o -type f -name "*.cs")

.PHONY: all
all: $(mainbin)

Makefile: ;
%.cs: ;
%.txt: ;

$(mainbin): $(dep) $(SOURCES)
	dotnet build

.PHONY: test
test: $(cobertura)

.PHONY: htmltest
htmltest: $(coberturahtml)

.PHONY: stryker
stryker: $(strykerhtml)

.PHONY: run
run: ${mainbin}
	dotnet run -p GameImpl

$(cobertura): $(pycobertura) $(dep) $(SOURCES) $(TESTS)
	rm -rf GameImpl.Tests/TestResults
	dotnet test --collect:"XPlat Code Coverage" --settings GameImpl.Tests/coverlet.runsettings
	mv GameImpl.Tests/TestResults/*/coverage.cobertura.xml GameImpl.Tests/bin/cobertura.xml
	. env/bin/activate; pycobertura show GameImpl.Tests/bin/cobertura.xml

$(coberturahtml): $(cobertura)
	cp $(cobertura) GameImpl/cobertura.xml
	(cd GameImpl; pycobertura show --format html --output bin/cobertura.html cobertura.xml; rm cobertura.xml)
	
$(strykerhtml): $(cobertura)
	rm -rf GameImpl.Tests/StrykerOutput
	(cd GameImpl.Tests; dotnet stryker; mv StrykerOutput/*/reports/mutation-report.html bin/stryker.html)

Nez/patch-applied:
	patch -d Nez -p1 < patches/0001-Patched-Nez.patch
MonoGame/patch-applied:
	patch -d MonoGame -p1 < patches/0002-Unlocked-monogame.patch

env:
	python3 -m venv env

$(pycobertura): env requirements.txt
	. env/bin/activate; pip install -r requirements.txt
	@ echo "Please run 'source env/bin/activate'"
