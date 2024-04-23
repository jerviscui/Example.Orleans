#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`
GATEWAYPORT=13000

docker build -t silo-host-cluster -f ./ops/Dockerfile-host . &&
  # docker run -it -e ADVERTISEDIP=$ADVERTISEDIP \
  # -p 13000:3000 -p 18080:8080 \
  docker run -it -e ADVERTISEDIP=$ADVERTISEDIP -e GATEWAYPORT=$GATEWAYPORT \
  -p $GATEWAYPORT:$GATEWAYPORT -p 18080:8080 \
  --rm silo-host-cluster
