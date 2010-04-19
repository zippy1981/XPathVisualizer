﻿// RichTextBoxEx.cs
// ------------------------------------------------------------------
//
// An extended RichTextBox that provides a few extra capabilities:
//
//  1. line numbering (fast and easy)
//  2. programmatic scrolling
//  3. BeginUpdate/EndUpdate
//  4. FirstVisibleLine / NumberOfVisibleLines
//
//
// Copyright (c) 2010 Dino Chiesa.
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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Ionic.WinForms
{
    /// <summary>
    /// Defines methods for performing operations on RichTextBox.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>
    ///     The methods in this class could be defined as "extension methods" but
    ///     for efficiency I'd like to retain some state between calls - for
    ///     example the handle on the richtextbox or the buffer and structure for
    ///     the EM_SETCHARFORMAT message, which can be called many times in quick
    ///     succession.
    ///   </para>
    ///
    ///   <para>
    ///     We define these in a separate class for speed and efficiency. For the
    ///     RichTextBox, in order to make a change in format of some portion of
    ///     the text, the app must select the text.  When the RTB has focus, it
    ///     will scroll when the selection is updated.  If we want to retain state
    ///     while highlighting text then, we'll have to restore the scroll state
    ///     after a highlight is applied.  But this will produce an ugly UI effect
    ///     where the scroll jumps forward and back repeatedly.  To avoid that, we
    ///     need to suppress updates to the RTB, using the WM_SETREDRAW message.
    ///   </para>
    ///
    ///   <para>
    ///     As a complement to that, we also have some speedy methods to get and
    ///     set the scroll state, and the selection state.
    ///   </para>
    ///
    /// </remarks>
    [ToolboxBitmap(typeof(RichTextBox))]
    public class RichTextBoxEx : RichTextBox
    {
        private User32.CHARFORMAT charFormat;
        private IntPtr lParam1;
        //private Rectangle _bounds;

        private int _savedScrollLine;
        private int _savedSelectionStart;
        private int _savedSelectionEnd;

        private System.Drawing.StringFormat _stringDrawingFormat;

        public RichTextBoxEx()
        {
            charFormat = new User32.CHARFORMAT()
                {
                    cbSize = Marshal.SizeOf(typeof(User32.CHARFORMAT)),
                    szFaceName= new char[32]
                };

            lParam1= Marshal.AllocCoTaskMem( charFormat.cbSize );

            _stringDrawingFormat = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.None,
                };

            // defaults
            NumberColor = Color.FromName("Red");
            NumberBorder = SystemColors.ControlDark;
            NumberBackground1 = SystemColors.ControlLight;
            NumberBackground2= SystemColors.Window;
        }

        ~RichTextBoxEx()
        {
            // Free the allocated memory
            Marshal.FreeCoTaskMem(lParam1);
        }


        protected override void OnTextChanged(EventArgs e)
        {
            NeedRecomputeOfLineNumbers();
            base.OnTextChanged(e);
        }

        public void BeginUpdate()
        {
            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_SETREDRAW, 0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_SETREDRAW, 1, IntPtr.Zero);
        }


        public IntPtr BeginUpdateAndSuspendEvents()
        {
            // Stop redrawing:
            User32.SendMessage(this.Handle, (int) User32.Msgs.WM_SETREDRAW, 0, IntPtr.Zero);
            // Stop sending of events:
            IntPtr eventMask = User32.SendMessage(this.Handle, User32.Msgs.EM_GETEVENTMASK, 0, IntPtr.Zero);

            return eventMask;
        }

        public void EndUpdateAndResumeEvents(IntPtr eventMask)
        {
            // turn on events
            User32.SendMessage(this.Handle, User32.Msgs.EM_SETEVENTMASK, 0, eventMask);
            // turn on redrawing
            User32.SendMessage(this.Handle, User32.Msgs.WM_SETREDRAW, 1, IntPtr.Zero);
            NeedRecomputeOfLineNumbers();
            this.Invalidate();
        }



        public void GetSelection(out int start, out int end)
        {
            User32.SendMessageRef(this.Handle, (int)User32.Msgs.EM_GETSEL, out start, out end);
        }

        public void SetSelection(int start, int end)
        {
            User32.SendMessage(this.Handle, (int)User32.Msgs.EM_SETSEL, start, end);
        }

        public void BeginUpdateAndSaveState()
        {
            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_SETREDRAW, 0, IntPtr.Zero);
            // save scroll position
            _savedScrollLine = FirstVisibleDisplayLine;

            // save selection
            GetSelection(out _savedSelectionStart, out _savedSelectionEnd);
        }

        public void EndUpdateAndRestoreState()
        {
            // restore scroll position
            int Line1 = FirstVisibleDisplayLine;
            Scroll(_savedScrollLine - Line1);

            // restore the selection/caret
            SetSelection(_savedSelectionStart, _savedSelectionEnd);

            // allow redraw
            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_SETREDRAW, 1, IntPtr.Zero);

            // explicitly ask for a redraw?
            Refresh();
        }

        private int _lnw = -1;
        private int LineNumberWidth
        {
            get
            {
                if (_lnw > 0) return _lnw;
                int ndigits = (CharIndexForTextLine.Length == 0)
                ? 1
                : (int)(1 + Math.Log((double)CharIndexForTextLine.Length, 10));
                _lnw = (int)(8 * (1 + ndigits));
                return _lnw;
            }
        }

        public bool _lineNumbers;
        public bool ShowLineNumbers
        {
            get
            {
                return _lineNumbers;
            }
            set
            {
                if (value == _lineNumbers) return;
                SetLeftMargin(value ? LineNumberWidth + Margin.Left : Margin.Left);
                _lineNumbers = value;
            }
        }

        private void NeedRecomputeOfLineNumbers()
        {
            _CharIndexForTextLine = null;
            _Text2 = null;
            _lnw = -1;
        }

        public Color NumberColor  { get;set; }
        public Color NumberBorder { get;set; }
        public Color NumberBackground1 { get;set; }
        public Color NumberBackground2 { get;set; }

        private DateTime _lastMsgRecd = new DateTime(1901,1,1);

        protected override void WndProc(ref Message m)
        {
            bool handled = false;
            switch (m.Msg)
            {
                case (int)User32.Msgs.WM_PAINT:
                    //System.Console.WriteLine("{0}", User32.Mnemonic(m.Msg));
                    if (_lineNumbers)
                    {
                        base.WndProc(ref m);
                        this.PaintLineNumbers();
                        handled = true;
                    }
                    break;

                case (int)User32.Msgs.WM_CHAR:
                    // the text is being modified
                    NeedRecomputeOfLineNumbers();
                    break;

//                 case (int)User32.Msgs.EM_POSFROMCHAR:
//                 case (int)User32.Msgs.WM_GETDLGCODE:
//                 case (int)User32.Msgs.WM_ERASEBKGND:
//                 case (int)User32.Msgs.OCM_COMMAND:
//                 case (int)User32.Msgs.OCM_NOTIFY:
//                 case (int)User32.Msgs.EM_CHARFROMPOS:
//                 case (int)User32.Msgs.EM_LINEINDEX:
//                 case (int)User32.Msgs.WM_NCHITTEST:
//                 case (int)User32.Msgs.WM_SETCURSOR:
//                 case (int)User32.Msgs.WM_KEYUP:
//                 case (int)User32.Msgs.WM_KEYDOWN:
//                 case (int)User32.Msgs.WM_MOUSEMOVE:
//                 case (int)User32.Msgs.WM_MOUSEACTIVATE:
//                 case (int)User32.Msgs.WM_NCMOUSEMOVE:
//                 case (int)User32.Msgs.WM_NCMOUSEHOVER:
//                 case (int)User32.Msgs.WM_NCMOUSELEAVE:
//                 case (int)User32.Msgs.WM_NCLBUTTONDOWN:
//                     break;
//
//                   default:
//                       // divider
//                       var now = DateTime.Now;
//                       if ((now - _lastMsgRecd) > TimeSpan.FromMilliseconds(850))
//                           System.Console.WriteLine("------------ {0}", now.ToString("G"));
//                       _lastMsgRecd = now;
//
//                       System.Console.WriteLine("{0}", User32.Mnemonic(m.Msg));
//                       break;
            }

            if (!handled)
                base.WndProc(ref m);
        }


        int _lastWidth = 0;
        private void PaintLineNumbers()
        {
            // To reduce flicker, double-buffer the output
            int w = LineNumberWidth;
            if (w!=_lastWidth)
            {
                SetLeftMargin(w + Margin.Left);
                _lastWidth = w;
            }

            Bitmap buffer = new Bitmap(w, this.Bounds.Height);
            Graphics g = Graphics.FromImage(buffer);

            Font font= new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            Brush forebrush = new SolidBrush(NumberColor);
            Brush backBrush = SystemBrushes.Window;
            var rect = new Rectangle (0, 0, w, this.Bounds.Height);
            g.FillRectangle(backBrush, rect);

            Pen borderPen = new Pen(NumberBorder);
            g.DrawLine(borderPen, w-1, 0, w-1, this.Bounds.Height);

            int n = NumberOfVisibleTextLines;
            int ix = FirstVisibleTextLine;
            int py = 0;
            for (int i=0; i <= n; i++)
            {
                //System.Console.Write(".");
                int c = GetCharIndexForTextLine(ix);
                var p = GetPosFromCharIndex(c+1);
                if (p.Y < py) continue;
                rect = new Rectangle (1, py, LineNumberWidth-2, p.Y);
                // new brush each time for gradient across variable rect sizes
                backBrush = new LinearGradientBrush(rect,
                                                    NumberBackground1,
                                                    NumberBackground2,
                                                    LinearGradientMode.Vertical);
                g.FillRectangle(backBrush, rect);
                ix++;
                g.DrawString(ix.ToString(), font, forebrush, rect, _stringDrawingFormat);
                py = p.Y;
            }

            // in case the file ends with a partiaal line
            rect = new Rectangle (1, py, LineNumberWidth-2, 20);  // this.Bounds.Y+this.Bounds.Height);
            // new brush each time for gradient across variable rect sizes
            backBrush = new LinearGradientBrush(rect,
                                                NumberBackground1,
                                                NumberBackground2,
                                                LinearGradientMode.Vertical);
            g.FillRectangle(backBrush, rect);
            if (n>0)
                ix++;
            g.DrawString(ix.ToString(), font, forebrush, rect, _stringDrawingFormat);


            // paint that buffer to the screen
            Graphics g1 = this.CreateGraphics();
            g1.DrawImage(buffer, new Point(0,0));
            g1.Dispose();
            g.Dispose();
        }

        private int GetCharIndexFromPos(int x, int y)
        {
            var p = new User32.POINTL { X= x, Y = y };
            int rawSize = Marshal.SizeOf( typeof(User32.POINTL) );
            IntPtr lParam = Marshal.AllocHGlobal( rawSize );
            Marshal.StructureToPtr(p, lParam, false);
            int r = User32.SendMessage(this.Handle, (int)User32.Msgs.EM_CHARFROMPOS, 0, lParam);
            Marshal.FreeHGlobal( lParam );
            return r;
        }


        private Point GetPosFromCharIndex(int ix)
        {
            int rawSize = Marshal.SizeOf( typeof(User32.POINTL) );
            IntPtr wParam = Marshal.AllocHGlobal( rawSize );
            int r = User32.SendMessage(this.Handle, (int)User32.Msgs.EM_POSFROMCHAR, (int)wParam, ix);

            User32.POINTL p1 = (User32.POINTL) Marshal.PtrToStructure(wParam, typeof(User32.POINTL));

            Marshal.FreeHGlobal( wParam );
            var p = new Point { X= p1.X, Y = p1.Y };
            return p;
        }


        private int GetLengthOfLineContainingChar(int charIndex)
        {
            int r = User32.SendMessage(this.Handle, (int)User32.Msgs.EM_LINELENGTH, 0,0);
            return r;
        }

        private int GetLineFromChar(int charIndex)
        {
            return User32.SendMessage(this.Handle, (int)User32.Msgs.EM_LINEFROMCHAR, charIndex, 0);
        }

        private int GetCharIndexForDisplayLine(int line)
        {
            return User32.SendMessage(this.Handle, (int)User32.Msgs.EM_LINEINDEX, line, 0);
        }


        /// <summary>
        ///   Sets the color of the characters in the given range.
        /// </summary>
        ///
        /// <remarks>
        /// Calling this is equivalent to calling
        /// <code>
        ///   richTextBox.Select(start, end-start);
        ///   this.richTextBox1.SelectionColor = color;
        /// </code>
        /// ...but without the error and bounds checking.
        /// </remarks>
        ///
        public void SetSelectionColor(int start, int end, System.Drawing.Color color)
        {
            User32.SendMessage(this.Handle, (int)User32.Msgs.EM_SETSEL, start, end);

            charFormat.dwMask = 0x40000000;
            charFormat.dwEffects = 0;
            charFormat.crTextColor = System.Drawing.ColorTranslator.ToWin32(color);

            Marshal.StructureToPtr(charFormat, lParam1, false);
            User32.SendMessage(this.Handle, (int)User32.Msgs.EM_SETCHARFORMAT, User32.SCF_SELECTION, lParam1);
        }


        private void SetLeftMargin(int widthInPixels)
        {
            User32.SendMessage(this.Handle, (int)User32.Msgs.EM_SETMARGINS, User32.EC_LEFTMARGIN,
                               widthInPixels);
        }

        public Tuple<int,int> GetMargins()
        {
            int r = User32.SendMessage(this.Handle, (int)User32.Msgs.EM_GETMARGINS, 0,0);
            //System.Console.WriteLine("Margins: 0x{0,8:X8}", r);
            return Tuple.New(r & 0x0000FFFF, (int)((r>>16) & 0x0000FFFF));
        }

        public void Scroll(int delta)
        {
            User32.SendMessage(this.Handle, (int)User32.Msgs.EM_LINESCROLL, 0, delta);
        }


        public int FirstVisibleDisplayLine
        {
            get
            {
                return User32.SendMessage(this.Handle, (int)User32.Msgs.EM_GETFIRSTVISIBLELINE, 0, 0);
            }
            set
            {
                // scroll
                int current = FirstVisibleDisplayLine;
                int delta = value - current;
                User32.SendMessage(this.Handle, (int)User32.Msgs.EM_LINESCROLL, 0, delta);
            }
        }

        public int NumberOfVisibleDisplayLines
        {
            get
            {
                int topIndex = this.GetCharIndexFromPosition(new System.Drawing.Point(1, 1));
                int bottomIndex = this.GetCharIndexFromPosition(new System.Drawing.Point(1, this.Height - 1));
                int topLine = this.GetLineFromCharIndex(topIndex);
                int bottomLine = this.GetLineFromCharIndex(bottomIndex);
                int n = bottomLine - topLine + 1;
                return n;
            }
        }

        private int GetCharIndexForTextLine(int ix)
        {
            if (ix >= CharIndexForTextLine.Length) return 0;
            if (ix < 0) return 0;
            return CharIndexForTextLine[ix];
        }


        public int FirstVisibleTextLine
        {
            get
            {
                int c = GetCharIndexFromPos(1,1);
                for (int i=0; i < CharIndexForTextLine.Length; i++)
                {
                    if (c < CharIndexForTextLine[i]) return i;
                }
                return CharIndexForTextLine.Length;
            }
        }

        public int LastVisibleTextLine
        {
            get
            {
                int c = GetCharIndexFromPos(1,this.Bounds.Y+this.Bounds.Height);
                for (int i=0; i < CharIndexForTextLine.Length; i++)
                {
                    if (c < CharIndexForTextLine[i]) return i;
                }
                return CharIndexForTextLine.Length;
            }
        }

        public int NumberOfVisibleTextLines
        {
            get
            {
                return LastVisibleTextLine - FirstVisibleTextLine;
            }
        }

        private int[] _CharIndexForTextLine;
        private int[] CharIndexForTextLine
        {
            get
            {
                if (_CharIndexForTextLine == null)
                {
                    var list = new List<int>();
                    int ix = 0;
                    foreach( var c in Text2 )
                    {
                        if ( c == '\n' ) list.Add(ix);
                        ix++;
                    }
                    _CharIndexForTextLine = list.ToArray();
                }
                return _CharIndexForTextLine;
            }

        }


        private String _Text2;
        private String Text2
        {
            get
            {
                if (_Text2 == null)
                {
                    _Text2 = Text;
                }
                return _Text2;
            }

        }
    }


    public static class Tuple
    {
        // Allows Tuple.New(1, "2") instead of new Tuple<int, string>(1, "2")
        public static Tuple<T1, T2> New<T1, T2>(T1 v1, T2 v2)
        {
            return new Tuple<T1, T2>(v1, v2);
        }
    }

    public class Tuple<T1, T2>
    {
        public Tuple(T1 v1, T2 v2)
        {
            V1 = v1;
            V2 = v2;
        }

        public T1 V1 { get; set; }
        public T2 V2 { get; set; }
    }

}

