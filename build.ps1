dotnet clean -c Release ./ResumeAnalyzer.slnx
dotnet tool restore
dotnet restore ./ResumeAnalyzer.slnx
dotnet build -c Release ./ResumeAnalyzer.slnx --no-restore
dotnet test -c Release ./ResumeAnalyzer.slnx --collect:"XPlat Code Coverage" -p:CollectCoverage=true -p:Threshold=0 --no-build
dotnet reportgenerator "-reports:./tests/**/coverage.cobertura.xml" "-targetdir:./.coverage" "-reporttypes:Html_Dark" "-classfilters:-Microsoft.*;-System.*" "-filefilters:-*.g.cs;-*.generated.cs"