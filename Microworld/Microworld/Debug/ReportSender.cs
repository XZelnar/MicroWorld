using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace MicroWorld.Debug
{
    public partial class ReportSender : Form
    {
        internal String ErrorFileName = "";

        public ReportSender()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        bool isFeedback = false;
        public void InitForFeedback()
        {
            Text = "Feedback";
            label1.Text = "Enter in the form below any feedback you have.\r\n" +
                "Note that no personal information is included.";
            isFeedback = true;
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            MailMessage m = new MailMessage("mwgamedebug@gmail.com", "mwgamedebug@gmail.com");
            m.BodyEncoding = Encoding.Unicode;
            m.Subject = "Report";

            m.Body = "-=MW REPORT=-\r\n";
            m.Body += DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString() + "\r\n";
            m.Body += "MW Version " + Settings.VERSION + "\r\n";
            m.Body += "Memory used: " + (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024).ToString() + " mb\r\n";
            m.Body += "\r\n";
            m.Body += "====================DESCRIPTION====================\r\n";
            m.Body += "\r\n";
            m.Body += textBox1.Text;
            m.Body += "\r\n";
            m.Body += "\r\n";
            m.Body += "========================END========================\r\n";

            try
            {
                if (!isFeedback && Settings.Debug)
                {
                    m.Body += "\r\n[Attached debug.log]\r\n";
                    m.Attachments.Add(new Attachment("debug/debug.log"));
                }
            }
            catch { }
            try
            {
                if (!isFeedback && Settings.LogInput)
                {
                    m.Body += "\r\n[Attached io.log]\r\n";
                    m.Attachments.Add(new Attachment("debug/io.log"));
                }
            }
            catch { }
            try
            {
                if (!isFeedback && ErrorFileName != "" && System.IO.File.Exists(ErrorFileName))
                {
                    m.Body += "\r\n[Attached error file]\r\n";
                    m.Attachments.Add(new Attachment(ErrorFileName));
                }
            }
            catch { }
            bool saved = false;
            try
            {
                if (isFeedback && Main.curState.StartsWith("GAME"))
                {
                    try
                    {
                        IO.SaveEngine.SaveAll("Saves/attReport.sav");
                        saved = true;
                    }
                    catch { }
                    if (saved)
                    {
                        m.Body += "\r\n[Attached save file]\r\n";
                        m.Attachments.Add(new Attachment("Saves/attReport.sav"));
                    }
                }
            }
            catch { }

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            //smtp.Credentials = new NetworkCredential("mwgamedebug@gmail.com", "InsdfiJI56135@2");
            smtp.Port = 587;
            smtp.Timeout = 30000;
            smtp.EnableSsl = true;
            try
            {
                smtp.Send(m);
            }
            catch
            {
                try
                {
                    String fn = "Report" + DateTime.Now.Ticks.ToString() + ".err";
                    System.IO.BinaryWriter sw = new System.IO.BinaryWriter(
                        new System.IO.FileStream(fn, System.IO.FileMode.Create));
                    sw.Write(m.Body + "\r\n\r\n");
                    for (int i = 0; i < m.Attachments.Count; i++)
                    {
                        byte[] buf = new byte[m.Attachments[i].ContentStream.Length];
                        m.Attachments[i].ContentStream.Read(buf, 0, buf.Length);
                        String r = "";
                        for (int j = 0; j < buf.Length; j++)
                        {
                            r += ((char)buf[j]).ToString();
                        }
                        sw.Write(r + "\r\n\r\n");
                    }
                    sw.Close();
                    MessageBox.Show("Error sending report.\r\nPossible reasons include:\r\n -faulty or no internet connection\r\n -interferance of firewall or an antivirus software.\r\n\r\n"+
                        "Report has been saved to \"" + Application.StartupPath + "\\" + fn + "\".\r\nPlease, consider mailing it manually to\r\nmwgamedebug@gmail.com\r\nThanks");
                }
                catch { }
            }

            smtp.Dispose();
            m.Dispose();
            try
            {
                if (saved)
                    System.IO.File.Delete("Saves/attReport.sav");
            }
            catch { }
            Close();

            if (Settings.IsFullscreen && isFeedback)
            {
                var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Main.game.Window.Handle);
                form.WindowState = System.Windows.Forms.FormWindowState.Normal;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isFeedback)
                Close();
            else if (MessageBox.Show("This report might be just what we need to fix this bug.\r\nThe report contains no personal information about you.\r\nAre you sure you don't want to send it?", "Dismiss report?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes);
                Close();

            if (Settings.IsFullscreen && isFeedback)
            {
                var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Main.game.Window.Handle);
                form.WindowState = System.Windows.Forms.FormWindowState.Normal;
            }
        }

        private void ReportSender_Shown(object sender, EventArgs e)
        {
            Activate();
            BringToFront();
        }
    }
}
