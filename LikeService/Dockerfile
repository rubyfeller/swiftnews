FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LikeService.csproj", "./"]
RUN dotnet restore "./LikeService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LikeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LikeService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LikeService.dll"]