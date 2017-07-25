
#!/bin/bash
set -ev
dotnet restore ./ExpressBase.Web/ExpressBase.Web.csproj
dotnet test
dotnet build -c Release
