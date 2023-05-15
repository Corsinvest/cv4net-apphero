# SPDX-FileCopyrightText: Copyright Corsinvest Srl
# SPDX-License-Identifier: AGPL-3.0-only

param(
    [Parameter(Mandatory=$true)]
	[ValidateSet('test','build')]
	[System.String]$operation
)

#read project version
$xml = [xml](Get-Content .\src\common.props)
$version = $xml.Project.PropertyGroup[0].Version
Write-Host "Project version: $version"

Write-Host "Operation: $operation "
	
function Build-Docker()
{
	#apphero
	Write-Host "Build Docker cv4net-apphero"
	docker rmi corsinvest/cv4net-apphero:$version --force
	docker build --rm -f "Dockerfile" -t corsinvest/cv4net-apphero:$version "."

	#remove unused images
	docker image prune -f
}

function Test-Docker()
{
	New-Item -Path "d:\DockerData\cv4net-apphero\data" -ItemType "directory" 

	if (!(Test-Path "d:\DockerData\cv4net-apphero\appsettings.json"))
	{
		Copy-Item "src\Corsinvest.AppHero.AppBss\appsettings.json" -Destination "d:\DockerData\cv4net-apphero\appsettings.json"
	}

	docker run --rm -it `
		-p 5000:80 `
		-e TZ=Europe/Rome `
		-v d:/DockerData/cv4net-apphero/data:/app/data `
		-v d:/DockerData/cv4net-apphero/appsettings.json:/app/appsettings.json `
		--name cv4net-apphero `
		corsinvest/cv4net-apphero:$version	
}

if($operation -eq 'test') 
{ 
	Test-Docker
}
elseif($operation -eq 'build') 
{ 
	Build-Docker
}