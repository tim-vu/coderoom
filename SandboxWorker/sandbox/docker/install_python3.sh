#!/bin/bash

apt-get update

# Install Python3 runtime
apt-get -y install --no-install-recommends python3

# Delete cached files
apt-get clean
rm -r /var/lib/apt/lists/*
