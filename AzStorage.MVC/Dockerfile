#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AzStorage.MVC/AzStorage.MVC.csproj", "AzStorage.MVC/"]
RUN dotnet restore "AzStorage.MVC/AzStorage.MVC.csproj"
COPY . .
WORKDIR "/src/AzStorage.MVC"
RUN dotnet build "AzStorage.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AzStorage.MVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzStorage.MVC.dll"]