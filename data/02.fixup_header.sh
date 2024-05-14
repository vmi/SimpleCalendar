#!/bin/bash

file="syukujitsu.csv.header"
if [ ! -f "$file.orig" ]; then
  dos2unix "$file"
  cp -v "$file" "$file.orig"
fi
sed -i.bak -En '
  /^(last-modified|etag|content-length):/p
' "$file"
diff -U0 "$file.orig" "$file"
