#!/bin/bash
echo "Waiting 5 seconds to start Liquibase"

sleep 5;

export PATH=$PATH:/usr/local/bin

echo "Liquibase starting..."

liquibase update