#!!!this dockerfile is changed manually
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 8080 #heroku fails on 80
ENV ASPNETCORE_URLS=http://*:8080

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Lexiconner/Lexiconner.Web/Lexiconner.Web.csproj", "Lexiconner.Web/"]
RUN dotnet restore "Lexiconner.Web/Lexiconner.Web.csproj"
WORKDIR "/src/Lexiconner.Web"
COPY "Lexiconner/Lexiconner.Web/" .
RUN dotnet build "Lexiconner.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Lexiconner.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Lexiconner.Web.dll"]
#Because Heroku doesn’t work nicely with ENTRYPOINT command we should change this
CMD dotnet Lexiconner.Web.dll