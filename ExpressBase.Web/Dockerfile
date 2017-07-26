FROM microsoft/aspnetcore:1.1
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/netcoreapp1.1} .
ENTRYPOINT ["dotnet", "ExpressBase.Web.dll"]
