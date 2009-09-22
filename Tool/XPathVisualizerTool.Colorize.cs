// XPathVisualizerTool.Colorize.cs
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
using System.Xml;                 // for XmlReader, etc
using System.Collections.Generic; // List
using System.IO;                  // StringReader
using System.Threading;           // ManualResetEvent
using System.Drawing;             // for Color
using System.ComponentModel;      // BackgroundWorker
using System.Runtime.InteropServices;

namespace XPathVisualizer
{
    public partial class XPathVisualizerTool
    {

        /// <summary>
        ///   Update the progressbar
        /// </summary>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= this.progressBar1.Minimum &&
                e.ProgressPercentage <= this.progressBar1.Maximum)
                this.progressBar1.Value = e.ProgressPercentage;

            this.progressBar1.Visible = (e.ProgressPercentage != 100);
        }


        
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        public void KickoffColorizer()
        {
            if (backgroundWorker1 != null)
                return;

            // this worker never completes, never returns
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.WorkerSupportsCancellation = false;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += this.backgroundWorker1_ProgressChanged;
            backgroundWorker1.DoWork += this.DoBackgroundColorizing;
            backgroundWorker1.RunWorkerAsync();
        }


                
        public class FormatChange
        {
            public FormatChange(int start, int length, System.Drawing.Color color)
            {
                Start = start;
                Length = length;
                ForeColor = color;
            }
            
            public int Start;
            public int Length;
            public System.Drawing.Color ForeColor;
        }

        
        
        private RichTextBoxExtras _rtbe;
        private RichTextBoxExtras rtbe
        {
            get
            {
                if (_rtbe == null)
                {
                    _rtbe= new RichTextBoxExtras(this.richTextBox1);
                }
                return _rtbe;
            }
        }

        
        private void ApplyChanges(List<FormatChange> list)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                this.richTextBox1.Invoke(new Action<List<FormatChange>>(this.ApplyChanges),
                                         new object[] { list });
            }
            else
            {
                // The RichTextBox is a RichEdit Win32 control. When the
                // selection changes and it has focus, the control will
                // auto-scroll.  The way to prevent that is to call
                // BeginUpdate/EndUpdate.
                rtbe.BeginUpdateAndSaveState();
                
                foreach (var change in list)
                {
                    rtbe.SetSelectionColor(change.Start, change.Start+change.Length, change.ForeColor);
                }

                rtbe.EndUpdateAndRestoreState();
            }
        }


        
        private void ResetBackground()
        {
            if (this.richTextBox1.InvokeRequired)
            {
                this.richTextBox1.Invoke(new Action(this.ResetBackground));
            }
            else
            {
                rtbe.BeginUpdateAndSaveState();
                
                this.richTextBox1.SelectAll();
                this.richTextBox1.SelectionBackColor = Color.White;

                rtbe.EndUpdateAndRestoreState();
            }
        }

        private const int DELAY_IN_MILLISECONDS = 650;
        private int progressCount = 0;


        
        private void DoBackgroundColorizing(object sender, DoWorkEventArgs e)
        {
            // Design Notes:
            // ----------------------
            //
            // It takes a long time, maybe 10s or more, to colorize the XML syntax
            // in an xml file 100k in size.  Therefore the approach we take is to
            // perform the syntax highlighting asynchronously.
            //
            // This method runs endlessly.  The first thing it does is wait for a
            // signal on the wantFormat event.  This event is set when an XMl file
            // is loaded, or when the rtb text is changed.
            //
            // When the signal is received, execution continues, and the
            // highlighting begins. It reads a segment of the XML, and decides how
            // to highlight it.  The change is then placed into a list, and then
            // the next segment of XML is read in.
            //
            // On an interval that is normally every 1/33rd of the lines - if
            // there are 330 lines, then every 10 lines - this method calls the
            // progress update for the BG worker, and also applies the queued
            // changes.  Using this approach the progress bar magically appears
            // while highlighting is happening, and disappears when highlighting
            // finishes.
            //
            // After calling the progress update method, we call the ApplyChanges
            // method. It saves the scroll and selection state, applies all
            // formatting changes to the rtb text, restores the scroll and
            // selection state, and then calls Refresh() on the RTB.  After that
            // method returns, this method clears the list and continues reading
            // the XML.
            //
            // If at any time, a change is detected in the RTB Text, the
            // wantFormat event is signalled once more.  During reading of the XML
            // this is interpreted as a "cancel-and-restart" message.  When
            // receiving that signal, this method starts reading and highlighting
            // at the beginning again.
            //
            // The reason I batch up changes is that the control.Invoke() method
            // can be costly. So I'd like to amortize the cost of it across a
            // batch of format changes.
            //
            // When it finishes highlighting, this method waits for the wantFormat
            // signal again.
            //
            BackgroundWorker self = sender as BackgroundWorker;
            do
            {
                try
                {
                    wantFormat.WaitOne();
                    wantFormat.Reset();
                    progressCount = 0; 
    
                    //StoreCaretPosition();
                    var list = new List<FormatChange>();

                    // we want a re-format, but let's wait til
                    // the user stops typing...
                    if (_lastRtbKeyPress != _originDateTime)
                    {
                        System.Threading.Thread.Sleep(DELAY_IN_MILLISECONDS);
                        System.DateTime now = System.DateTime.Now;
                        var _delta = now - _lastRtbKeyPress;
                        if (_delta < new System.TimeSpan(0, 0, 0, 0, DELAY_IN_MILLISECONDS))
                            continue;
                    }
                    
                    //string txt = (string)e.Argument;
                    string txt = (this.richTextBox1.InvokeRequired)
                        ? (string)this.richTextBox1.Invoke((System.Func<string>)(() => this.richTextBox1.Text))
                        : this.richTextBox1.Text;

                    ResetBackground();

                    var lc = new LineCalculator(txt);
                    float maxLines = (float) lc.CountLines();

                    int reportingInterval = (maxLines > 64)
                        ? (int)(maxLines / 32)
                        : 1;
                    
                    int lastReport = -1;
                    var sr = new StringReader(txt);
                    XmlReader reader = XmlReader.Create(sr);

                    IXmlLineInfo rinfo = (IXmlLineInfo)reader;
                    if (!rinfo.HasLineInfo()) continue;

                    int ix = 0;
                    while (reader.Read())
                    {
                        // If another format is pending, that means
                        // the text has changed and we should stop this
                        // formatting effort and start again. 
                        if ( wantFormat.WaitOne(1, false))
                            break;

                        // report progress
                        if ((rinfo.LineNumber / reportingInterval) > lastReport)
                        {
                            int pct = (int)((float)rinfo.LineNumber / maxLines * 100);
                            self.ReportProgress(pct);
                            lastReport = (rinfo.LineNumber / reportingInterval);
                            ApplyChanges(list);
                            list.Clear();
                            progressCount++;
                        }

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    + rinfo.LinePosition - 1;

                                list.Add(new FormatChange(ix - 1, 1, Color.Blue));
                                //HighlightText(ix - 1, 1, Color.Blue);
                                list.Add(new FormatChange(ix,      reader.Name.Length, Color.DarkRed));
                                //HighlightText(ix, reader.Name.Length, Color.DarkRed);

                                if (reader.HasAttributes)
                                {
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        //string s = reader.Value;
                                        ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                            + rinfo.LinePosition - 1;
                                        list.Add(new FormatChange(ix, reader.Name.Length, Color.Red));
                                        //HighlightText(ix, reader.Name.Length, Color.Red);

                                        ix += reader.Name.Length;

                                        ix = txt.IndexOf('=', ix);

                                        // make the equals sign blue
                                        list.Add(new FormatChange(ix, 1, Color.Blue));
                                        //HighlightText(ix, 1, Color.Blue);

                                        // skip over the quote char (it remains black)
                                        ix = txt.IndexOf(reader.QuoteChar, ix);
                                        ix++;
                                        // highlight the value of the attribute as blue
                                        if (txt.Substring(ix).StartsWith(reader.Value))
                                        {
                                            list.Add(new FormatChange(ix, reader.Value.Length, Color.Blue));
                                            //HighlightText(ix, reader.Value.Length, Color.Blue);
                                        }
                                        else
                                        {
                                            // Difference in escaping.  The InnerXml may include
                                            // \" where &quot; is in the doc.
                                            string s = reader.Value.XmlEscapeQuotes();
                                            int delta = s.Length - reader.Value.Length;
                                            list.Add(new FormatChange(ix, reader.Value.Length + delta, Color.Blue));
                                            //HighlightText(ix, reader.Value.Length + delta, Color.Blue);
                                        }

                                    }
                                    while (reader.MoveToNextAttribute());

                                    ix = txt.IndexOf('>', ix);

                                    // the close-angle-bracket
                                    if (txt[ix - 1] == '/')
                                    {
                                        list.Add(new FormatChange(ix - 1, 2, Color.Blue));
                                        //HighlightText(ix - 1, 2, Color.Blue);
                                    }
                                    else
                                    {
                                        list.Add(new FormatChange(ix, 1, Color.Blue));
                                        //HighlightText(ix, 1, Color.Blue);
                                    }
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

                                //HighlightText(ix - 2, 2, Color.Blue);
                                list.Add(new FormatChange(ix - 2, 2, Color.Blue));
                                //HighlightText(ix, reader.Name.Length, Color.DarkRed);
                                list.Add(new FormatChange(ix, reader.Name.Length, Color.DarkRed));
                                //HighlightText(ix + reader.Name.Length, 1, Color.Blue);
                                list.Add(new FormatChange(ix + reader.Name.Length, 1, Color.Blue));
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
                                //HighlightText(ix, reader.Value.Length, Color.Green);
                                list.Add(new FormatChange(ix, reader.Value.Length, Color.Green));
                                break;
                        }
                    }

                    // in case there are more 
                    ApplyChanges(list);
                    self.ReportProgress(100);
                } 
                catch (Exception exc1)
                {
                    Console.WriteLine("Exception: " + exc1.Message);
                }
                finally
                {
                    //RestoreCaretPosition();
                }
            }
            while (true);

        } 
    }
}

