FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 51065
EXPOSE 44395

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Backend-API/Backend-API.csproj", "Backend-API/"]
RUN dotnet restore "Backend-API/Backend-API.csproj"
COPY . .
WORKDIR "/src/Backend-API"
RUN dotnet build "Backend-API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Backend-API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Backend-API.dll"]