#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`

docker build -t silo-host-cluster -f ./ops/Dockerfile-host . &&
  docker run -it -e ADVERTISEDIP=$ADVERTISEDIP \
  -p 3000:3000 -p 8080:8080 \
  --rm silo-host-cluster
