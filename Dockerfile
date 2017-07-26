FROM microsoft/aspnetcore:1.1
WORKDIR /app
EXPOSE 80
COPY /home/travis/build/[secure]/TravisCITest/ExpressBase.Web/bin/Release/netcoreapp1.1/ .
ENTRYPOINT ["dotnet", "ExpressBase.Web.dll"]
