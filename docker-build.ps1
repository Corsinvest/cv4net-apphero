# SPDX-FileCopyrightText: Copyright Corsinvest Srl
# SPDX-License-Identifier: AGPL-3.0-only

#read project version
$xml = [xml](Get-Content .\src\common.props)
$version = $xml.Project.PropertyGroup[0].Version
Write-Host "Project version: $version"

#admin
Write-Host "Build Docker cv4net-apphero"
docker rmi corsinvest/cv4net-apphero:$version --force
docker build --rm -f "Dockerfile" -t corsinvest/cv4net-apphero:$version "."

#remove unused images
docker image prune -f