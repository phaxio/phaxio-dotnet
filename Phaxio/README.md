# Development notes

You need Visual Studio 2015 for the best experience

You need the NUnit 3 VS extension to run the tests.

ThinRestClient could be replaced by RestSharp should it get .NET standard support.

You can use test.sh in the parent directory to run tests.

# Nuget publishing

Run the build command from test.sh, then run:

`mono .nuget/nuget.exe pack Phaxio/Phaxio.csproj -Prop Configuration=Release`

This should create a nuget package that you can then upload at nuget.org.