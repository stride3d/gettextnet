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
    internal class CharsetInfoFinder : CatalogParser
	{
		string charset;

		// Expecting iso-8859-1 encoding
		public CharsetInfoFinder (string poFile)
			: base (poFile, Encoding.GetEncoding ("iso-8859-1"))
		{
			charset = "iso-8859-1";
		}
		
		public string Charset {
			get { 
				return charset; 
			}
		}
		
		protected override bool OnEntry (string msgid, string msgidPlural, bool hasPlural,
		                                 string[] translations, string flags,
		                                 string[] references, string comment,
		                                 string[] autocomments,
		                                 string msgctxt)
		{
			if (string.IsNullOrEmpty (msgid)) {
				// gettext header:
				Catalog headers = new Catalog ();
				headers.ParseHeaderString (translations[0]);
				charset = headers.Charset;
				if (charset == "CHARSET")
					charset = "iso-8859-1";
				return false; // stop parsing
			}
			return true;
		}
	}
}