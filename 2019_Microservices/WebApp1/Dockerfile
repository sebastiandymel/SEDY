FROM microsoft/dotnet:2.2-sdk AS build_env
COPY WebApp1/ App
WORKDIR /App
RUN dotnet restore WebApp1.csproj
RUN ls
RUN dotnet publish "WebApp1.csproj" -c Release -o publish
RUN ls 

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR  /App
COPY --from=build_env /App/publish .
RUN ls
EXPOSE 80
ENTRYPOINT ["dotnet", "WebApp1.dll"]