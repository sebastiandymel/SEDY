FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY ["CardService/CardService.csproj", "CardService/"]
RUN dotnet restore "CardService/CardService.csproj"
COPY . .
WORKDIR "/src/CardService"
RUN dotnet build "CardService.csproj" -c Release -o /app1

# copy and build everything else
FROM build AS publish
RUN dotnet publish "CardService.csproj" -c Release -o /app1

FROM build AS final
WORKDIR /app1
COPY --from=publish /app1 .
ENTRYPOINT ["dotnet", "CardService.dll"]