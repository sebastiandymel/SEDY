FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY ["Kitchen/Kitchen.csproj", "Kitchen/"]
RUN dotnet restore "Kitchen/Kitchen.csproj"
COPY . .
WORKDIR "/src/Kitchen"
RUN dotnet build "Kitchen.csproj" -c Release -o /app1

# copy and build everything else
FROM build AS publish
RUN dotnet publish "Kitchen.csproj" -c Release -o /app1

FROM build AS final
WORKDIR /app1
COPY --from=publish /app1 .
ENTRYPOINT ["dotnet", "Kitchen.dll"]