
using System;
using System.Xml;         // for XmlReader, etc
using System.IO;          // StringReader

using System.Drawing;     // for Color
using System.ComponentModel;  // BackgroundWorker

namespace XPathVisualizer
{
    public partial class XPathVisualizerTool
    {
        private void backgroundWorker1_DoWork(object sender,
                                              DoWorkEventArgs e)
        {

            BackgroundWorker worker = sender as BackgroundWorker;

            _DoBackgroundColorizing((string)e.Argument, worker, e);
            e.Result = true;
        }



        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= this.progressBar1.Minimum &&
                e.ProgressPercentage <= this.progressBar1.Maximum)
                this.progressBar1.Value = e.ProgressPercentage;
            this.progressBar1.Visible = true;
        }


        // This event handler demonstrates how to interpret 
        // the outcome of the asynchronous operation implemented
        // in the DoWork event handler.
        private void backgroundWorker1_RunWorkerCompleted(object sender,
                                                          RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.lblStatus.Text = String.Format("Failed to load. ({0})", e.Error.Message);
            }
            if (e.Cancelled)
            {
                // nothing to do. 
            }
            else
            {
                backgroundWorker1 = null;
                stopWatch.Stop();

                // we may have updated the status bar in the interim... 
                if ("Highlighting..." == this.lblStatus.Text)
                {
                    TimeSpan ts = stopWatch.Elapsed;
                    this.lblStatus.Text =
                        String.Format("Loaded and Highlighted... {0:00}.{1:00}s",
                                      ts.Minutes * 60 + ts.Seconds,
                                      ts.Milliseconds / 10);
                }
                this.progressBar1.Visible = false;

            }
        }

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        public void ColorizeXml(System.Windows.Forms.RichTextBox rtb)
        {
            if (backgroundWorker1 != null)
            {
                // Cancel the asynchronous operation.
                this.backgroundWorker1.CancelAsync();
            }

            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += this.backgroundWorker1_ProgressChanged;
            backgroundWorker1.DoWork += this.backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += this.backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.RunWorkerAsync(rtb.Text);
        }


        private void SetTextColor(int start, int length, System.Drawing.Color color)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                this.richTextBox1.Invoke(new Action<int, int, Color>(this.SetTextColor),
                                         new object[] { start, length, color });
            }
            else
            {
                this.richTextBox1.Select(start, length);
                this.richTextBox1.SelectionColor = color;
            }
        }




        private void _DoBackgroundColorizing(string txt, BackgroundWorker self, DoWorkEventArgs e)
        {
            //string txt = rtb.Text;
            var lc = new LineCalculator(txt);
            float maxLines = (float)lc.CountLines();
            int reportingInterval =
                ((maxLines / 1000) > 10)
                ? (int)(maxLines / 1000)
                : ((maxLines / 100) > 10)
                ? (int)(maxLines / 100)
                : 1;
            int lastReport = -1;
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
                        // handle cancel
                        if (self.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }

                        // report progress
                        if ((rinfo.LineNumber / reportingInterval) > lastReport)
                        {
                            int pct = (int)((float)rinfo.LineNumber / maxLines * 100);
                            self.ReportProgress(pct);
                            lastReport = (rinfo.LineNumber / reportingInterval);
                        }

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                    +rinfo.LinePosition - 1;
                                SetTextColor(ix - 1, 1, Color.Blue);
                                SetTextColor(ix, reader.Name.Length, Color.DarkRed);

                                if (reader.HasAttributes)
                                {
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        //string s = reader.Value;
                                        ix = lc.GetCharIndexFromLine(rinfo.LineNumber - 1) +
                                            +rinfo.LinePosition - 1;
                                        SetTextColor(ix, reader.Name.Length, Color.Red);

                                        ix += reader.Name.Length;

                                        ix = txt.IndexOf('=', ix);

                                        // make the equals sign blue
                                        SetTextColor(ix, 1, Color.Blue);

                                        // skip over the quote char (it remains black)
                                        ix = txt.IndexOf(reader.QuoteChar, ix);
                                        ix++;
                                        // highlight the value of the attribute as blue
                                        if (txt.Substring(ix).StartsWith(reader.Value))
                                        {
                                            SetTextColor(ix, reader.Value.Length,
                                                         Color.Blue);
                                        }
                                        else
                                        {
                                            // Difference in escaping.  The InnerXml may include
                                            // \" where &quot; is in the doc.
                                            string s = reader.Value.XmlEscapeQuotes();
                                            int delta = s.Length - reader.Value.Length;
                                            SetTextColor(ix, reader.Value.Length + delta,
                                                         Color.Blue);
                                        }

                                    }
                                    while (reader.MoveToNextAttribute());

                                    ix = txt.IndexOf('>', ix);

                                    // the close-angle-bracket
                                    if (txt[ix - 1] == '/')
                                        SetTextColor(ix - 1, 2, Color.Blue);
                                    else
                                        SetTextColor(ix, 1, Color.Blue);
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
                                    +rinfo.LinePosition - 1;
                                SetTextColor(ix - 2, 2, Color.Blue);
                                SetTextColor(ix, reader.Name.Length, Color.DarkRed);
                                SetTextColor(ix + reader.Name.Length, 1, Color.Blue);
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
                                    +rinfo.LinePosition - 1;
                                SetTextColor(ix, reader.Value.Length, Color.Green);
                                break;
                        }
                    }


                }
            }
        }
    }
}

