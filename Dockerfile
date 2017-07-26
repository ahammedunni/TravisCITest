FROM microsoft/aspnetcore:1.1
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:/TravisCITest/ExpressBase.Web/bin/Release/netcoreapp1.1/} .
ENTRYPOINT ["dotnet", "ExpressBase.Web.dll"]
