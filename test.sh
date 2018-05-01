#!/usr/bin/env bash

set -e

MONO_EXE="mono"
NUGET_EXE=".nuget/nuget.exe"
NUGET_URL="https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
NUNIT_RUNNER_EXE="./tools/NUnit.ConsoleRunner.3.6.1/tools/nunit3-console.exe"

# Make sure mono is installed
if [ ! hash $MONO_EXE 2>/dev/null ]; then
    echo "Could not find mono. Exiting" >&2
    exit 1
fi

# Install Nuget if it isn't present
if [ ! -d "$DIRECTORY" ]; then
    mkdir -p .nuget
fi

if [ ! hash $NUGET_EXE 2>/dev/null ]; then
    if hash curl 2>/dev/null; then
        curl -o $NUGET_EXE $NUGET_URL
    elif hash curl 2>/dev/null; then
        wget -O $NUGET_EXE $NUGET_URL
    else
        echo "Could not find curl or wget. Exiting" >&2
        exit 1
    fi
fi

# Restore the nuget packages
mono .nuget/nuget.exe restore phaxio-dotnet.sln

# Install the test runner if it's not present
if [ ! -f $NUNIT_RUNNER_EXE ]; then
    mono .nuget/nuget.exe install NUnit.Runners -Version 3.6.1 -OutputDirectory tools
fi

# Build the project
xbuild /p:Configuration=Release phaxio-dotnet.sln /p:TargetFrameworkVersion="v4.5"

# See if this is an integration test
RUNLIST=""
if [ "$1" = "integration" ]; then
    echo $1
    echo "Running integration tests\n"
    RUNLIST="--testlist=./Phaxio.Tests/IntegrationTestsRunList.txt"
fi

# Run the tests
mono $NUNIT_RUNNER_EXE $RUNLIST ./Phaxio.Tests/bin/Release/Phaxio.Tests.dll
