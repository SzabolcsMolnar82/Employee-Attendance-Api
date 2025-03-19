# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

## 1. L�p�s: Haszn�ljuk a hivatalos .NET ASP.NET futtat�k�rnyezetet
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. L�p�s: Build image
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# A projekt f�jl m�sol�sa �s build
COPY ["Employee Attendance Api.csproj", "."]
RUN dotnet restore "./Employee Attendance Api.csproj"


# Teljes forr�sk�d m�sol�sa �s build
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Employee Attendance Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Employee Attendance Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# 3. L�p�s: Runtime kont�ner l�trehoz�sa
# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish 


# Futtat�s.
ENTRYPOINT ["dotnet", "Employee Attendance Api.dll"]