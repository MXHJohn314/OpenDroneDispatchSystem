FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 85
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SimDrone/SimDrone.csproj", "SimDrone/"]
RUN dotnet restore "SimDrone/SimDrone.csproj"
COPY . .
WORKDIR "/src/SimDrone"
RUN dotnet build "SimDrone.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimDrone.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimDrone.dll"]
