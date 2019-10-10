FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY ["Client.csproj", "Client/"]
RUN dotnet restore "Client/Client.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Client.csproj" -c Release -o /app1

# copy and build everything else
FROM build AS publish
RUN dotnet publish "Client.csproj" -c Release -o /app1

FROM build AS final
WORKDIR /app1
COPY --from=publish /app1 .
ENTRYPOINT ["dotnet", "Client.dll"]