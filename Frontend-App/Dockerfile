FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50108
EXPOSE 44380

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Frontend-App/Frontend-App.csproj", "Frontend-App/"]
RUN dotnet restore "Frontend-App/Frontend-App.csproj"
COPY . .
WORKDIR "/src/Frontend-App"
RUN dotnet build "Frontend-App.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Frontend-App.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Frontend-App.dll"]