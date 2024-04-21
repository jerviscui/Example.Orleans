#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`
SILOPORT=2000
GATEWAYPORT=3000
PRIMARYADDRESS=`RetrieveIp`
PRIMARYPORT=2000
DASHBOARDPORT=8080

docker build -t silo-host-cluster -f ./ops/Dockerfile-host . &&
  docker run -it -e ADVERTISEDIP=$ADVERTISEDIP -e SILOPORT=$SILOPORT -e GATEWAYPORT=$GATEWAYPORT \
    -e PRIMARYADDRESS=$PRIMARYADDRESS -e PRIMARYPORT=$PRIMARYPORT -e DASHBOARDPORT=$DASHBOARDPORT\
    -p $SILOPORT:$SILOPORT -p $GATEWAYPORT:$GATEWAYPORT -p $DASHBOARDPORT:$DASHBOARDPORT --rm silo-host-cluster
