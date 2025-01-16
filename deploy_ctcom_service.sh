#!/bin/bash

# Define variables
IMAGE_TAG="v1"
IMAGE_NAME="ctcom.product-service:$IMAGE_TAG"
CONTAINER_NAME="ctcom.product-service"
HOST_PORT=7128
CONTAINER_PORT=7128
UPLOADS_VOLUME="uploads_volume"

# build image
docker build -t ctcom.product-service:v1 . 

# Stop and remove any existing container with the same name
docker stop $CONTAINER_NAME || true
docker rm $CONTAINER_NAME || true

# Run the container with specified settings
docker run -d --name $CONTAINER_NAME \
  -p $HOST_PORT:$CONTAINER_PORT \
  -v $UPLOADS_VOLUME:/app/wwwroot/uploads \
  $IMAGE_NAME

# Create and connect networks
NETWORKS=("gateway" "redis" "sql-server")

for NETWORK in "${NETWORKS[@]}"; do
  docker network create --driver bridge $NETWORK || true
  docker network connect $NETWORK $CONTAINER_NAME
done

echo "Deployment of $CONTAINER_NAME completed successfully."



#chmod +x deploy_ctcom_service.sh
#./deploy_ctcom_service.sh
