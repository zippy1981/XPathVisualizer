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
        [DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wparam, int lparam);
        
        [DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        private static extern int SendMessageRef(IntPtr hwnd, int wMsg, out int wparam, out int lparam);

        [DllImport("User32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wparam, IntPtr lparam);

        // from WinUser.h and RichEdit.h
        private const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        private const int EM_GETSEL              = 0x00B0;
        private const int EM_SETSEL              = 0x00B1;
        private const int EM_LINESCROLL          = 0x00B6;
        private const int EM_GETCHARFORMAT       = 0x0400 + 58;
        private const int EM_SETCHARFORMAT       = 0x0400 + 68;
        private const int SCF_SELECTION          = 0x0001;
        private const int WM_SETREDRAW           = 0x000B;

        [ StructLayout( LayoutKind.Sequential )]
        private struct CHARFORMAT
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

        private System.Windows.Forms.RichTextBox _rtb;
        private IntPtr hWnd;
        private CHARFORMAT charFormat;
        private IntPtr lParam1;

        private int _savedScrollLine;
        private int _savedSelectionStart;
        private int _savedSelectionEnd;
        
        public RichTextBoxExtras(System.Windows.Forms.RichTextBox rtb)
        {
            hWnd = rtb.Handle;
            _rtb = rtb;
            charFormat = new CHARFORMAT()
                {
                    cbSize = Marshal.SizeOf(typeof(CHARFORMAT)),
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
            return SendMessage(hWnd, EM_GETFIRSTVISIBLELINE, 0, 0);
        }

        public void GetSelection(out int start, out int end)
        {
            SendMessageRef(hWnd, EM_GETSEL, out start, out end);
        }

        public void SetSelection(int start, int end)
        {
            SendMessage(hWnd, EM_SETSEL, start, end);
        }

        
        public void BeginUpdate()
        {
            SendMessage(hWnd, WM_SETREDRAW, 0, IntPtr.Zero);
        }
        
        public void EndUpdate()
        {
            SendMessage(hWnd, WM_SETREDRAW, 1, IntPtr.Zero);
        }
        

        public void BeginUpdateAndSaveState()
        {
            SendMessage(hWnd, WM_SETREDRAW, 0, IntPtr.Zero);
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
            SendMessage(hWnd, WM_SETREDRAW, 1, IntPtr.Zero);

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
            SendMessage(hWnd, EM_SETSEL, start, end);
            
            charFormat.dwMask = 0x40000000;
            charFormat.dwEffects = 0;
            charFormat.crTextColor = System.Drawing.ColorTranslator.ToWin32(color);
            
            Marshal.StructureToPtr(charFormat, lParam1, false);
            
            SendMessage(hWnd, EM_SETCHARFORMAT, SCF_SELECTION, lParam1);
        }

        public void Scroll(int delta)
        {
            SendMessage(hWnd,EM_LINESCROLL,0,delta);
        }

    }
}


