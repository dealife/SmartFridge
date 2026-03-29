# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY *.sln .
COPY *.csproj .
RUN dotnet restore

# Копируем всё остальное и собираем
COPY . .
RUN dotnet publish -c Release -o out

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Открываем порт, который обычно использует Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "SmartFridge.Api.dll"]