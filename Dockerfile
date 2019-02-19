FROM microsoft/dotnet:2.2-aspnetcore-build AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["PriceWatcher.Core/PriceWatcher.Core.csproj", "PriceWatcher.Core/"]
RUN dotnet restore "PriceWatcher.Core/PriceWatcher.Core.csproj"
COPY . .
WORKDIR "/src/PriceWatcher.Core"
RUN dotnet build "PriceWatcher.Core.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PriceWatcher.Core.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PriceWatcher.Core.dll"]