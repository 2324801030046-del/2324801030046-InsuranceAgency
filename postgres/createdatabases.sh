#!/bin/sh
set -eu

: "${PRICING_DB_PASSWORD:?Set PRICING_DB_PASSWORD for the pricing database user}"

psql --set=ON_ERROR_STOP=1 --set=db_password="$PRICING_DB_PASSWORD" --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<'SQL'
CREATE USER lab_user WITH ENCRYPTED PASSWORD :'db_password';

CREATE DATABASE lab_netmicro_payments;
CREATE DATABASE lab_netmicro_jobs;
CREATE DATABASE lab_netmicro_policy;
CREATE DATABASE lab_netmicro_pricing;

GRANT ALL PRIVILEGES ON DATABASE lab_netmicro_payments TO lab_user;
GRANT ALL PRIVILEGES ON DATABASE lab_netmicro_jobs TO lab_user;
GRANT ALL PRIVILEGES ON DATABASE lab_netmicro_policy TO lab_user;
GRANT ALL PRIVILEGES ON DATABASE lab_netmicro_pricing TO lab_user;
SQL
