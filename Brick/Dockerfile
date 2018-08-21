FROM microsoft/dotnet:2.1-sdk AS builder
WORKDIR /source

# caches restore result by copying csproj file separately
COPY *.csproj .
RUN dotnet restore

# copy the rest of the code
COPY . .
RUN dotnet publish --output /app/ --configuration Release

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=builder /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Brick.dll"]