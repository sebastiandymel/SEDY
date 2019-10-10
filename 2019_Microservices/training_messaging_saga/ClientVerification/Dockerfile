FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY ["ClientVerification/ClientVerification.csproj", "ClientVerification/"]
RUN dotnet restore "ClientVerification/ClientVerification.csproj"
COPY . .
WORKDIR "/src/ClientVerification"
RUN dotnet build "ClientVerification.csproj" -c Release -o /app1

# copy and build everything else
FROM build AS publish
RUN dotnet publish "ClientVerification.csproj" -c Release -o /app1

FROM build AS final
WORKDIR /app1
COPY --from=publish /app1 .
ENTRYPOINT ["dotnet", "ClientVerification.dll"]