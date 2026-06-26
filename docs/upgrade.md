# Upgrade

```PowerShell
./upgrade.ps1
```

Upgrades:

- **Dotnet tools** — updates all local tools from `dotnet-tools.json`
- **Aspire** — runs `dotnet aspire upgrade`
- **NuGet packages** — updates all packages in `Directory.Packages.props` (CPM)

> Note: Does not upgrade .NET SDK in `global.json`. No straightforward CLI exists for this yet.
