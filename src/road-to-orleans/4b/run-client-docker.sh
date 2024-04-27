#!/bin/bash

docker build -t client-cluster -f ./ops/Dockerfile-client . &&
  docker run -d --rm client-cluster
