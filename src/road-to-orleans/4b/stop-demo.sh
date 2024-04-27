#!/bin/bash
docker ps | grep client-cluster | awk '{print $1}' | xargs -I {} docker rm -f {}
docker ps | grep silo-host-cluster | awk '{print $1}' | xargs -I {} docker rm -f {}
docker ps | grep silo2-host-cluster | awk '{print $1}' | xargs -I {} docker rm -f {}
