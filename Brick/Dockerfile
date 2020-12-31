FROM mcr.microsoft.com/dotnet/sdk:5.0.101-alpine3.12 AS builder
WORKDIR /source

# caches restore result by copying csproj file separately
COPY *.csproj .
RUN dotnet restore

# copy the rest of the code
COPY . .
RUN dotnet publish --output /app/ --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0.1-alpine3.12
WORKDIR /app
COPY --from=builder /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Brick.dll"]