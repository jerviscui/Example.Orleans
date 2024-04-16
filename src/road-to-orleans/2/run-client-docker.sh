#!/bin/bash

docker build -t silo-client -f ./ops/Dockerfile-client . &&
  docker run -it silo-client
