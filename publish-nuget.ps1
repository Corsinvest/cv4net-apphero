# SPDX-FileCopyrightText: Copyright Corsinvest Srl
# SPDX-License-Identifier: AGPL-3.0-only

 .\setKey.ps1

#Read project version
$xml = [xml](Get-Content .\src\common.props)
$version = $xml.Project.PropertyGroup[0].Version
Write-Host "Project version: $version"

dotnet nuget push .\nupkgs\Corsinvest.AppHero.Auditing.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Auditing.MudBlazorUI.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Authentication.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Authentication.MudBlazorUI.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Core.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Core.BaseUI.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Core.FluentUI.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Core.MudBlazorUI.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Core.RazdenUI.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.HangFire.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Localization.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Localization.MudBlazorUI.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Notification.Telegram.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Serilog.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push .\nupkgs\Corsinvest.AppHero.Translation.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
