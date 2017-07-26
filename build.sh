
#!/bin/bash
set -ev
dotnet restore ./ExpressBase.Web/ExpressBase.csproj
dotnet build -c Release ./ExpressBase.Web/ExpressBase.csproj
