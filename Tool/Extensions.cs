// Extensions.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa.
// All rights reserved.
//
// This file is part of the source code disribution for Ionic's
// XPath Visualizer Tool.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.rtf or License.txt for the license details.
// More info on: http://XPathVisualizer.codeplex.com
//
// ------------------------------------------------------------------
//

using System;
using System.Collections.Generic;

namespace XPathVisualizer
{
    public static class Extensions
    {
        public static string XmlEscapeQuotes(this String s)
        {
            while (s.Contains("\""))
            {
                s = s.Replace("\"", "&quot;");
            }
            return s;
        }

        public static string XmlEscapeIexcl(this String s)
        {
            while (s.Contains("¡"))
            {
                s = s.Replace("¡", "&#161;");
            }
            return s;
        }
        public static string XmlUnescapeIexcl(this String s)
        {
            while (s.Contains("&#161;"))
            {
                s = s.Replace("&#161;", "¡");
            }
            return s;
        }

        public static List<String> ToList(this System.Windows.Forms.AutoCompleteStringCollection coll)
        {
            var list = new List<String>();
            foreach (string item in coll)
            {
                list.Add(item);
            }
            return list;
        }

    }
}
