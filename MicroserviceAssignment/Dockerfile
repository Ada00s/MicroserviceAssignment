#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MicroserviceAssignment/MicroserviceAssignment.csproj", "MicroserviceAssignment/"]
COPY ["CommonLib/CommonLib.csproj", "CommonLib/"]
RUN dotnet restore "MicroserviceAssignment/MicroserviceAssignment.csproj"
COPY . .
WORKDIR "/src/MicroserviceAssignment"
RUN dotnet build "MicroserviceAssignment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroserviceAssignment.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceAssignment.dll"]