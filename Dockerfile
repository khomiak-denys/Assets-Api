# Крок 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копіюємо проєкт
COPY *.sln .
COPY Assets_Api/*.csproj ./Assets_Api/
RUN dotnet restore

# Копіюємо решту файлів та будуємо
COPY Assets_Api/. ./Assets_Api/
WORKDIR /app/Assets_Api
RUN dotnet publish -c Release -o out

# Крок 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/Assets_Api/out ./
ENTRYPOINT ["dotnet", "Assets_Api.dll"]
