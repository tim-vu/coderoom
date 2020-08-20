#!/bin/bash

mkdir public
mkdir public/app

npm install --prefix App
npm run --prefix App build; exit_1=$?

cp -r ./App/build/* ./public/app/

npm install --prefix Landing
npm install --prefix Landing --only=dev 
npm run --prefix Landing --production build; exit_2=$?

cp -r ./Landing/build/* ./public/

cp ./_redirects ./public/

! (( $exit_1 || $exit_2 ))
