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

using System.Resources; /* ResourceManager, ResourceSet, IResourceReader */
using System.Collections; /* Hashtable, ICollection, IEnumerator, IDictionaryEnumerator */

namespace GNU.Gettext
{
    /// <summary>
    /// A trivial <c>IResourceReader</c> implementation.
    /// </summary>
    class DummyResourceReader : IResourceReader
    {

        // Implementation of IDisposable.
        void System.IDisposable.Dispose()
        {
        }

        // Implementation of IEnumerable.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        // Implementation of IResourceReader.
        void System.Resources.IResourceReader.Close()
        {
        }
        IDictionaryEnumerator IResourceReader.GetEnumerator()
        {
            return null;
        }

    }

}
