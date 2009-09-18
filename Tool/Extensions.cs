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
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.XPath;
using System.Drawing;


namespace XPathVisualizer
{

    // This previously was an extension, but it performs much better as
    // a separate class. Key reason: state. For example, this class
    // takes the rtb.Text once, instead of every time through, which
    // provides a big performance advantage.  Also: it stores the last
    // search and starts the search from there, if appropriate.
    internal class LineCalculator 
    {
        private int lastLine=Int32.MaxValue;
        private int lastC=-1;
        private string txt;

        public LineCalculator(System.Windows.Forms.RichTextBox rtb)
        {
            txt = rtb.Text;
        }
        
        public int GetCharIndexFromLine(int line)
        {
            // The built-in RichTextBox.GetFirstCharIndexFromLine does not 
            // work for me. Not sure why.
            line++;
            if (line <= 1) return 0;

            int c = 0;
            int cLine = 0;

            if (line >= lastLine)
            {
                c= lastC;
                cLine= lastLine-1;
            }
            if (cLine + 1 == line)
                return c;
        

            do
            {
                int delta = txt.IndexOf('\n', c);
                if (delta - c < 0) return -1;
                c = delta + 1;
                cLine++;
            }
            while (cLine + 1 < line);


            lastLine = line;
            lastC = c;
            
            return c;
        }
    }



    
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


        public static void ColorizeXml(this System.Windows.Forms.RichTextBox rtb)
        {
            string txt = rtb.Text;
            var lc = new LineCalculator(rtb);
            var sr = new StringReader(txt);
            XmlReader reader = XmlReader.Create(sr);

            if ((reader as IXmlLineInfo) != null)
            {
                IXmlLineInfo rinfo = (IXmlLineInfo)reader;
                if (rinfo.HasLineInfo())
                {
                    int ix = 0;
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    + rinfo.LinePosition - 1;
                                rtb.Select(ix - 1, 1);
                                rtb.SelectionColor = Color.Blue;
                                rtb.Select(ix, reader.Name.Length);
                                rtb.SelectionColor = Color.DarkRed;

                                if (reader.HasAttributes)
                                {
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        //string s = reader.Value;
                                        ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                            + rinfo.LinePosition - 1;
                                        rtb.Select(ix, reader.Name.Length);
                                        rtb.SelectionColor = Color.Red;

                                        ix += reader.Name.Length;

                                        ix = txt.IndexOf('=', ix);

                                        // make the equals sign blue
                                        rtb.Select(ix, 1);
                                        rtb.SelectionColor = Color.Blue;

                                        // skip over the quote char (it remains black)
                                        ix = txt.IndexOf(reader.QuoteChar, ix);
                                        ix++;
                                        // highlight the value of the attribute as blue
                                        if (txt.Substring(ix).StartsWith(reader.Value))
                                        {
                                            rtb.Select(ix, reader.Value.Length);
                                        }
                                        else
                                        {
                                            // Difference in escaping.  The InnerXml may include
                                            // \" where &quot; is in the doc.
                                            string s = reader.Value.XmlEscapeQuotes();
                                            int delta = s.Length - reader.Value.Length;
                                            rtb.Select(ix, reader.Value.Length + delta);
                                        }
                                        rtb.SelectionColor = Color.Blue;

                                    }
                                    while (reader.MoveToNextAttribute());

                                    ix = txt.IndexOf('>', ix);

                                    // the close-angle-bracket
                                    if (txt[ix-1]=='/')
                                        rtb.Select(ix-1, 2);
                                    else
                                        rtb.Select(ix, 1);
                                    rtb.SelectionColor = Color.Blue;
                                }
                                break;

                            case XmlNodeType.Text: // Display the text in each element.
                                //ix = rtb.MyGetCharIndexFromLine(rinfo.LineNumber-1) +
                                //    + rinfo.LinePosition-1;
                                //rtb.Select(ix, reader.Value.Length);
                                //rtb.SelectionColor = Color.Black;
                                break;

                            case XmlNodeType.EndElement: // Display the end of the element.
                                ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    + rinfo.LinePosition - 1;
                                rtb.Select(ix - 2, 2);
                                rtb.SelectionColor = Color.Blue;
                                rtb.Select(ix, reader.Name.Length);
                                rtb.SelectionColor = Color.DarkRed;
                                rtb.Select(ix + reader.Name.Length, 1);
                                rtb.SelectionColor = Color.Blue;
                                break;

                            case XmlNodeType.Attribute:
                                // These are handed within XmlNodeType.Element
                                // ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                //    + rinfo.LinePosition - 1;
                                //rtb.Select(ix, reader.Name.Length);
                                //rtb.SelectionColor = Color.Green;
                                break;

                            case XmlNodeType.Comment:
                                ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    + rinfo.LinePosition - 1;                                
                                rtb.Select(ix, reader.Value.Length);
                                rtb.SelectionColor = Color.Green;
                                break;
                        }
                    }
                }
            }
        }

        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        public static IntPtr BeginUpdate(this System.Windows.Forms.RichTextBox rtb)
        {
            // Stop redrawing:
            SendMessage(rtb.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
            // Stop sending of events:
            IntPtr eventMask = SendMessage(rtb.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);

            return eventMask;
        }

        public static void EndUpdate(this System.Windows.Forms.RichTextBox rtb, IntPtr eventMask)
        {
            // turn on events
            SendMessage(rtb.Handle, EM_SETEVENTMASK, 0, eventMask);
            // turn on redrawing
            SendMessage(rtb.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            rtb.Invalidate();
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