# 1. Base image لعمل الـ Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. SDK image لعمل الـ Build والـ Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملفات الـ .csproj أولاً للاستفادة من الـ Caching
COPY ["Tawasol.API/Tawasol.API.csproj", "Tawasol.API/"]
COPY ["Tawasol.Infrastructure/Tawasol.Infrastructure.csproj", "Tawasol.Infrastructure/"]
COPY ["Tawasol.Application/Tawasol.Application.csproj", "Tawasol.Application/"]
COPY ["Tawasol.Domain/Tawasol.Domain.csproj", "Tawasol.Domain/"]

RUN dotnet restore "Tawasol.API/Tawasol.API.csproj"

# نسخ باقي الكود وعمل الـ Build
COPY . .
WORKDIR "/src/Tawasol.API"
RUN dotnet build "Tawasol.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tawasol.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. الصورة النهائية للتشغيل
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tawasol.API.dll"]