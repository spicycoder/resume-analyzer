dotnet tool update --all
dotnet aspire upgrade
dotnet package update --all

Push-Location web
pnpm update
Pop-Location
