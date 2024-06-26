#!/bin/bash
RetrieveIp(){
  ifconfig | grep -Eo 'inet (addr:)?([0-9]*\.){3}[0-9]*' | grep -Eo '([0-9]*\.){3}[0-9]*' | grep -v -m1 '127.0.0.1'
}

ADVERTISEDIP=`RetrieveIp`
GATEWAYPORT=30002
SILOPORT=11112
DASHBOARDPORT=18082

docker run -d -e ADVERTISEDIP=$ADVERTISEDIP -e GATEWAYPORT=$GATEWAYPORT -e SILOPORT=$SILOPORT \
  -p $GATEWAYPORT:$GATEWAYPORT -p $SILOPORT:$SILOPORT -p $DASHBOARDPORT:8080 \
  silo-host-cluster
