FROM mcr.microsoft.com/dotnet/nightly/sdk:8.0-alpine-aot AS build
WORKDIR /src
COPY ["QP.Storage.WebApp.csproj", ""]
RUN dotnet restore -r linux-musl-x64
COPY . .
RUN dotnet publish -c Release -r linux-musl-x64 -o /app/publish --self-contained=true --no-restore

FROM alpine:3.18 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["./QP.Storage.WebApp", "--urls", "http://*:80"]