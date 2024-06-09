#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TheOmenDen.CrowsOnTheShelf.Api/TheOmenDen.CrowsOnTheShelf.Api.csproj", "TheOmenDen.CrowsOnTheShelf.Api/"]
RUN dotnet restore "./TheOmenDen.CrowsOnTheShelf.Api/TheOmenDen.CrowsOnTheShelf.Api.csproj"
COPY . .
WORKDIR "/src/TheOmenDen.CrowsOnTheShelf.Api"
RUN dotnet build "./TheOmenDen.CrowsOnTheShelf.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TheOmenDen.CrowsOnTheShelf.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheOmenDen.CrowsOnTheShelf.Api.dll"]