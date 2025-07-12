#!/usr/bin/env bash

LOG_DIR="./logs"
ARCHIVE_DIR="$LOG_DIR/archive"
MAX_ARCHIVES=5

mkdir -p "$ARCHIVE_DIR"
timestamp=$(date +"%Y%m%d_%H%M%S")


tar czf "$ARCHIVE_DIR/logs_$timestamp.tar.gz" \
    -C "$LOG_DIR" \
    --exclude="archive" \
    .


cd "$ARCHIVE_DIR"
ls -1t *.tar.gz | tail -n +$((MAX_ARCHIVES + 1)) | xargs -r rm
