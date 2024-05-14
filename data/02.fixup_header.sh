#!/bin/bash

file="syukujitsu.csv.header"
if [ ! -f "$file.orig" ]; then
  cp -v "$file" "$file.orig"
fi
awk '
  $1 == "last-modified:"  { $1 = "Last-Modified:"  ; print }
  $1 == "content-length:" { $1 = "Content-Length:" ; print }
' "$file.orig" > "$file"
diff -i -u "$file.orig" "$file"
