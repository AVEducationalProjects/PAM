FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["PAM.AssetService/PAM.AssetService.csproj", "PAM.AssetService/"]
RUN dotnet restore "PAM.AssetService/PAM.AssetService.csproj"
COPY . .
WORKDIR "/src/PAM.AssetService"
RUN dotnet build "PAM.AssetService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PAM.AssetService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PAM.AssetService.dll"]