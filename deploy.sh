#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3  

# Create publish artifact
dotnet publish -c Release -o ./src ./ExpressBase.Web/ExpressBase.Web.csproj

# Build the Docker images
docker build -t ahammedunni/expressbaseweb:$TAG ./ExpressBase.Web/.
docker tag ahammedunni/expressbaseweb:$TAG ahammedunni/expressbase:latest

# Login to Docker Hub and upload images
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push ahammedunni/expressbaseweb:$TAG
docker push ahammedunni/expressbaseweb:latest
