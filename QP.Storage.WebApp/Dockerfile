FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QP.Storage.WebApp.csproj", ""]
RUN dotnet restore
COPY . .
RUN dotnet publish "QP.Storage.WebApp.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "./QP.Storage.WebApp", "--urls", "http://*:80"]
