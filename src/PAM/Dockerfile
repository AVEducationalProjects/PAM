FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY PAM/PAM.csproj PAM/
RUN dotnet restore PAM/PAM.csproj
COPY . .
WORKDIR /src/PAM
RUN dotnet build PAM.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish PAM.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PAM.dll"]
