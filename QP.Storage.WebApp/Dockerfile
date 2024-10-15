FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
RUN apk update && apk upgrade
RUN apk add --no-cache clang build-base zlib-dev
WORKDIR /src
COPY ["QP.Storage.WebApp.csproj", ""]
RUN dotnet restore -r linux-musl-x64
COPY . .
RUN dotnet publish -c Release -r linux-musl-x64 -o /app/publish --self-contained=true --no-restore

FROM alpine:3 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["./QP.Storage.WebApp", "--urls", "http://*:80"]