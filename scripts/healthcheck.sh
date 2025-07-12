#!/usr/bin/env bash

#web checking 
WEB_STATUS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8000/)
if [ "$WEB_STATUS" -ne 200 ]; then
  echo "ERROR: web is down (status $WEB_STATUS)"
  exit 1
fi

#postgress checking
docker-compose exec -T db pg_isready -U postgres >/dev/null 2>&1
if [ $? -ne 0 ]; then
  echo "ERROR: postgres is not ready"
  exit 1
fi

echo "OK: all services are healthy"
exit 0
