dotnet clean -c Release ./ResumeAnalyzer.slnx
dotnet tool restore
dotnet restore ./ResumeAnalyzer.slnx
dotnet build --no-restore -c Release ./ResumeAnalyzer.slnx

# Frontend build
Push-Location web
pnpm install
pnpm run build
Pop-Location

if (Test-Path ./.coverage) { Remove-Item ./.coverage -Recurse -Force }
New-Item -ItemType Directory -Path ./.coverage | Out-Null

dotnet dotnet-coverage collect `
  --settings ./coverage.runsettings `
  --output ./.coverage/coverage.cobertura.xml `
  --output-format cobertura `
  "dotnet test --no-build -c Release ./ResumeAnalyzer.slnx"

dotnet reportgenerator `
  "-reports:./.coverage/coverage.cobertura.xml" `
  "-targetdir:./.coverage" `
  -reporttypes:"Html_Dark;Badges"
