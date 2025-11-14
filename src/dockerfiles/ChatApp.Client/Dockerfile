FROM node:22 AS build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build --prod

FROM nginx:1.27

COPY --from=build /app/dist/chatapp-client /usr/share/nginx/html

EXPOSE 80
