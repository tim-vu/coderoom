mkdir public
mkdir public/app

npm install --prefix App
npm run --prefix App build

cp -r ./App/build/* ./public/app/

export NODE_ENV=production

npm install --prefix Landing
npm run --prefix Landing build

cp -r ./Landing/build/* ./public/

cp ./_redirects ./public/
