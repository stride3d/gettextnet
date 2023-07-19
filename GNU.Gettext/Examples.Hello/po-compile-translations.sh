#!bin/sh

# Compile PO files to satellite assemblies
./../../Bin/Debug/GNU.Gettext.Msgfmt -l fr-FR -d ./bin/Debug -r Examples.Hello  -L ./../../Bin/Debug ./po/fr.po
./../../Bin/Debug/GNU.Gettext.Msgfmt -l ru-RU -d ./bin/Debug -r Examples.Hello  -L ./../../Bin/Debug ./po/ru.po
