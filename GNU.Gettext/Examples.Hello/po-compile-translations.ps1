# Set-ExecutionPolicy RemoteSigned

# Get the current directory

$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$currentDirectoryName = Split-Path -Path ./ -Leaf

$catalogs = Get-ChildItem -Path ./po -Filter "*.po"

foreach ($c in $catalogs) {
    & msgfmt-net -l $c.BaseName -d "./bin/Debug/net6.0" -r $currentDirectoryName -L "./bin/Debug/net6.0" --check-format $c.FullName
}
