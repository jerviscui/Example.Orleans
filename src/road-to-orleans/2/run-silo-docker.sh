#!/bin/bash

docker build -t silo-host-cluster -f ./ops/Dockerfile-host . &&
  docker run -it -p 8080:8080 -p 3000:3000 silo-host-cluster
