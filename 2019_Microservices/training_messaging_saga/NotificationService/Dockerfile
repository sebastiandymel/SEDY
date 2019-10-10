FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY ["NotificationService/NotificationService.csproj", "NotificationService/"]
RUN dotnet restore "NotificationService/NotificationService.csproj"
COPY . .
WORKDIR "/src/NotificationService"
RUN dotnet build "NotificationService.csproj" -c Release -o /app1

# copy and build everything else
FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c Release -o /app1

FROM build AS final
WORKDIR /app1
COPY --from=publish /app1 .
ENTRYPOINT ["dotnet", "NotificationService.dll"]