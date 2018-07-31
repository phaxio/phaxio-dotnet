#!/usr/bin/env bash

set -e

NET_EXE="mono"
DOTNET_CLI_EXE="dotnet"
NUGET_EXE=".nuget/nuget.exe"
NUGET_URL="https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
NUNIT_RUNNER_EXE="./tools/NUnit.ConsoleRunner.3.6.1/tools/nunit3-console.exe"
BUILD_EXE="msbuild"

# Make sure mono is installed
if [ ! hash $NET_EXE 2>/dev/null ]; then
    echo "Could not find $NET_EXE. Exiting" >&2
    exit 1
fi

# Make sure dotnet (the CLI) is installed
if [ ! hash $DOTNET_CLI_EXE 2>/dev/null ]; then
    echo "Could not find $DOTNET_CLI_EXE. Exiting" >&2
    exit 1
fi

# Install Nuget if it isn't present
if [ ! -d "$DIRECTORY" ]; then
    mkdir -p .nuget
fi

if [ ! -f $NUGET_EXE 2>/dev/null ]; then
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
$NET_EXE $NUGET_EXE restore phaxio-dotnet.sln

# Install the test runner if it's not present
if [ ! -f $NUNIT_RUNNER_EXE ]; then
    $NET_EXE .$NUGET_EXE install NUnit.Runners -Version 3.6.1 -OutputDirectory tools
fi

# Build the project
$BUILD_EXE phaxio-dotnet.sln /p:Configuration=Release /verbosity:quiet

# See if this is an integration test
RUNLIST=""
if [ "$1" = "integration" ]; then
    echo $1
    echo "Running integration tests\n"
    RUNLIST="--testlist=./Phaxio.Tests/IntegrationTestsRunList.txt"
fi

# See if this is an integration test
RUNLIST=""
if [ "$1" = "integration" ]; then
    echo $1
    echo "Running integration tests\n"
    RUNLIST="--testlist=./Phaxio.Tests/IntegrationTestsRunList.txt"
fi

# Run the tests
$NET_EXE $NUNIT_RUNNER_EXE $RUNLIST ./Phaxio.Tests/bin/Release/net45/Phaxio.Tests.dll
$DOTNET_CLI_EXE test --framework netcoreapp2.0 ./Phaxio.Tests/Phaxio.Tests.csproj
