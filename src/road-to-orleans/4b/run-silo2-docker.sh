#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`
GATEWAYPORT=40001
SILOPORT=21111
DASHBOARDPORT=28081

docker build -t silo2-host-cluster -f ./ops/Dockerfile-host2 . &&
  docker run -d -e ADVERTISEDIP=$ADVERTISEDIP -e GATEWAYPORT=$GATEWAYPORT -e SILOPORT=$SILOPORT \
  -p $GATEWAYPORT:$GATEWAYPORT -p $SILOPORT:$SILOPORT -p $DASHBOARDPORT:28080 \
  silo2-host-cluster
