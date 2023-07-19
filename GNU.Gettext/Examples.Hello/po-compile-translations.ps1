# Set-ExecutionPolicy RemoteSigned

# Get the current directory

$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$currentDirectoryName = Split-Path -Path $currentDirectory -Leaf


$exePath = Join-Path $currentDirectory "..\GNU.Gettext.Msgfmt\bin\Debug\net6.0\GNU.Gettext.Msgfmt.exe"
$gettextPath = Join-Path $currentDirectory "..\GNU.Gettext.Msgfmt\bin\Debug\net6.0" | Resolve-Path
$directory = $catalogPath = Join-Path $currentDirectory "bin/Debug/net6.0"
$catalogPath = Join-Path $currentDirectory "po"
$catalogs = Get-ChildItem -Path $catalogPath -Filter "*.po"


foreach ($c in $catalogs) {
    & $exePath -l $c.BaseName -d $directory -r ($currentDirectoryName).Message -L $gettextPath --check-format $c.FullName
}

