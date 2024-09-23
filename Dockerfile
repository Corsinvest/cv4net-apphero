# SPDX-FileCopyrightText: Copyright Corsinvest Srl
# SPDX-License-Identifier: AGPL-3.0-only

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443
VOLUME ["/app/data"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/", "src/"]
RUN dotnet restore "src/Corsinvest.AppHero.AppBss/Corsinvest.AppHero.AppBss.csproj"
RUN dotnet build "src/Corsinvest.AppHero.AppBss/Corsinvest.AppHero.AppBss.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Corsinvest.AppHero.AppBss/Corsinvest.AppHero.AppBss.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Corsinvest.AppHero.AppBss.dll"]