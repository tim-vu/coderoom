#!/bin/bash

apt-get update
apt-get install -y wget

wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

dpkg -i packages-microsoft-prod.deb
rm ./packages-microsoft-prod.deb

# Install .NET core
apt-get update
apt-get install -y --no-install-recommends apt-transport-https
apt-get update
apt-get install -y --no-install-recommends dotnet-sdk-3.1

# Delete cached files
apt-get clean
rm -r /var/lib/apt/lists/*
