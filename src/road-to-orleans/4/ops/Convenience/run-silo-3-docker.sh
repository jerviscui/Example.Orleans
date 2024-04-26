#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`
GATEWAYPORT=30003
SILOPORT=11113
PRIMARYADDRESS=`RetrieveIp`
PRIMARYPORT=11111
DASHBOARDPORT=18083

docker run -d -e ADVERTISEDIP=$ADVERTISEDIP -e GATEWAYPORT=$GATEWAYPORT -e SILOPORT=$SILOPORT \
  -e PRIMARYADDRESS=$PRIMARYADDRESS -e PRIMARYPORT=$PRIMARYPORT \
  -p $GATEWAYPORT:$GATEWAYPORT -p $SILOPORT:$SILOPORT -p $DASHBOARDPORT:8080 \
  --rm silo-host-cluster