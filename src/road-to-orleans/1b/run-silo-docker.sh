#!/bin/bash

docker build -t silo-host-cluster -f ./ops/Dockerfile ./ &&
  docker run -it -p 8080:8080 --rm silo-host-cluster
