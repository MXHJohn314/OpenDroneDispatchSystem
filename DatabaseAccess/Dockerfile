FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 85
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DatabaseAccess/DatabaseAccess.csproj", "DatabaseAccess/"]
RUN dotnet restore "DatabaseAccess/DatabaseAccess.csproj"
COPY . .
WORKDIR "/src/DatabaseAccess"
RUN dotnet build "DatabaseAccess.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseAccess.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseAccess.dll"]
