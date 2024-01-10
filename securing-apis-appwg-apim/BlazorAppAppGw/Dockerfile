# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o publish

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /source
COPY --from=build /source/publish ./
ENTRYPOINT ["dotnet", "BlazorAppAppGw.dll"]