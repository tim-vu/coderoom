#!/bin/bash

# Install Python3 runtime
apt-get update
apt-get -y install --no-install-recommends python3

# Delete cached files
apt-get clean
rm -r /var/lib/apt/lists/*
