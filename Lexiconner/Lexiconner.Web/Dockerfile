FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Lexiconner.Web/Lexiconner.Web.csproj", "Lexiconner.Web/"]
RUN dotnet restore "Lexiconner.Web/Lexiconner.Web.csproj"
COPY . .
WORKDIR "/src/Lexiconner.Web"
RUN dotnet build "Lexiconner.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Lexiconner.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Lexiconner.Web.dll"]
#Because Heroku doesn’t work nicely with ENTRYPOINT command we should change this
CMD dotnet Lexiconner.Web.dll