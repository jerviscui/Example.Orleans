#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`
GATEWAYPORT=30000

docker build -t silo-client -f ./ops/Dockerfile-client . &&
  docker run -it -e ADVERTISEDIP=$ADVERTISEDIP -e GATEWAYPORT=$GATEWAYPORT --rm silo-client
