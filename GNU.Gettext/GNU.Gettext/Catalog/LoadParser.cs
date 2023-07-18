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

using System.Text;

namespace GNU.Gettext
{
    /// <summary>
    /// Load parser.
    /// </summary>
    internal class LoadParser : CatalogParser
	{
        readonly Catalog catalog;
		bool headerParsed = false;
		
		public LoadParser (Catalog catalog, string poFile, Encoding encoding) : base (poFile, encoding)
		{
			this.catalog = catalog;
		}
		
		protected override bool OnEntry (string msgid, string msgidPlural, bool hasPlural,
		                                 string[] translations, string flags,
		                                 string[] references, string comment,
		                                 string[] autocomments,
		                                 string msgctxt)
		{
			if (string.IsNullOrEmpty (msgid) && ! headerParsed) {
				// gettext header:
				catalog.ParseHeaderString (translations[0]);
				catalog.Comment = comment;
				headerParsed = true;
			} else {
				CatalogEntry d = new CatalogEntry (catalog, string.Empty, string.Empty);
				if (! string.IsNullOrEmpty (flags))
					d.Flags = flags;
				d.SetString (msgid);
				if (hasPlural)
				    d.SetPluralString (msgidPlural);
				d.SetTranslations (translations);
				d.Comment = comment;
				for (uint i = 0; i < references.Length; i++) {
					d.AddReference (references[i]);
				}
				for (uint i = 0; i < autocomments.Length; i++) {
					d.AddAutoComment (autocomments[i]);
				}
				d.Context = msgctxt;
				catalog.AddItem (d);
			}
			return true;
		}
		
		 protected override bool OnDeletedEntry (string[] deletedLines, string flags,
		                                        string[] references, string comment,
		                                        string[] autocomments)
		{
			CatalogDeletedEntry d = new CatalogDeletedEntry (new string[0]);
			if (!string.IsNullOrEmpty (flags))
				d.Flags = flags;
			d.SetDeletedLines (deletedLines);
			d.SetComment (comment);
			for (uint i = 0; i < autocomments.Length; i++) {
				d.AddAutoComments (autocomments[i]);
				
			}
			catalog.AddDeletedItem (d);
			return true;
		}
	}
}