/* GNU gettext for C#
 * Copyright (C) 2003, 2005, 2007, 2012 Free Software Foundation, Inc.
 * Written by Bruno Haible <bruno@clisp.org>, 2003.
 * Adapted by Serguei Tarassov <st@arbinada.com>, 2012.
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Library General Public License as published
 * by the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301,
 * USA.
 */

/*
 * Using the GNU gettext approach, compiled message catalogs are assemblies
 * containing just one class, a subclass of GettextResourceSet. They are thus
 * interoperable with standard ResourceManager based code.
 *
 * The main differences between the common .NET resources approach and the
 * GNU gettext approach are:
 * - In the .NET resource approach, the keys are abstract textual shortcuts.
 *   In the GNU gettext approach, the keys are the English/ASCII version
 *   of the messages.
 * - In the .NET resource approach, the translation files are called
 *   "Resource.locale.resx" and are UTF-8 encoded XML files. In the GNU gettext
 *   approach, the translation files are called "Resource.locale.po" and are
 *   in the encoding the translator has chosen. There are at least three GUI
 *   translating tools (Emacs PO mode, KDE KBabel, GNOME gtranslator).
 * - In the .NET resource approach, the function ResourceManager.GetString
 *   returns an empty string or throws an InvalidOperationException when no
 *   translation is found. In the GNU gettext approach, the GetString function
 *   returns the (English) message key in that case.
 * - In the .NET resource approach, there is no support for plural handling.
 *   In the GNU gettext approach, we have the GetPluralString function.
 * - In the .NET resource approach, there is no support for context specific
 *   translations.
 *   In the GNU gettext approach, we have the GetParticularString function.
 *
 * To compile GNU gettext message catalogs into C# assemblies, the msgfmt
 * program can be used.
 */

using System;
using System.Resources;
using System.Collections;
using System.IO;

namespace GNU.Gettext
{
    /// <summary>
    /// <para>
    /// Each instance of this class encapsulates a single PO file.
    /// </para>
    /// <para>
    /// This API of this class is not meant to be used directly; use
    /// <c>GettextResourceManager</c> instead.
    /// </para>
    /// </summary>
    // We need this subclass of ResourceSet, because the plural formula must come
    // from the same ResourceSet as the object containing the plural forms.
    public class GettextResourceSet : ResourceSet
    {

        /// <summary>
        /// Creates a new message catalog. When using this constructor, you
        /// must override the <c>ReadResources</c> method, in order to initialize
        /// the <c>Table</c> property. The message catalog will support plural
        /// forms only if the <c>ReadResources</c> method installs values of type
        /// <c>String[]</c> and if the <c>PluralEval</c> method is overridden.
        /// </summary>
        protected GettextResourceSet()
            : base(DummyResourceReader)
        {
        }

        /// <summary>
        /// Creates a new message catalog, by reading the string/value pairs from
        /// the given <paramref name="reader"/>. The message catalog will support
        /// plural forms only if the reader can produce values of type
        /// <c>String[]</c> and if the <c>PluralEval</c> method is overridden.
        /// </summary>
        public GettextResourceSet(IResourceReader reader)
            : base(reader)
        {
        }

        /// <summary>
        /// Creates a new message catalog, by reading the string/value pairs from
        /// the given <paramref name="stream"/>, which should have the format of
        /// a <c>.resources</c> file. The message catalog will not support plural
        /// forms.
        /// </summary>
        public GettextResourceSet(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Creates a new message catalog, by reading the string/value pairs from
        /// the file with the given <paramref name="fileName"/>. The file should
        /// be in the format of a <c>.resources</c> file. The message catalog will
        /// not support plural forms.
        /// </summary>
        public GettextResourceSet(String fileName)
            : base(fileName)
        {
        }
		
		/// <summary>
		/// Constant for default plural forms (English/French/Germany).
		/// </summary>
		public const string DefaultPluralForms = "nplurals=2; plural=(n != 1);";
		
        public virtual string PluralForms 
		{
			get { return DefaultPluralForms; }
		}


        /// <summary>
        /// Returns the translation of <paramref name="msgid"/>.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or <c>null</c> if
        ///          none is found</returns>
        // The default implementation essentially does (String)Table[msgid].
        // Here we also catch the plural form case.
        public override string GetString(string msgid)
        {
            object value = GetObject(msgid);
            if (value == null || value is string)
                return (string)value;
            else if (value is string[])
                // A plural form, but no number is given.
                // Like the C implementation, return the first plural form.
                return (value as string[])[0];
            else
                throw new InvalidOperationException("resource for \"" + msgid + "\" in " + GetType().FullName + " is not a string");
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/>, with possibly
        /// case-insensitive lookup.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <returns>the translation of <paramref name="msgid"/>, or <c>null</c> if
        ///          none is found</returns>
        // The default implementation essentially does (String)Table[msgid].
        // Here we also catch the plural form case.
        public override string GetString(string msgid, bool ignoreCase)
        {
            object value = GetObject(msgid, ignoreCase);
            if (value == null || value is string)
                return (string)value;
            else if (value is string[])
                // A plural form, but no number is given.
                // Like the C implementation, return the first plural form.
                return (value as string[])[0];
            else
                throw new InvalidOperationException("resource for \"" + msgid + "\" in " + GetType().FullName + " is not a string");
        }

        /// <summary>
        /// Returns the translation of <paramref name="msgid"/> and
        /// <paramref name="msgidPlural"/>, choosing the right plural form
        /// depending on the number <paramref name="n"/>.
        /// </summary>
        /// <param name="msgid">the key string to be translated, an ASCII
        ///                     string</param>
        /// <param name="msgidPlural">the English plural of <paramref name="msgid"/>,
        ///                           an ASCII string</param>
        /// <param name="n">the number, should be &gt;= 0</param>
        /// <returns>the translation, or <c>null</c> if none is found</returns>
        public virtual string GetPluralString(string msgid, String msgidPlural, long n)
        {
            Object value = GetObject(msgid);
            if (value == null || value is string)
                return (string)value;
            else if (value is string[])
            {
                String[] choices = value as string[];
                long index = PluralEval(n);
                return choices[index >= 0 && index < choices.Length ? index : 0];
            }
            else
                throw new InvalidOperationException("resource for \"" + msgid + "\" in " + GetType().FullName + " is not a string");
        }

        /// <summary>
        /// Returns the index of the plural form to be chosen for a given number.
        /// The default implementation is the Englis/Germanic/French plural formula:
        /// See <see cref="DefaultPluralForms"/>
        /// </summary>
        protected virtual long PluralEval(long n)
        {
			PluralFormsCalculator pfc = PluralFormsCalculator.Make(PluralForms);
			if (pfc != null)
				return pfc.Evaluate(n);
			pfc = PluralFormsCalculator.Make(DefaultPluralForms);
			if (pfc != null)
				return pfc.Evaluate(n);
            return (n == 1 ? 0 : 1);
        }

        /// <summary>
        /// A trivial instance of <c>IResourceReader</c> that does nothing.
        /// </summary>
        // Needed by the no-arguments constructor.
        private static readonly IResourceReader DummyResourceReader = new DummyResourceReader();

    }

}
