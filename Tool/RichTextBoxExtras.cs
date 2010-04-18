// RichTextBoxExtras.cs
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
using System.Runtime.InteropServices;

namespace XPathVisualizer
{

    internal static class User32
    {
        [DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wparam, int lparam);

        [DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        public static extern int SendMessageRef(IntPtr hwnd, int wMsg, out int wparam, out int lparam);

        // [DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        // public static extern int SendMessage(IntPtr hwnd, int wMsg, int ignored, out RECT lpRect);

        //[DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        //public static extern int SendMessage(IntPtr hwnd, int wMsg, int ignored, POINT lpPoint);

        [DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wparam, IntPtr lparam);


        public static void BeginUpdate(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_SETREDRAW, 0, IntPtr.Zero);
        }

        public static void EndUpdate(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_SETREDRAW, 1, IntPtr.Zero);
        }


        // from WinUser.h and RichEdit.h
        public const int EM_GETSEL              = 0x00B0;
        public const int EM_SETSEL              = 0x00B1;
        public const int EM_GETRECT             = 0x00B2;
        public const int EM_LINESCROLL          = 0x00B6;
        public const int EM_GETLINECOUNT        = 0x00BA;
        public const int EM_LINEFROMCHAR        = 0x00C9;
        public const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        public const int EM_CHARFROMPOS         = 0x00D7;
        public const int EM_GETCHARFORMAT       = 0x0400 + 58;
        public const int EM_SETCHARFORMAT       = 0x0400 + 68;
        public const int SCF_SELECTION          = 0x0001;
        public const int WM_SETREDRAW           = 0x000B;

        [ StructLayout( LayoutKind.Sequential )]
        public struct CHARFORMAT
        {
            public int    cbSize;
            public UInt32 dwMask;
            public UInt32 dwEffects;
            public Int32  yHeight;
            public Int32  yOffset;
            public Int32   crTextColor;
            public byte   bCharSet;
            public byte   bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
            public char[] szFaceName;
        }
    }


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
    public class RichTextBoxExtras
    {
        private System.Windows.Forms.RichTextBox _rtb;
        private IntPtr hWnd;
        private User32.CHARFORMAT charFormat;
        private IntPtr lParam1;

        private int _savedScrollLine;
        private int _savedSelectionStart;
        private int _savedSelectionEnd;

        public RichTextBoxExtras(System.Windows.Forms.RichTextBox rtb)
        {
            hWnd = rtb.Handle;
            _rtb = rtb;
            charFormat = new User32.CHARFORMAT()
                {
                    cbSize = Marshal.SizeOf(typeof(User32.CHARFORMAT)),
                    szFaceName= new char[32]
                };

            lParam1= Marshal.AllocCoTaskMem( charFormat.cbSize );
        }

        ~RichTextBoxExtras()
        {
            // Free the allocated memory
            Marshal.FreeCoTaskMem(lParam1);
        }

        public int GetFirstVisibleLine()
        {
            return User32.SendMessage(hWnd, User32.EM_GETFIRSTVISIBLELINE, 0, 0);
        }

        public void GetSelection(out int start, out int end)
        {
            User32.SendMessageRef(hWnd, User32.EM_GETSEL, out start, out end);
        }

        public void SetSelection(int start, int end)
        {
            User32.SendMessage(hWnd, User32.EM_SETSEL, start, end);
        }


        public void BeginUpdateAndSaveState()
        {
            User32.SendMessage(hWnd, User32.WM_SETREDRAW, 0, IntPtr.Zero);
            // save scroll position
            _savedScrollLine = GetFirstVisibleLine();

            // save selection
            GetSelection(out _savedSelectionStart, out _savedSelectionEnd);
        }

        public void EndUpdateAndRestoreState()
        {
            // restore scroll position
            int Line1 = GetFirstVisibleLine();
            Scroll(_savedScrollLine - Line1);

            // restore the selection/caret
            SetSelection(_savedSelectionStart, _savedSelectionEnd);

            // allow redraw
            User32.SendMessage(hWnd, User32.WM_SETREDRAW, 1, IntPtr.Zero);

            // explicitly ask for a redraw
            _rtb.Refresh();
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
            User32.SendMessage(hWnd, User32.EM_SETSEL, start, end);

            charFormat.dwMask = 0x40000000;
            charFormat.dwEffects = 0;
            charFormat.crTextColor = System.Drawing.ColorTranslator.ToWin32(color);

            Marshal.StructureToPtr(charFormat, lParam1, false);

            User32.SendMessage(hWnd, User32.EM_SETCHARFORMAT, User32.SCF_SELECTION, lParam1);
        }

        public void Scroll(int delta)
        {
            User32.SendMessage(hWnd, User32.EM_LINESCROLL, 0, delta);
        }


        #if HARDWORK

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

        private RECT GetRect()
        {
            int rawSize = Marshal.SizeOf( typeof(RECT) );
            IntPtr buffer = Marshal.AllocHGlobal( rawSize );
            int ignored = 0;
            SendMessage(hWnd, EM_GETRECT, ignored, buffer);
            RECT rect = (RECT) Marshal.PtrToStructure( buffer, typeof(RECT) );
            Marshal.FreeHGlobal( buffer );
            return rect;
        }

        private int GetCharFromPoint(int x, int y)
        {
            POINT point = new POINT(x, y);
            int rawSize = Marshal.SizeOf( typeof(POINT) );
            IntPtr buffer = Marshal.AllocHGlobal( rawSize );
            Marshal.StructureToPtr( point, buffer, false );
            int cix = SendMessage(hWnd, EM_CHARFROMPOS, 0, buffer);
            Marshal.FreeHGlobal( buffer );
            return cix;
        }


        public int NumberOfVisibleLines
        {
            get
            {

                int topIndex = RichTextBox1.GetCharIndexFromPosition(New Point(1, 1))
      Dim bottomIndex As Integer = RichTextBox1.GetCharIndexFromPosition(New Point(1, RichTextBox1.Height - 1))

      Dim topLine As Integer = RichTextBox1.GetLineFromCharIndex(topIndex)
      Dim bottomLine As Integer = RichTextBox1.GetLineFromCharIndex(bottomIndex)

      Dim numLinesDisplayed As Integer = bottomLine - topLine


                int firstVisibleLine = SendMessage(hWnd, EM_GETFIRSTVISIBLELINE, 0, 0);
                RECT rect = GetRect();
                int cix = GetCharFromPoint(rect.Left, rect.Bottom);
                int lastVisibleLine = SendMessage(hWnd, EM_LINEFROMCHAR, cix, 0);
                int n = lastVisibleLine - firstVisibleLine+1 ;
                return n;
            }
        }

#else
        public int NumberOfVisibleLines
        {
            get
            {
                int topIndex = _rtb.GetCharIndexFromPosition(new System.Drawing.Point(1, 1));
                int bottomIndex = _rtb.GetCharIndexFromPosition(new System.Drawing.Point(1, _rtb.Height - 1));
                int topLine = _rtb.GetLineFromCharIndex(topIndex);
                int bottomLine = _rtb.GetLineFromCharIndex(bottomIndex);
                int n = bottomLine - topLine + 1;
                return n;
            }
        }

#endif

    }
}


