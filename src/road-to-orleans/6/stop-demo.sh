#!/bin/bash
docker ps -a | grep client-cluster | awk '{print $1}' | xargs -I {} docker stop {} | xargs -I {} docker rm -f {}
docker ps -a | grep silo-host-cluster | awk '{print $1}' | xargs -I {} docker stop {} | xargs -I {} docker rm -f {}
docker ps -a | grep silo2-host-cluster | awk '{print $1}' | xargs -I {} docker stop {} | xargs -I {} docker rm -f {}
