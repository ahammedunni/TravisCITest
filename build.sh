
#!/bin/bash
set -ev
dotnet restore ./ExpressBase.Web/ExpressBase.Web.sln
dotnet test
dotnet build -c Release
