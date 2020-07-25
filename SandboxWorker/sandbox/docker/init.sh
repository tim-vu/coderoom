#!/bin/bash

set -euo pipefail

export DEBIAN_FRONTEND=noninteractive

# Update packages

apt-get update
apt-get -y upgrade
