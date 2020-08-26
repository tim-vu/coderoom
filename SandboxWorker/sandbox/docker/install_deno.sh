#!/bin/bash

apt-get update
apt-get install curl -y --no-install-recommends apt-transport-https
apt-get install unzip -y --no-install-recommends apt-transport-https

# Install deno
curl -fsSL https://deno.land/x/install/install.sh | sh

ln ~/.deno/bin/deno /usr/bin/deno

# Delete cached files
apt-get clean
rm -r /var/lib/apt/lists/*