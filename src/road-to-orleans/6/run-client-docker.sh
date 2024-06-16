#!/bin/bash

docker build -t client-cluster -f ./ops/Dockerfile-client . &&
  docker run -d -p 5000:5000 client-cluster
