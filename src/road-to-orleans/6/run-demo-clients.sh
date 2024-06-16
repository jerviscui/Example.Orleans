#!/bin/bash
set -e

./run-client-docker.sh
./ops/Convenience/run-demo-client-docker.sh &
./ops/Convenience/run-demo-client-docker.sh &
./ops/Convenience/run-demo-client-docker.sh &
./ops/Convenience/run-demo-client-docker.sh &
wait
