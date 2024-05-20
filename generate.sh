#!/bin/bash

rm -r docs
spf=structured_programming_in_fsharp
wget "https://github.com/lamg/$spf/archive/refs/heads/master.zip"
unzip master.zip
mv "$spf-master" docs
dotnet tool restore
dotnet fsdocs build
rm -rf docs
rm master.zip
mv output docs
rm docs/Dockerfile
rm docs/NuGet.config