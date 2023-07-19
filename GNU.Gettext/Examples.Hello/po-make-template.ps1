# Set-ExecutionPolicy RemoteSigned

# Get the current directory

$currentDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path

$exePath = Join-Path $currentDirectory "..\GNU.Gettext.Xgettext\bin\Debug\net6.0\GNU.Gettext.Xgettext.exe"
$outputPath = Join-Path $currentDirectory "po\Messages.pot"

# Run the command
& $exePath -D $currentDirectory --recursive -o $outputPath

# Return to the original directory
Set-Location -Path $currentDirectory
