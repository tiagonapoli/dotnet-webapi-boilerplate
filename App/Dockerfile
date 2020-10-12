FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env

WORKDIR /app

COPY *.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

RUN apk add --no-cache tini

# OpenTelemetry's GRPC needs the following lines to work
# https://github.com/open-telemetry/opentelemetry-dotnet/issues/1251 
RUN echo 'http://dl-cdn.alpinelinux.org/alpine/v3.8/main' >> /etc/apk/repositories
RUN apk update && apk add --no-cache bash libc6-compat=1.1.19-r11

ENV ASPNETCORE_ENVIRONMENT="Production"

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["/sbin/tini", "--"]

CMD ["dotnet", "App.dll"]