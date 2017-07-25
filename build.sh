
#!/bin/bash
set -ev
dotnet restore ./ExpressBase.Web/ExpressBase.Web.csproj
dotnet build -c ./ExpressBase.Web/ExpressBase.Web.csproj Release 
