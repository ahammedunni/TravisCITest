#!/bin/bash
set -ev


DOCKER_USERNAME=$1
DOCKER_PASSWORD=$2

# Create publish artifact
dotnet publish -c Release ./ExpressBase.Web/ExpressBase.Web.csproj

# Build the Docker images
docker build -t ahammedunni/expressbase ExpressBase.Web/bin/Release/netcoreapp1.1/publish/.
docker tag ahammedunni/expressbase ahammedunni/expressbase:latest

# Login to Docker Hub and upload images
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push ahammedunni/expressbase:latest
