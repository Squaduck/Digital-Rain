declare -a rids=("linux-x64" "win-x64") # The RIDS to build. Should be able to add more as needed.

cd "$(dirname "$0")" # cd to script directory.
rm -r ./release
mkdir ./release

for rid in "${rids[@]}"
do
    echo "Preparing RID $rid:"
    dotnet publish -r $rid
    exe=$(find ./bin/Release/net8.0/$rid/publish/ -mindepth 1) # There should only be one file, so find it :) mindepth prevents this from returning the dir itself.
    ext=$(echo $exe | grep -Eo '\.[^\./]+$') # Get the file extension, if present.
    copy="./release/DigitalRain-$rid""$ext" # Location to copy build to.
    cp "$exe" $copy

    7z a $copy.zip $copy # ZIP the executable
    rm $copy # Delete after zipping.
done