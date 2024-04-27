#!/bin/bash
./run-silo-docker.sh
./ops/Convenience/run-silo-2-docker.sh &
./ops/Convenience/run-silo-3-docker.sh &
wait
