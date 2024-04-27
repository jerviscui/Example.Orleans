#!/bin/bash

docker build -t client-cluster -f ./ops/Dockerfile-client . &&
  docker run -d client-cluster
