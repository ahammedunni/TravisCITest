
#!/bin/bash
set -ev
dotnet restore ./ExpresBase.Web/ExpressBase.Web.csproj
dotnet build -c Release
