FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY ["Basket/Basket.csproj", "Basket/"]
RUN dotnet restore "Basket/Basket.csproj"
COPY . .
WORKDIR "/src/Basket"
RUN dotnet build "Basket.csproj" -c Release -o /app1

# copy and build everything else
FROM build AS publish
RUN dotnet publish "Basket.csproj" -c Release -o /app1

FROM build AS final
WORKDIR /app1
COPY --from=publish /app1 .
ENTRYPOINT ["dotnet", "Basket.dll"]