using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GNU.Gettext.Xgettext;

public partial class ExtractorCsharp
{
    const string CsharpStringPatternExplained = @"
			(\w+)\s*=\s*    # key =
			(               # Capturing group for the string
			    @""               # verbatim string - match literal at-sign and a quote
			    (?:
			        [^""]|""""    # match a non-quote character, or two quotes
			    )*                # zero times or more
			    ""                #literal quote
			|               #OR - regular string
			    ""              # string literal - opening quote
			    (?:
			        \\.         # match an escaped character,
			        |[^\\""]    # or a character that isn't a quote or a backslash
			    )*              # a few times
			    ""              # string literal - closing quote
			)";

    const string CsharpStringPattern = @"(@""(?:[^""]|"""")*""|""(?:\\.|[^\\""])*"")";
    const string ConcatenatedStringsPattern = @"((@""(?:[^""]|"""")*""|""(?:\\.|[^\\""])*"")\s*(?:\+|;|,|\))\s*){2,}";
    const string TwoStringsArgumentsPattern = CsharpStringPattern + @"\s*,\s*" + CsharpStringPattern;
    const string ThreeStringsArgumentsPattern = TwoStringsArgumentsPattern + @"\s*,\s*" + CsharpStringPattern;

    // Full composite patterns for source-generated regex
    const string GetStringFullPattern = @"GetString\s*\(\s*" + CsharpStringPattern;
    const string GetStringFmtFullPattern = @"GetStringFmt\s*\(\s*" + CsharpStringPattern;
    const string GetPluralStringFullPattern = @"GetPluralString\s*\(\s*" + TwoStringsArgumentsPattern;
    const string GetPluralStringFmtFullPattern = @"GetPluralStringFmt\s*\(\s*" + TwoStringsArgumentsPattern;
    const string GetParticularStringFullPattern = @"GetParticularString\s*\(\s*" + TwoStringsArgumentsPattern;
    const string GetParticularPluralStringFullPattern = @"GetParticularPluralString\s*\(\s*" + ThreeStringsArgumentsPattern;
    const string TextAssignFullPattern = @"\.\s*Text\s*=\s*" + CsharpStringPattern + @"\s*;";
    const string TextConcatFullPattern = @"\.\s*Text\s*=\s*" + ConcatenatedStringsPattern;
    const string HeaderTextAssignFullPattern = @"\.\s*HeaderText\s*=\s*" + CsharpStringPattern + @"\s*;";
    const string HeaderTextConcatFullPattern = @"\.\s*HeaderText\s*=\s*" + ConcatenatedStringsPattern;
    const string ToolTipTextAssignFullPattern = @"\.\s*ToolTipText\s*=\s*" + CsharpStringPattern + @"\s*;";
    const string ToolTipTextConcatFullPattern = @"\.\s*ToolTipText\s*=\s*" + ConcatenatedStringsPattern;
    const string SetToolTipAssignFullPattern = @"\.\s*SetToolTip\s*\([^\\""]*\s*,\s*" + CsharpStringPattern + @"\s*\)\s*;";
    const string SetToolTipConcatFullPattern = @"\.\s*SetToolTip\s*\([^\\""]*\s*,\s*" + ConcatenatedStringsPattern;
    const string ApplyResourcesFullPattern = @"\.\s*ApplyResources\s*\([^\\""]*\s*,\s*" + CsharpStringPattern + @"\s*\)\s*;";
    const string RemoveCommentsFullPattern = @"/\*(.*?)\*/|//(.*?)(\r?\n|$)|" + CsharpStringPattern;

    // Source-generated regex (compiled at build time, no runtime allocation cost)
    [GeneratedRegex(GetStringFullPattern, RegexOptions.Multiline)]
    private static partial Regex GetStringRegex();
    [GeneratedRegex(GetStringFmtFullPattern, RegexOptions.Multiline)]
    private static partial Regex GetStringFmtRegex();
    [GeneratedRegex(GetPluralStringFullPattern, RegexOptions.Multiline)]
    private static partial Regex GetPluralStringRegex();
    [GeneratedRegex(GetPluralStringFmtFullPattern, RegexOptions.Multiline)]
    private static partial Regex GetPluralStringFmtRegex();
    [GeneratedRegex(GetParticularStringFullPattern, RegexOptions.Multiline)]
    private static partial Regex GetParticularStringRegex();
    [GeneratedRegex(GetParticularPluralStringFullPattern, RegexOptions.Multiline)]
    private static partial Regex GetParticularPluralStringRegex();
    [GeneratedRegex(TextAssignFullPattern, RegexOptions.Multiline)]
    private static partial Regex TextAssignRegex();
    [GeneratedRegex(TextConcatFullPattern, RegexOptions.Multiline)]
    private static partial Regex TextConcatRegex();
    [GeneratedRegex(HeaderTextAssignFullPattern, RegexOptions.Multiline)]
    private static partial Regex HeaderTextAssignRegex();
    [GeneratedRegex(HeaderTextConcatFullPattern, RegexOptions.Multiline)]
    private static partial Regex HeaderTextConcatRegex();
    [GeneratedRegex(ToolTipTextAssignFullPattern, RegexOptions.Multiline)]
    private static partial Regex ToolTipTextAssignRegex();
    [GeneratedRegex(ToolTipTextConcatFullPattern, RegexOptions.Multiline)]
    private static partial Regex ToolTipTextConcatRegex();
    [GeneratedRegex(SetToolTipAssignFullPattern, RegexOptions.Multiline)]
    private static partial Regex SetToolTipAssignRegex();
    [GeneratedRegex(SetToolTipConcatFullPattern, RegexOptions.Multiline)]
    private static partial Regex SetToolTipConcatRegex();
    [GeneratedRegex(ApplyResourcesFullPattern, RegexOptions.Multiline)]
    private static partial Regex ApplyResourcesRegex();
    [GeneratedRegex(RemoveCommentsFullPattern, RegexOptions.Singleline)]
    private static partial Regex RemoveCommentsRegex();
    [GeneratedRegex(CsharpStringPattern)]
    private static partial Regex CsharpStringRegex();

    public const string CsharpStringPatternMacro = "%CsharpString%";

    public Catalog Catalog { get; private set; }
    public Options Options { get; private set; }

    public ExtractorCsharp(Options options)
    {
        Options = options;
        Catalog = new Catalog();
        if (!Options.Overwrite && File.Exists(Options.OutFile))
        {
            Catalog.Load(Options.OutFile);
            foreach (CatalogEntry entry in Catalog)
                entry.ClearReferences();
        }
        else
        {
            Catalog.Project = "PACKAGE VERSION";
        }

        Options.OutFile = Path.GetFullPath(Options.OutFile);
    }

    public void GetMessages()
    {
        // Create input files list
        Dictionary<string, string> inputFiles = [];
        foreach (string dir in Options.InputDirs)
        {
            foreach (string fileNameOrMask in Options.InputFiles)
            {
                string[] filesInDir = Directory.GetFiles(
                    dir,
                    fileNameOrMask,
                    Options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                foreach (string fileName in filesInDir)
                {
                    string fullFileName = Path.GetFullPath(fileName);
                    if (!inputFiles.ContainsKey(fullFileName))
                        inputFiles.Add(fullFileName, fullFileName);
                }
            }
        }

        foreach (string inputFile in inputFiles.Values)
        {
            GetMessagesFromFile(inputFile);
        }
    }

    private void GetMessagesFromFile(string inputFile)
    {
        inputFile = Path.GetFullPath(inputFile);
        using StreamReader input = new(inputFile, Options.InputEncoding, Options.DetectEncoding);
        string text = input.ReadToEnd();
        GetMessages(text, inputFile);
    }


    public void GetMessages(string text, string inputFile)
    {
        text = RemoveComments(text);

        // Gettext functions patterns
        ProcessPattern(ExtractMode.Msgid, GetStringRegex(), text, inputFile);
        ProcessPattern(ExtractMode.Msgid, GetStringFmtRegex(), text, inputFile);
        ProcessPattern(ExtractMode.MsgidPlural, GetPluralStringRegex(), text, inputFile);
        ProcessPattern(ExtractMode.MsgidPlural, GetPluralStringFmtRegex(), text, inputFile);
        ProcessPattern(ExtractMode.ContextMsgid, GetParticularStringRegex(), text, inputFile);
        ProcessPattern(ExtractMode.ContextMsgid, GetParticularPluralStringRegex(), text, inputFile);

        // Avalonia patterns
        ProcessPattern(ExtractMode.Msgid, TextAssignRegex(), text, inputFile);
        ProcessPattern(ExtractMode.MsgidConcat, TextConcatRegex(), text, inputFile);

        ProcessPattern(ExtractMode.Msgid, HeaderTextAssignRegex(), text, inputFile);
        ProcessPattern(ExtractMode.MsgidConcat, HeaderTextConcatRegex(), text, inputFile);

        ProcessPattern(ExtractMode.Msgid, ToolTipTextAssignRegex(), text, inputFile);
        ProcessPattern(ExtractMode.MsgidConcat, ToolTipTextConcatRegex(), text, inputFile);

        ProcessPattern(ExtractMode.Msgid, SetToolTipAssignRegex(), text, inputFile);
        ProcessPattern(ExtractMode.MsgidConcat, SetToolTipConcatRegex(), text, inputFile);

        if (ReadResources(inputFile))
            ProcessPattern(ExtractMode.MsgidFromResx, ApplyResourcesRegex(), text, inputFile);

        // Custom patterns (user-supplied at runtime — must remain dynamic)
        foreach (string pattern in Options.SearchPatterns)
        {
            var r = new Regex(
                pattern.Replace(CsharpStringPatternMacro, CsharpStringPattern),
                RegexOptions.Multiline);
            ProcessPattern(ExtractMode.Msgid, r, text, inputFile);
        }
    }


    public void Save()
    {
        if (File.Exists(Options.OutFile))
        {
            string bakFileName = Options.OutFile + ".bak";
            if (File.Exists(bakFileName))
                File.Delete(bakFileName);
            File.Copy(Options.OutFile, bakFileName);
            File.Delete(Options.OutFile);
        }
        Catalog.Save(Options.OutFile);
    }

    public static string RemoveComments(string input)
    {
        return RemoveCommentsRegex().Replace(
            input,
            m =>
            {
                if (m.Value.StartsWith("/*") || m.Value.StartsWith("//"))
                {
                    // Replace the comments with empty, i.e. remove them
                    return m.Value.StartsWith("//") ? m.Groups[3].Value : "";
                }
                // Keep the literal strings
                return m.Value;
            });
    }

    private void ProcessPattern(ExtractMode mode, Regex r, string text, string inputFile)
    {
        MatchCollection matches = r.Matches(text);
        foreach (Match match in matches)
        {
            GroupCollection groups = match.Groups;
            if (groups.Count < 2)
                throw new Exception($"Invalid pattern '{r}'.\nTwo groups are required at least.\nSource: {match.Value}");

            // Initialisation
            string context = string.Empty;
            string msgid = string.Empty;
            string msgidPlural = string.Empty;
            switch (mode)
            {
                case ExtractMode.Msgid:
                    msgid = Unescape(groups[1].Value);
                    break;
                case ExtractMode.MsgidConcat:
                    MatchCollection matches2 = CsharpStringRegex().Matches(groups[0].Value);
                    StringBuilder sb = new();
                    foreach (Match match2 in matches2)
                    {
                        sb.Append(Unescape(match2.Value));
                    }
                    msgid = sb.ToString();
                    break;
                case ExtractMode.MsgidFromResx:
                    string controlId = Unescape(groups[1].Value);
                    msgid = ExtractResourceString(controlId);
                    if (string.IsNullOrEmpty(msgid))
                    {
                        if (Options.Verbose)
                            Trace.WriteLine($"Warning: cannot extract string for control '{controlId}' ({inputFile})");
                        continue;
                    }
                    if (controlId == msgid)
                        continue; // Text property was initialized by controlId and was not changed so this text is not usable in application
                    break;
                case ExtractMode.MsgidPlural:
                    if (groups.Count < 3)
                        throw new Exception($"Invalid 'GetPluralString' call.\nSource: {match.Value}");
                    msgid = Unescape(groups[1].Value);
                    msgidPlural = Unescape(groups[2].Value);
                    break;
                case ExtractMode.ContextMsgid:
                    if (groups.Count < 3)
                        throw new Exception($"Invalid get context message call.\nSource: {match.Value}");
                    context = Unescape(groups[1].Value);
                    msgid = Unescape(groups[2].Value);
                    if (groups.Count == 4)
                        msgidPlural = Unescape(groups[3].Value);
                    break;
            }

            if (string.IsNullOrEmpty(msgid))
            {
                if (Options.Verbose)
                    Trace.Write($"WARN: msgid is empty in {inputFile}\r\n");
            }
            else
            {
                MergeWithEntry(context, msgid, msgidPlural, inputFile, CalcLineNumber(text, match.Index));
            }
        }
    }

    private void MergeWithEntry(
        string context,
        string msgid,
        string msgidPlural,
        string inputFile,
        int line)
    {
        // Processing
        CatalogEntry entry = Catalog.FindItem(msgid, context);
        bool entryFound = entry is not null;
        if (!entryFound)
            entry = new CatalogEntry(Catalog, msgid, msgidPlural);

        // Add source reference if it not exists yet
        // Each reference is in the form "path_name:line_number"
        string sourceRef = $"{Utils.FileUtils.GetRelativeUri(Path.GetFullPath(inputFile), Path.GetFullPath(Options.OutFile))}:{line}";
        entry.AddReference(sourceRef); // Wont be added if exists

        if (FormatValidator.IsFormatString(msgid) || FormatValidator.IsFormatString(msgidPlural))
        {
            if (!entry.IsInFormat("csharp"))
                entry.Flags += ", csharp-format";
            Trace.WriteLineIf(
                !FormatValidator.IsValidFormatString(msgid),
                $"Warning: string format may be invalid: '{msgid}'\nSource: {sourceRef}");
            Trace.WriteLineIf(
                !FormatValidator.IsValidFormatString(msgidPlural),
                $"Warning: plural string format may be invalid: '{msgidPlural}'\nSource: {sourceRef}");
        }

        if (!string.IsNullOrEmpty(msgidPlural))
        {
            if (!entryFound)
            {
                AddPluralsTranslations(entry);
            }
            else
                UpdatePluralEntry(entry, msgidPlural);
        }
        if (!string.IsNullOrEmpty(context))
        {
            entry.Context = context;
            entry.AddAutoComment($"Context: {context}", true);
        }

        if (!entryFound)
            Catalog.AddItem(entry);
    }


    private readonly Dictionary<string, string> resources = [];

    private bool ReadResources(string inputFile)
    {
        resources.Clear();
        string resxFileName = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile));
        if (resxFileName.EndsWith(".Designer"))
            resxFileName = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(resxFileName));
        resxFileName += ".resx";

        if (!File.Exists(resxFileName))
            return false;
        else
        {
            if (Options.Verbose)
                Debug.WriteLine($"Extracting from resource file: {resxFileName} (Input file: {inputFile})");
        }
        var doc = XDocument.Load(resxFileName);
        foreach (var data in doc.Root.Elements("data"))
        {
            if (data.Attribute("type") != null || data.Attribute("mimetype") != null) continue; // skip non-string resources
            var key = data.Attribute("name")?.Value;
            var value = data.Element("value")?.Value;
            if (key != null && value != null)
                resources.Add(key, value);
        }
        return true;
    }

    private string ExtractResourceString(string controlId)
    {
        if (!resources.TryGetValue(controlId + ".Text", out string msgid)
            && !resources.TryGetValue(controlId + ".TooTipText", out msgid)
            && !resources.TryGetValue(controlId + ".HeaderText", out msgid))
            return null;
        return msgid;
    }

    private static int CalcLineNumber(string text, int pos)
    {
        if (pos >= text.Length)
            pos = text.Length - 1;
        int line = 0;
        for (int i = 0; i < pos; i++)
            if (text[i] == '\n')
                line++;
        return line + 1;
    }

    private void UpdatePluralEntry(CatalogEntry entry, string msgidPlural)
    {
        if (!entry.HasPlural)
        {
            AddPluralsTranslations(entry);
            entry.SetPluralString(msgidPlural);
        }
        else if (entry.HasPlural && entry.PluralString != msgidPlural)
        {
            entry.SetPluralString(msgidPlural);
        }
    }

    private void AddPluralsTranslations(CatalogEntry entry)
    {
        // Creating 2 plurals forms by default
        // Translator should change it using expression for it own country
        // http://translate.sourceforge.net/wiki/l10n/pluralforms
        List<string> translations = [];
        for (int i = 0; i < Catalog.PluralFormsCount; i++)
            translations.Add("");
        entry.SetTranslations([.. translations]);
    }

    private static string Unescape(string msgid)
    {
        StringEscaping.EscapeMode mode = StringEscaping.EscapeMode.CSharp;
        if (msgid.StartsWith("@"))
            mode = StringEscaping.EscapeMode.CSharpVerbatim;
        return StringEscaping.UnEscape(mode, msgid.Trim(['@', '"']));
    }
}
