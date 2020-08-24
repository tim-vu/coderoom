#!/bin/bash

wget https://deb.nodesource.com/setup_12.x -O nodesource_setup.sh
bash nodesource_setup.sh

# Install NodeJs
apt-get -y install --no-install-recommends  nodejs

# Delete cached files
apt-get clean
rm -r /var/lib/apt/lists/*
