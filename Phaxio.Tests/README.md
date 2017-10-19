# Testing

## Testing outside Visual Studio

Run ./test.sh

## Integration tests

Integration tests must be run explicitly. To run them, you must first place your API keys in an a file called "keys.ini". (This file will not be checked into Git, as it is ignored). See the sample file keys.ini.sample for an example.

You'll also need to add this to your post-build event in your test project (but don't check it in as it will break Travis):

    if exist "$(ProjectDir)\keys.ini" copy "$(ProjectDir)\keys.ini" "$(TargetDir)"