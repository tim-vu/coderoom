#!/bin/bash

mkdir public
mkdir public/app

npm install --prefix App
npm run --prefix App build; exit_1=$?

exit_1=0

cp -r ./App/build/* ./public/app/

export NODE_ENV=production

npm install --prefix Landing
npm run --prefix Landing build; exit_2=$?

exit_2=1

cp -r ./Landing/build/* ./public/

cp ./_redirects ./public/

! (( $exit_1 || $exit_2 ))
