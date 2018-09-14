using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;

using MailKit.Net.Imap;
using MailKit;
using MimeKit;

using System.Data.SQLite;

namespace MAIL_IMAP
{
    public partial class Form1 : Form
    {
        private int licznik = 0;
        private int wiadomosci = 0;
        private int liczba;
        private string rozszerzenia;
        private int autostart = 20;

        //private string b;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.MarqueeAnimationSpeed = 0;
        }

        private void Form1_Load(object sender, EventArgs a)
        {
            timer1.Interval = 1000;
            timer3.Interval = 1000;

            timer3.Start();

            label14.Text = " ";

            liczba = Convert.ToInt32(textBox5.Text);

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = false;

            label12.BackColor = System.Drawing.Color.Red;
            label12.ForeColor = System.Drawing.Color.Red;
            label13.Text = " ";

            Directory.CreateDirectory("attachments");
            if (!File.Exists("imap_db"))
            {
                utworz_baza();
            }


            button5.PerformClick();
        }

        private void button1_Click(object sender, EventArgs a)
        {
            timer3.Stop();
            label13.Text = " ";

            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;

            richTextBox1.Clear();

            backgroundWorker1.RunWorkerAsync();

            textBox2.ReadOnly = true;
            textBox2.BackColor = System.Drawing.SystemColors.Control;

            textBox3.ReadOnly = true;
            textBox3.BackColor = System.Drawing.SystemColors.Control;

            textBox4.ReadOnly = true;
            textBox4.BackColor = System.Drawing.SystemColors.Control;

            textBox5.ReadOnly = true;
            textBox5.BackColor = System.Drawing.SystemColors.Control;

            textBox6.ReadOnly = true;
            textBox6.BackColor = System.Drawing.SystemColors.Control;

            textBox7.ReadOnly = true;
            textBox7.BackColor = System.Drawing.SystemColors.Control;

            System.Threading.Thread.Sleep(1000);

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;

            label8.Select();

            rozszerzenia = textBox6.Text;
            string[] rozszerzenia2 = rozszerzenia.Split(';', ',');

            var l = from s in rozszerzenia2
                    select s;
            int b = l.Count();

            var client = new ImapClient();
            var port = Convert.ToInt32(textBox7.Text);
            //MessageBox.Show(port.ToString());

            //client.ServerCertificateValidationCallback = (s, c, h, r) => true;
            //client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Connect(textBox2.Text, port, true);

            client.Authenticate(textBox3.Text, textBox4.Text);

            var inbox = client.Inbox;
            var inbox2 = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            label2.Text = inbox.Count.ToString();
            //progressBar1.Maximum = inbox.Count;
            //progressBar1.Value = 0;

            //System.Threading.Thread.Sleep(100);

            var messages = client.Inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
            int unnamed = 0;

            foreach (var message in messages)
            {
                var multipart = message.Body as BodyPartMultipart;
                var basic = message.Body as BodyPartBasic;
                var temat = message.NormalizedSubject;
                var data = message.Date.DateTime;
                var ticks_data = message.Date.Ticks;

                //MessageBox.Show(ticks_data.ToString());

                if (multipart != null)
                {
                    foreach (var attachment in multipart.BodyParts.OfType<BodyPartBasic>())
                    {
                        var mime = (MimePart)client.Inbox.GetBodyPart(message.UniqueId, attachment);

                        var fileName = mime.FileName;
                        var sciezka = Directory.GetCurrentDirectory() + @"\attachments\" + ticks_data + "_" + mime.FileName;
                        //MessageBox.Show(b);

                        if (string.IsNullOrEmpty(fileName))
                        {
                            richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nnie znaleziono załącznika. \r\n\r\n";
                        }
                        else if (File.Exists(sciezka) != true & sprawdz_baza(ticks_data.ToString(), data.ToString(), mime.FileName.ToString()) == 0)
                        {
                            switch (b)
                            {
                                case 1:
                                    if ((fileName.Contains("." + rozszerzenia2[0])))
                                    {
                                        using (var stream = File.Create(sciezka))
                                            mime.ContentObject.DecodeTo(stream);
                                        data_pliku(sciezka, data.ToString());
                                        richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n\r\n";
                                        wiadomosci = wiadomosci + 1;
                                        label7.Text = wiadomosci.ToString();
                                        dodaj_baza(ticks_data.ToString(), data.ToString(), mime.FileName.ToString());

                                        //if (progressBar1.Maximum < progressBar1.Value + 1)
                                        //{
                                        //    progressBar1.Value = progressBar1.Value + 1;
                                        //}
                                    }
                                    break;
                                case 2:
                                    if ((fileName.Contains("." + rozszerzenia2[0])) | (fileName.Contains("." + rozszerzenia2[1])))
                                    {
                                        using (var stream = File.Create(sciezka))
                                            mime.ContentObject.DecodeTo(stream);
                                        data_pliku(sciezka, data.ToString());
                                        richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n\r\n";
                                        wiadomosci = wiadomosci + 1;
                                        label7.Text = wiadomosci.ToString();
                                        dodaj_baza(ticks_data.ToString(), data.ToString(), mime.FileName.ToString());

                                        //if (progressBar1.Maximum < progressBar1.Value + 1)
                                        //{
                                        //    progressBar1.Value = progressBar1.Value + 1;
                                        //}
                                    }
                                    break;
                                case 3:
                                    if ((fileName.Contains("." + rozszerzenia2[0])) | (fileName.Contains("." + rozszerzenia2[1])) | (fileName.Contains("." + rozszerzenia2[2])))
                                    {
                                        using (var stream = File.Create(sciezka))
                                            mime.ContentObject.DecodeTo(stream);
                                        data_pliku(sciezka, data.ToString());
                                        richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n\r\n";
                                        wiadomosci = wiadomosci + 1;
                                        label7.Text = wiadomosci.ToString();
                                        dodaj_baza(ticks_data.ToString(), data.ToString(), mime.FileName.ToString());

                                        //if (progressBar1.Maximum < progressBar1.Value + 1)
                                        //{
                                        //    progressBar1.Value = progressBar1.Value + 1;
                                        //}
                                    }
                                    break;
                                case 4:
                                    if ((fileName.Contains("." + rozszerzenia2[0])) | (fileName.Contains("." + rozszerzenia2[1])) | (fileName.Contains("." + rozszerzenia2[2])) | (fileName.Contains("." + rozszerzenia2[3])))
                                    {
                                        using (var stream = File.Create(sciezka))
                                            mime.ContentObject.DecodeTo(stream);
                                        data_pliku(sciezka, data.ToString());
                                        richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n\r\n";
                                        wiadomosci = wiadomosci + 1;
                                        label7.Text = wiadomosci.ToString();
                                        dodaj_baza(ticks_data.ToString(), data.ToString(), mime.FileName.ToString());

                                        //if (progressBar1.Maximum < progressBar1.Value + 1)
                                        //{
                                        //    progressBar1.Value = progressBar1.Value + 1;
                                        //}
                                    }
                                    break;
                            }

                        }
                        else
                        {
                            richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n -- DUPLIKAT (W PLIKU/BAZIE) -- \r\n\r\n";
                            
                            
                            //switch (b)
                            //{
                            //    case 1:
                            //        if (fileName.Contains("." + rozszerzenia2[0]))
                            //        {
                            //            richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n -- DUPLIKAT -- \r\n\r\n";

                            //            //if (progressBar1.Maximum < progressBar1.Value + 1)
                            //            //{
                            //            //    progressBar1.Value = progressBar1.Value + 1;
                            //            //}
                            //        }
                            //        break;
                            //    case 2:
                            //        if ((fileName.Contains("." + rozszerzenia2[0])) | (fileName.Contains("." + rozszerzenia2[1])))
                            //        {
                            //            richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n -- DUPLIKAT -- \r\n\r\n";

                            //            //if (progressBar1.Maximum < progressBar1.Value + 1)
                            //            //{
                            //            //    progressBar1.Value = progressBar1.Value + 1;
                            //            //}
                            //        }
                            //        break;
                            //    case 3:
                            //        if ((fileName.Contains("." + rozszerzenia2[0])) | (fileName.Contains("." + rozszerzenia2[1])) | (fileName.Contains("." + rozszerzenia2[2])))
                            //        {
                            //            richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n -- DUPLIKAT -- \r\n\r\n";

                            //            //if (progressBar1.Maximum < progressBar1.Value + 1)
                            //            //{
                            //            //    progressBar1.Value = progressBar1.Value + 1;
                            //            //}
                            //        }
                            //        break;
                            //    case 4:
                            //        if ((fileName.Contains("." + rozszerzenia2[0])) | (fileName.Contains("." + rozszerzenia2[1])) | (fileName.Contains("." + rozszerzenia2[2])) | (fileName.Contains("." + rozszerzenia2[3])))
                            //        {
                            //            richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + " ----- Data: " + data + "\r\nZałącznik: " + fileName.ToString() + "\r\n -- DUPLIKAT -- \r\n\r\n";

                            //            //if (progressBar1.Maximum < progressBar1.Value + 1)
                            //            //{
                            //            //    progressBar1.Value = progressBar1.Value + 1;
                            //            //}
                            //        }
                            //        break;
                            //}

                        }
                    }
                }
                else if (basic != null && basic.IsAttachment)
                {
                    var liczba = inbox2.Count - 1;
                    label2.Text = liczba.ToString();

                    var mime = (MimePart)client.Inbox.GetBodyPart(message.UniqueId, basic);
                    var fileName = mime.FileName;
                    var sciezka = Directory.GetCurrentDirectory() + @"\attachments\" + mime.FileName;

                    if (string.IsNullOrEmpty(fileName))
                        fileName = string.Format("unnamed-{0}", ++unnamed);

                    using (var stream = File.Create(sciezka))
                        mime.ContentObject.DecodeTo(stream);
                    wiadomosci = wiadomosci + 1;
                    label7.Text = wiadomosci.ToString();
                    richTextBox1.Text = richTextBox1.Text + "Temat: " + temat + "\r\nZałącznik: " + fileName.ToString() + "\r\n\r\n";
                    //progressBar1.Value = progressBar1.Value + 1;
                }
            }

            //richTextBox1.Text = richTextBox1.Text + "========== OCZEKIWANIE ========== ";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer3.Stop();
            label13.Text = " ";

            liczba = Convert.ToInt32(textBox5.Text);

            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;

            richTextBox1.Clear();

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;

            textBox2.ReadOnly = true;
            textBox2.BackColor = System.Drawing.SystemColors.Control;

            textBox3.ReadOnly = true;
            textBox3.BackColor = System.Drawing.SystemColors.Control;

            textBox4.ReadOnly = true;
            textBox4.BackColor = System.Drawing.SystemColors.Control;

            textBox5.ReadOnly = true;
            textBox5.BackColor = System.Drawing.SystemColors.Control;

            textBox6.ReadOnly = true;
            textBox6.BackColor = System.Drawing.SystemColors.Control;

            textBox7.ReadOnly = true;
            textBox7.BackColor = System.Drawing.SystemColors.Control;

            label8.Select();

            timer1.Interval = Convert.ToInt32(textBox5.Text) * 1000;
            timer2.Interval = 1000;

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                System.Threading.Thread.Sleep(500);
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.CancelAsync();
            }

            timer1.Start();
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void timer2_Tick_1(object sender, EventArgs e)
        {
            if (liczba > 0)
            {
                label8.Text = liczba.ToString();
                liczba = liczba - 1;
            }
            else
            {
                licznik = licznik + 1;

                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                    if (backgroundWorker1.IsBusy)
                    {
                        backgroundWorker1.WorkerSupportsCancellation = true;
                        backgroundWorker1.CancelAsync();
                    }
                }

                liczba = Convert.ToInt32(textBox5.Text);

                if (licznik > 5)
                {
                    richTextBox1.Clear();
                }

                richTextBox1.Text = richTextBox1.Text + "==========  PRZEBIEG: " + licznik + "  ========== \r\n\r\n";


            }

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            liczba = Convert.ToInt32(textBox5.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.CancelAsync();

            timer1.Stop();
            timer2.Stop();
            timer3.Stop();

            textBox2.ReadOnly = false;
            textBox2.BackColor = System.Drawing.SystemColors.InactiveBorder;

            textBox3.ReadOnly = false;
            textBox3.BackColor = System.Drawing.SystemColors.InactiveBorder;

            textBox4.ReadOnly = false;
            textBox4.BackColor = System.Drawing.SystemColors.InactiveBorder;

            textBox5.ReadOnly = false;
            textBox5.BackColor = System.Drawing.SystemColors.InactiveBorder;

            textBox6.ReadOnly = false;
            textBox6.BackColor = System.Drawing.SystemColors.InactiveBorder;

            textBox7.ReadOnly = false;
            textBox7.BackColor = System.Drawing.SystemColors.InactiveBorder;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;

            label8.Text = textBox5.Text;
            label2.Text = 0.ToString();
            label7.Text = 0.ToString();
            label14.Text = " ";

            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.MarqueeAnimationSpeed = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.IP = textBox2.Text;
            Properties.Settings.Default.Login = textBox3.Text;
            Properties.Settings.Default.Haslo = textBox4.Text;
            Properties.Settings.Default.Synch = textBox5.Text;
            Properties.Settings.Default.Synch = textBox5.Text;
            Properties.Settings.Default.Rozszerzenia = textBox6.Text;

            Properties.Settings.Default.Save();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    var port = Convert.ToInt32(textBox7.Text);
                    //MessageBox.Show(port.ToString());

                    client.ServerCertificateValidationCallback = (s, c, h, r) => true;
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Connect(textBox2.Text, port, true);

                    client.Authenticate(textBox3.Text, textBox4.Text);
                    client.Disconnect(true);
                }

                MessageBox.Show("Połącznie ustanowione!");

                label14.ForeColor = System.Drawing.Color.ForestGreen;
                label14.Text = "OK!";

                button4.Enabled = true;
                button4.ForeColor = System.Drawing.Color.ForestGreen;

                label12.BackColor = System.Drawing.Color.Lime;
                label12.ForeColor = System.Drawing.Color.Lime;

                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            catch (Exception ex) {
                MessageBox.Show("Połącznie nieudane!"); 

                label14.ForeColor = System.Drawing.Color.Maroon;
                label14.Text = "BŁĄD!";

                label12.BackColor = System.Drawing.Color.Red;
                label12.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (autostart >= 1)
            {
                autostart = autostart - 1;
                label13.Text = autostart.ToString();
            }
            else if (autostart == 0 & label14.Text == "OK!")
            {
                button2.PerformClick();
            }
        }   

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info; //Shows the info icon so the user doesn't thing there is an error.
                this.notifyIcon.BalloonTipText = "Aplikacja kontynuuje działanie w tle.";
                this.notifyIcon.BalloonTipTitle = "IMAP IMPORTER";
             
                this.notifyIcon.Text = "Kolejka nr: " + licznik.ToString();

                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
        }

        //
        //      BAZA, TWORZENIE, DODWANIA, SELECT
        //

        private void utworz_baza()
        {
            SQLiteConnection.CreateFile("imap_db");
            SQLiteConnection m_dbConnection;
             m_dbConnection = new SQLiteConnection("Data Source=imap_db.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "CREATE TABLE Pliki (ID VARCHAR(50), Data VARCHAR(50), Plik VARCHAR(300), Plik_calosc VARCHAR(300))";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            m_dbConnection.Close();
        }

        private void dodaj_baza(string ID_value, string Data_value, string Plik_value)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=imap_db.sqlite;Version=3;");
            m_dbConnection.Open();

            string uq_name = ID_value + "_" + Plik_value;
         
            string sql2 = "SELECT count(*) FROM Pliki WHERE Plik_calosc='" + uq_name + "'";
            SQLiteCommand command = new SQLiteCommand(sql2, m_dbConnection);
            int count = Convert.ToInt32(command.ExecuteScalar());
            if (count == 0)
            {
                string sql = "insert into Pliki (ID, Data, Plik, Plik_calosc) values ('" + ID_value + "','" + Data_value + "','" + Plik_value + "','" + uq_name + "')";
                SQLiteCommand command2 = new SQLiteCommand(sql, m_dbConnection);
                command2.ExecuteNonQuery();
                m_dbConnection.Close();
            }
        }

        public int sprawdz_baza(string ID_value, string Data_value, string Plik_value)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=imap_db.sqlite;Version=3;");
            m_dbConnection.Open();

            string uq_name = ID_value + "_" + Plik_value;

            string sql2 = "SELECT count(*) FROM Pliki WHERE Plik_calosc='" + uq_name + "'";
            SQLiteCommand command = new SQLiteCommand(sql2, m_dbConnection);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count;
        }

        public void data_pliku (string sciezka, string data)
        {
            var data2 = Convert.ToDateTime(data);

            File.SetCreationTime(sciezka, data2);
            File.SetLastWriteTime(sciezka, data2);
            File.SetLastAccessTime(sciezka, data2);
        }

        //private void button6_Click(object sender, EventArgs e)
        //{
            
        //    utworz_baza();
        //}

        //private void button7_Click(object sender, EventArgs e)
        //{
        //    dodaj_baza("555", "3232323", "nazwanazwa.jpg");
        //}
    }
}
