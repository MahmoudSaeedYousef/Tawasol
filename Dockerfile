FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

EXPOSE 7860
ENV ASPNETCORE_URLS=http://+:7860

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Tawasol.API/Tawasol.API.csproj", "Tawasol.API/"]
COPY ["Tawasol.Infrastructure/Tawasol.Infrastructure.csproj", "Tawasol.Infrastructure/"]
COPY ["Tawasol.Application/Tawasol.Application.csproj", "Tawasol.Application/"]
COPY ["Tawasol.Domain/Tawasol.Domain.csproj", "Tawasol.Domain/"]

RUN dotnet restore "Tawasol.API/Tawasol.API.csproj"

COPY . .
WORKDIR "/src/Tawasol.API"
RUN dotnet build "Tawasol.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tawasol.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tawasol.API.dll"]