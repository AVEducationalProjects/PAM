FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["../PAM.UserService/PAM.UserService.csproj", "../PAM.UserService/"]
RUN dotnet restore "../PAM.UserService/PAM.UserService.csproj"
COPY . .
WORKDIR "/src/../PAM.UserService"
RUN dotnet build "PAM.UserService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PAM.UserService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PAM.UserService.dll"]