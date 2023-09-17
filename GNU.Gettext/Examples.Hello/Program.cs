// Example for use of GNU gettext.
// This file is in the public domain.
//
// Source code of the C# program.

using System;
using System.Globalization;
using System.Threading;
using GNU.Gettext;

Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
ShowMessages();
Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
ShowMessages();
Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
ShowMessages();


void ShowMessages()
{
    Console.WriteLine("===[ Current culture {0} ]===", Thread.CurrentThread.CurrentUICulture);
    GettextResourceManager catalog = new GettextResourceManager();
    Console.WriteLine(catalog.GetString("Hello, world!"));
    // GetStringFmt is an Gettext.NET extension
    Console.WriteLine(catalog.GetStringFmt("This program is running as process number \"{0}\".",
                      Environment.ProcessId));
    Console.WriteLine(string.Format(
        catalog.GetPluralString("found {0} similar word", "found {0} similar words", 1),
        1));
    // GetPluralStringFmt is an Gettext.NET extension
    Console.WriteLine(catalog.GetPluralStringFmt("found {0} similar word", "found {0} similar words", 2));
    Console.WriteLine(string.Format(
        catalog.GetPluralString("found {0} similar word", "found {0} similar words", 5),
        5));

    Console.WriteLine("{0} ('computers')", catalog.GetParticularString("Computers", "Text encoding"));
    Console.WriteLine("{0} ('military')", catalog.GetParticularString("Military", "Text encoding"));
    Console.WriteLine("{0} (non cotextual)", catalog.GetString("Text encoding"));

    Console.WriteLine(catalog.GetString(
        "Here is an example of how one might continue a very long string\nfor the common case the string represents multi-line output.\n"));
}
