FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ServiceApi.csproj", "ServiceApi/"]
RUN dotnet restore "ServiceApi/ServiceApi.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ServiceApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ServiceApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ServiceApi.dll"]