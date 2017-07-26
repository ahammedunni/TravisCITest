FROM microsoft/aspnetcore:1.1
WORKDIR /app
EXPOSE 80
COPY ./TravisCITest/ExpressBase.Web/bin/Release/netcoreapp1.1/ .
ENTRYPOINT ["dotnet", "ExpressBase.Web.dll"]
