FROM node:22 AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

RUN npm install -g @angular/cli

COPY . .

RUN npm run build:prod

FROM nginx:1.27-alpine

# Remove default NGINX config (optional)
RUN rm -rf /usr/share/nginx/html/*

# Copy built Angular production files
COPY --from=build /app/dist/ElectronClient/browser /usr/share/nginx/html

EXPOSE 80

# Default NGINX entrypoint
CMD ["nginx", "-g", "daemon off;"]
