#!/bin/bash

apt-get update

# Install java runtime
apt-get -y install --no-install-recommends openjdk-11-jre-headless

# Delete cached files
apt-get clean
rm -r /var/lib/apt/lists/*
