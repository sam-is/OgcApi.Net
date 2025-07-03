#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../SampleWebApplication/SampleWebApplication.csproj", "samples/SampleWebApplication/"]
COPY ["../../src/Providers/OgcApi.Net.MbTiles/OgcApi.Net.MbTiles.csproj", "src/Providers/OgcApi.Net.MbTiles/"]
COPY ["../../src/Common/OgcApi.Net/OgcApi.Net.csproj", "src/Common/OgcApi.Net/"]
COPY ["../../src/Providers/OgcApi.Net.SqlServer/OgcApi.Net.SqlServer.csproj", "src/Providers/OgcApi.Net.SqlServer/"]
COPY ["../../src/Common/Standards/OgcApi.Net.Schemas/OgcApi.Net.Schemas.csproj", "src/Common/Standards/OgcApi.Net.Schemas/"]
RUN dotnet restore "./samples/SampleWebApplication/SampleWebApplication.csproj"
COPY . .
WORKDIR "/src/samples/SampleWebApplication"
RUN dotnet build "./SampleWebApplication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SampleWebApplication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleWebApplication.dll"]