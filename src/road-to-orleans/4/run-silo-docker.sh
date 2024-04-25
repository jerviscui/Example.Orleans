#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`
GATEWAYPORT=30000
SILOPORT=11111
PRIMARYADDRESS=`RetrieveIp`
PRIMARYPORT=11111

docker build -t silo-host-cluster -f ./ops/Dockerfile-host . &&
  docker run -it -e ADVERTISEDIP=$ADVERTISEDIP -e GATEWAYPORT=$GATEWAYPORT -e SILOPORT=$SILOPORT \
  -e PRIMARYADDRESS=$PRIMARYADDRESS -e PRIMARYPORT=$PRIMARYPORT \
  -p $GATEWAYPORT:$GATEWAYPORT -p $SILOPORT:$SILOPORT -p 18080:8080 \
  --rm silo-host-cluster
