#!/bin/bash

spf=structured_programming_in_fsharp
git clone --depth 1 "https://github.com/lamg/$spf" -o $spf
dotnet run --project web_generator/Cli $spf
rm -rf $spf