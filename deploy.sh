#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3

# Create publish artifact
dotnet publish -c Release src

# Build the Docker images
docker build -t ahammedunni/expressbase:$TAG src/bin/Release/netcoreapp1.1/publish/.
docker tag ahammedunni/expressbase:$TAG ahammedunni/expressbase:latest

# Login to Docker Hub and upload images
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push ahammedunni/expressbase:$TAG
docker push ahammedunni/expressbase:latest