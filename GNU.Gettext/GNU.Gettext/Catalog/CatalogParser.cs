//
// CatalogParser.cs
//
// Author:
//   David Makovsk� <yakeen@sannyas-on.net>
//
// Copyright (C) 1999-2006 Vaclav Slavik (Code and design inspiration - poedit.org)
// Copyright (C) 2007 David Makovsk�
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GNU.Gettext
{
    //FIXME: StreamReader has been implemeneted but not a real parser
    public abstract class CatalogParser
    {
        internal static readonly string[] LineSplitStrings = { "\r\n", "\r", "\n" };

        readonly string newLine;
        readonly string fileName;
        readonly Encoding encoding;

        protected CatalogParser(string fileName, Encoding encoding)
        {

            newLine = GetNewLine(fileName, encoding);

            // parse command will open file later through streamreader
            this.fileName = fileName;
            this.encoding = encoding;

        }

        // Returns new line constant used in file
        public string NewLine
        {
            get { return newLine; }
        }

        // Detects the characters used for newline from the 1st newline present in file
        static string GetNewLine(string fileName, Encoding encoding)
        {

            char foundchar = 'x';
            char[] curr = new char[] { 'x' };

            using (TextReader tr = new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), encoding))
            {
                while (tr.Read(curr, 0, 1) != 0)
                {
                    if (curr[0] == '\n')
                    {
                        if (foundchar == '\r')
                            return "\r\n";
                        else
                            return "\n";
                    }
                    else if (curr[0] == '\r')
                    {
                        if (foundchar == 'x')
                            foundchar = '\r';
                        else if (foundchar == '\r')
                            return "\r";
                    }
                    else if (foundchar != 'x')
                        return foundchar.ToString();
                }
            }

            // only gets here if EOF reached	
            if (foundchar != 'x')
                return foundchar.ToString();
            else
                return Environment.NewLine;
        }

        // If input begins with pattern, fill output with end of input (without
        // pattern; strips trailing spaces) and return true.  Return false otherwise
        // or if input is null
        static bool ReadParam(string input, string pattern, out string output)
        {
            output = string.Empty;

            if (input == null)
                return false;

            input = input.TrimStart(' ', '\t');
            if (input.Length < pattern.Length)
                return false;

            if (!input.StartsWith(pattern))
                return false;

            // Avoid parsing Windows paths as escape sequences
            if (pattern.Trim().Equals("#:"))
                input = input.Replace('\\', '/');

            output = StringEscaping.FromGettextFormat(input.Substring(pattern.Length).TrimEnd(' ', '\t'));
            return true;
        }

        // returns value in dummy plus any trailing lines in sr enclosed in quotes
        // next line is ready for parsing by function end
        string ParseMessage(ref string line, ref string dummy, StreamReader sr)
        {
            StringBuilder result = new StringBuilder(dummy.Substring(0, dummy.Length - 1));

            while (!string.IsNullOrEmpty(line = sr.ReadLine()))
            {
                if (line[0] == '\t')
                    line = line.Substring(1);

                if (line[0] == '"' && line[line.Length - 1] == '"')
                {
                    result.Append(StringEscaping.FromGettextFormat(line.Substring(1, line.Length - 2)));
                }
                else
                    break;
            }
            return result.ToString();
        }

        // Parses the entire file, calls OnEntry each time msgid/msgstr pair is found.
        // return false if parsing failed, true otherwise
        public bool Parse()
        {

            string line;
            string mflags = string.Empty;
            string mstr = string.Empty;
            string msgctxt = string.Empty;
            string msgidPlural = string.Empty;
            string mcomment = string.Empty;
            List<string> mrefs = new List<string>();
            List<string> mautocomments = new List<string>();
            List<string> mtranslations = new List<string>();
            bool hasPlural = false;

            using (StreamReader sr = new StreamReader(fileName, encoding))
            {
                line = sr.ReadLine();

                while (line == "")
                    line = sr.ReadLine();

                if (line == null)
                    return false;

                while (line != null)
                {
                    // ignore empty special tags (except for automatic comments which we
                    // DO want to preserve):
                    while (line == "#," || line == "#:")
                        line = sr.ReadLine();

                    // flags:
                    // Can't we have more than one flag, now only the last is kept ...
                    if (CatalogParser.ReadParam(line, "#, ", out string dummy))
                    {
                        mflags = dummy; //"#, " +
                        line = sr.ReadLine();
                    }

                    // auto comments:
                    if (ReadParam(line, "#. ", out dummy) || ReadParam(line, "#.", out dummy)) // second one to account for empty auto comments
                    {
                        mautocomments.Add(dummy);
                        line = sr.ReadLine();
                    }

                    // references:
                    else if (CatalogParser.ReadParam(line, "#: ", out dummy))
                    {
                        // A line may contain several references, separated by white-space.
                        // Each reference is in the form "path_name:line_number"
                        // (path_name may contain spaces)
                        dummy = dummy.Trim();
                        while (dummy != string.Empty)
                        {
                            int i = 0;
                            while (i < dummy.Length && dummy[i] != ':')
                            {
                                i++;
                            }
                            while (i < dummy.Length && !Char.IsWhiteSpace(dummy[i]))
                            {
                                i++;
                            }

                            //store paths as Unix-type paths, but internally use native style
                            string refpath = dummy.Substring(0, i);
                            if (Path.DirectorySeparatorChar == '\\')
                            {
                                refpath = refpath.Replace('/', Path.DirectorySeparatorChar);
                            }

                            mrefs.Add(refpath);
                            dummy = dummy.Substring(i).Trim();
                        }

                        line = sr.ReadLine();
                    }

                    // msgctxt
                    else if (ReadParam(line, "msgctxt \"", out dummy) ||
                             ReadParam(line, "msgctxt\t\"", out dummy))
                    {
                        msgctxt = ParseMessage(ref line, ref dummy, sr);
                    }

                    // msgid:
                    else if (ReadParam(line, "msgid \"", out dummy) ||
                             ReadParam(line, "msgid\t\"", out dummy))
                    {
                        mstr = ParseMessage(ref line, ref dummy, sr);
                    }

                    // msgid_plural:
                    else if (ReadParam(line, "msgid_plural \"", out dummy) ||
                             ReadParam(line, "msgid_plural\t\"", out dummy))
                    {
                        msgidPlural = ParseMessage(ref line, ref dummy, sr);
                        hasPlural = true;
                    }

                    // msgstr:
                    else if (ReadParam(line, "msgstr \"", out dummy) ||
                             ReadParam(line, "msgstr\t\"", out dummy))
                    {
                        if (hasPlural)
                        {
                            // TODO: use logging
                            Console.WriteLine("Broken catalog file: singular form msgstr used together with msgid_plural");
                            return false;
                        }

                        string str = ParseMessage(ref line, ref dummy, sr);
                        mtranslations.Add(str);

                        if (!OnEntry(mstr, string.Empty, false, mtranslations.ToArray(),
                                       mflags, mrefs.ToArray(), mcomment,
                                       mautocomments.ToArray(),
                                       msgctxt))
                        {
                            return false;
                        }

                        // Cleanup vars
                        mcomment = mstr = msgidPlural = mflags = msgctxt = string.Empty;
                        hasPlural = false;
                        mrefs.Clear();
                        mautocomments.Clear();
                        mtranslations.Clear();
                    }
                    else if (ReadParam(line, "msgstr[", out dummy))
                    {
                        // msgstr[i]:
                        if (!hasPlural)
                        {
                            // TODO: use logging
                            Console.WriteLine("Broken catalog file: plural form msgstr used without msgid_plural");
                            return false;
                        }

                        int pos = dummy.IndexOf(']');
                        string idx = dummy.Substring(pos - 1, 1);
                        string label = "msgstr[" + idx + "]";

                        while (ReadParam(line, label + " \"", out dummy) || CatalogParser.ReadParam(line, label + "\t\"", out dummy))
                        {
                            StringBuilder str = new StringBuilder(dummy.Substring(0, dummy.Length - 1));

                            while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                            {
                                if (line[0] == '\t')
                                    line = line.Substring(1);
                                if (line[0] == '"' && line[line.Length - 1] == '"')
                                {
                                    str.Append(line.Substring(1, line.Length - 2));
                                }
                                else
                                {
                                    if (ReadParam(line, "msgstr[", out dummy))
                                    {
                                        pos = dummy.IndexOf(']');
                                        idx = dummy.Substring(pos - 1, 1);
                                        label = "msgstr[" + idx + "]";
                                    }
                                    break;
                                }
                            }
                            mtranslations.Add(StringEscaping.FromGettextFormat(str.ToString()));
                        }

                        if (!OnEntry(mstr, msgidPlural, true, mtranslations.ToArray(),
                                       mflags, mrefs.ToArray(), mcomment,
                                       mautocomments.ToArray(),
                                       msgctxt))
                        {
                            return false;
                        }

                        mcomment = mstr = msgidPlural = mflags = string.Empty;
                        hasPlural = false;
                        mrefs.Clear();
                        mautocomments.Clear();
                        mtranslations.Clear();
                    }
                    else if (ReadParam(line, "#~ ", out dummy))
                    {
                        // deleted lines:

                        List<string> deletedLines = new List<string>
                        {
                            line
                        };
                        while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                        {
                            // if line does not start with "#~ " anymore, stop reading
                            if (!ReadParam(line, "#~ ", out dummy))
                                break;

                            deletedLines.Add(line);
                        }
                        if (!OnDeletedEntry(deletedLines.ToArray(), mflags, null, mcomment, mautocomments.ToArray()))
                            return false;

                        mcomment = mstr = msgidPlural = mflags = msgctxt = string.Empty;
                        hasPlural = false;
                        mrefs.Clear();
                        mautocomments.Clear();
                        mtranslations.Clear();
                    }
                    else if (line != null && line[0] == '#')
                    {
                        // comment:

                        //  added line[1] != '~' check as deleted lines where being wrongly detected as comments
                        while (!string.IsNullOrEmpty(line) &&
                               ((line[0] == '#' && line.Length < 2) ||
                               (line[0] == '#' && line[1] != ',' && line[1] != ':' && line[1] != '.' && line[1] != '~')))
                        {
                            mcomment += mcomment.Length > 0 ? '\n' + line : line;
                            line = sr.ReadLine();
                        }
                    }
                    else
                    {
                        line = sr.ReadLine();

                    }

                    while (line == string.Empty)
                        line = sr.ReadLine();
                }
            }
            return true;
        }

        // Called when new entry was parsed. Parsing continues
        // if returned value is true and is cancelled if it is false.
        protected abstract bool OnEntry(string msgid, string msgidPlural, bool hasPlural,
                                         string[] translations, string flags,
                                         string[] references, string comment,
                                         string[] autocomments,
                                         string msgctxt);

        // Called when new deleted entry was parsed. Parsing continues
        // if returned value is true and is cancelled if it
        // is false. Defaults to an empty implementation.
        protected virtual bool OnDeletedEntry(string[] deletedLines, string flags,
                                               string[] references, string comment,
                                               string[] autocomments)
        {
            return true;
        }

    }
}