
#!/bin/bash
set -ev
dotnet restore ./ExpressBase.Core.sln
dotnet build -c Release ./ExpressBase.Core.sln
