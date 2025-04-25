# Project Setup

### Install Docker

https://docs.docker.com/get-started/get-docker/

## For Deployment

```sh
docker compose up --build
```

## For Development

### Install dependencies

```sh
npm install
```

### Build and start db container

```sh
docker compose build
```

```sh
docker start net-core-react-postgres-chatapp-db-1
```

### Start the front and backend dev servers

```sh
npm run dev
```
