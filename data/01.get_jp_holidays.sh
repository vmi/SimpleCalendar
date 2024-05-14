#!/bin/bash

curl -D syukujitsu.csv.header -LO https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv
dos2unix syukujitsu.csv.header
rm -f syukujitsu.csv.header.orig
