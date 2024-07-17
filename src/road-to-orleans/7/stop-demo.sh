#!/bin/bash
echo stop
docker ps | grep client-cluster | awk '{print $1}' | xargs -r docker stop
docker ps | grep silo-host-cluster | awk '{print $1}' | xargs -r docker stop
docker ps | grep silo2-host-cluster | awk '{print $1}' | xargs -r docker stop
echo rm
docker ps -a | grep client-cluster | awk '{print $1}' | xargs -r docker rm
docker ps -a | grep silo-host-cluster | awk '{print $1}' | xargs -r docker rm
docker ps -a | grep silo2-host-cluster | awk '{print $1}' | xargs -r docker rm
