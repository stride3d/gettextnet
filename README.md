# gettextnet

A .NET port of [GNU gettext](https://www.gnu.org/software/gettext/) for internationalization (i18n) and localization (l10n). Provides a runtime library for loading `.po`/`.mo` translations and CLI tools to extract strings from C# source and compile `.po` files to `.mo` binaries.

Originally forked from the Mono project. Published on NuGet under the `Stride.*` namespace.

Requires **.NET 10** or later.

## Packages

| Package | NuGet ID | Version | Description |
|---------|----------|---------|-------------|
| Core library | `Stride.GNU.Gettext` | 3.0.0 | Runtime API for loading translations |
| Argument parser | `Stride.GNU.Getopt` | 3.0.0 | POSIX-compatible CLI argument parsing |
| msgfmt tool | `GNU.Gettext.Msgfmt` | — | Compiles `.po` → satellite assemblies |
| xgettext tool | `GNU.Gettext.Xgettext` | — | Extracts translatable strings from C# |

## Usage

### Runtime (library)

```csharp
using GNU.Gettext;

var catalog = new GettextResourceManager();

// Simple string
catalog.GetString("Hello, world!")

// With format arguments
catalog.GetStringFmt("Running as process {0}.", Environment.ProcessId)

// Plural form
catalog.GetPluralString("found {0} similar word", "found {0} similar words", count)

// Context-disambiguated string
catalog.GetParticularString("Computers", "Text encoding")
```

Translations are loaded from satellite assemblies (`.resources.dll`) following standard .NET resource fallback. The resource base name must end in `.Messages` (e.g. `MyApp.Messages`).

### Workflow

1. **Extract** translatable strings from C# source into a `.pot` template:
   ```
   xgettext-net -D src/ --recursive -o messages.pot
   ```

2. Create per-language `.po` files from the template and translate them.

3. **Compile** `.po` files to satellite assemblies:
   ```
   msgfmt-net -r MyApp.Messages -l fr-FR -d out/ fr.po
   ```

4. Deploy the satellite assemblies alongside your application.

## Tools

### `msgfmt-net`

```
Usage: msgfmt-net [OPTIONS] filename.po ...

  -r resource     Base resource name (e.g. MyApp.Messages — suffix added automatically)
  -o file         Output .resources file (ignored when -d is used)
  -d directory    Output directory; locale subdirectory is created automatically
  -l locale       .NET culture name (e.g. fr-FR, ru)
  -L path         Path to GNU.Gettext.dll
  --check-format  Validate C# format strings
  --csharp-resources  Produce a .resources file instead of a satellite assembly
  -h, --help
```

### `xgettext-net`

```
Usage: xgettext-net [options] [inputfile | filemask] ...

  -f file         Read input filenames from file
  -o file         Output POT file (default: messages.pot)
  -D directory    Input directory (repeatable)
  --recursive     Process subdirectories
  --from-code     Input encoding (default: UTF-8)
  -j              Join with existing output file
  -v, --verbose
  -h, --help
```

## Building

```bash
cd GNU.Gettext
dotnet build GNU.Gettext.sln
dotnet test GNU.Gettext.Test/GNU.Gettext.Test.csproj
```

Pack the tools as NuGet packages:
```bash
dotnet pack GNU.Gettext.Msgfmt/GNU.Gettext.Msgfmt.csproj -o ../nupkg
dotnet pack GNU.Gettext.Xgettext/GNU.Gettext.Xgettext.csproj -o ../nupkg
```

## License

- Library (`GNU.Gettext.dll`), tools, and `GNU.Getopt.dll`: **LGPL v2.1 or later**
- Documentation: **GPL v3 or later**

See [COPYING.txt](COPYING.txt) for details.
