using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Zeiterfassung
{
    public partial class Form1 : Form
    {
        #region Fields

        public bool Ausgeklappt = false;
        private Messung Zeit = new Messung();

        private float progress1value = 0;
        private float progress2value = 0;
        private float progress1max = 0;
        private float progress2max = 0;

        #endregion Fields

        #region Constructors

        public Form1()
        {
            InitializeComponent();
            DateTime Time = DateTime.Now;
            selectedMonth = Time.Month;
            this.Hide();
        }

        #endregion Constructors

        #region Methods

        int selectedMonth;

        public void drawFields()
        {
            label1.Text = "Heute: " + (Zeit.Heute.Days*24+Zeit.Heute.Hours).ToString().PadLeft(2, '0') + ":" + Zeit.Heute.Minutes.ToString().PadLeft(2, '0') + ":" + Zeit.Heute.Seconds.ToString().PadLeft(2, '0');
            label2.Text = "Monat: " + (Zeit.Monat.Days*24+Zeit.Monat.Hours).ToString().PadLeft(2, '0') + ":" + Zeit.Monat.Minutes.ToString().PadLeft(2, '0') + ":" + Zeit.Monat.Seconds.ToString().PadLeft(2, '0');
            progress2value = (int)((60 * 60 * 30) * ((float)(DateTime.Now.Day*24*60*60 + DateTime.Now.Hour * 60 * 60 + DateTime.Now.Minute * 60 + DateTime.Now.Second) / (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) * 24 * 60 * 60)));
            resizeFields();
        }

        public void hideFields()
        {
            label1.Hide();
            label2.Hide();
            panel1.Hide();
            panel2.Hide();
            panel3.Hide();
            panel4.Hide();
            pictureBox1.Hide();
        }

        public void initialisieren()
        {
            if (Directory.Exists("Daten"))
            {
                Zeit.Heute = new TimeSpan();
                Zeit.Monat = new TimeSpan();

                string[] Dateien = Directory.GetFiles("Daten");
                DateTime Time = DateTime.Now;
                String Jahr = Time.Year.ToString();
                String Monat = selectedMonth.ToString().PadLeft(2, '0');
                String Tag = Time.Day.ToString().PadLeft(2, '0');
                String DateinameHeute = "Daten\\" + Jahr + "_" + Monat + "_" + Tag + ".dat";
                String DateinameMonat = "Daten\\" + Jahr + "_" + Monat + "_";

                for (int i = 0; i < Dateien.Length; i++)
                {
                    if (DateinameHeute == Dateien[i])
                    {
                        // heute ermitteln
                        FileInfo data = new FileInfo(Dateien[i]);
                        Zeit.Heute += TimeSpan.FromSeconds(data.Length);
                    }

                    if (Dateien[i].Substring(0, DateinameMonat.Length) == DateinameMonat)
                    {
                        FileInfo data = new FileInfo(Dateien[i]);
                        // monat ermitteln
                        Zeit.Monat += TimeSpan.FromSeconds(data.Length);
                    }
                }
                progress1value = (int)Zeit.Monat.TotalSeconds;
            }
        }

        public void resizeFields()
        {
            label1.Left = button1.Left+button1.Width+40;
            label2.Left = label1.Left + label1.Width + 40;
            label1.Top = 0;
            label2.Top = 0;

            panel1.Left = label2.Left + label2.Width + 40;
            panel1.Width = this.Width - 40 - (label2.Left + label2.Width + 40) - (this.Width-button4.Left);
            panel1.Height = button1.Height / 2;
            panel1.Top = 0;

            panel3.Left = 0;
            panel3.Width = (int)(panel1.Width * ((float)progress1value / progress1max));
            panel3.Height = button1.Height / 2;
            panel3.Top = 0;
            panel3.BackColor = Color.Green;
            panel3.BringToFront();

            panel2.Left = label2.Left + label2.Width + 40;
            panel2.Width = this.Width - 40 - (label2.Left + label2.Width + 40) - (this.Width - button4.Left);
            panel2.Height = button1.Height / 2;
            panel2.Top = button1.Height / 2;

            panel4.Left = 0;
            panel4.Width = (int)(panel2.Width * ((float)progress2value / progress2max));
            panel4.Height = button1.Height / 2;
            panel4.Top = 0;
            panel4.BackColor = Color.Gold;
            panel4.BringToFront();

            pictureBox1.Left = 20;
            pictureBox1.Top = button1.Top + button1.Height + 20;
            pictureBox1.Width = this.Width - 40;
            pictureBox1.Height = this.Height - pictureBox1.Top - 20;
        }

        public void showFields()
        {
            resizeFields();
            label1.Show();
            label2.Show();
            panel1.Show();
            panel2.Show();
            panel3.Show();
            panel4.Show();
            pictureBox1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Zeit.Zustand)
            {
                Zeit.Abschalten();
                button1.Text = "Start";
                button1.ForeColor = Color.Black;
                timer2.Enabled = false;
                timer3.Enabled = false;
            }
            else
            {
                Zeit.Einschalten();
                button1.Text = "Stop";
                timer2.Enabled = true;
                timer3.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Ausgeklappt)
            {
                hideFields();
                Ausgeklappt = false;
                button2.Text = ">";
                this.Width = button1.Width + button2.Width + button3.Width;
                this.Height = button1.Height;
            }
            else
            {
                this.Width = 600;// Screen.PrimaryScreen.WorkingArea.Width;
                this.Height = Screen.PrimaryScreen.WorkingArea.Height/2;
                button2.Text = "<";
                Ausgeklappt = true;
                showFields();
                drawFields();
                resizeFields();
                drawStat();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Height = button1.Height;
            this.Width = button1.Width + button2.Width + button3.Width;
            progress1value = 0;
            progress1max = 60 * 60 * 30;
            progress2max = 60 * 60 * 30;
            progress2value = (int)((60 * 60 * 30) * ((float)DateTime.Now.Day / DateTime.DaysInMonth(DateTime.Now.Year, selectedMonth)));

            initialisieren();
            this.Show();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (button1.ForeColor == Color.Black)
            {
                button1.ForeColor = Color.Red;
            }
            else
            {
                button1.ForeColor = Color.Black;
            }
            {
                TimeSpan sec = TimeSpan.FromSeconds(1);
                Zeit.Heute += sec;
                Zeit.Monat += sec;
            }

            if (Zeit.Heute.Seconds == 0)
                initialisieren();

            if (Ausgeklappt)
            {
                drawFields();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            DateTime Time = DateTime.Now;
            String Dateiname = "Daten\\" + Time.Year.ToString() + "_" + selectedMonth.ToString().PadLeft(2, '0') + "_" + Time.Day.ToString().PadLeft(2, '0') + ".dat";
            String Data = Time.Hour.ToString().PadLeft(2, '0') + ":" + Time.Minute.ToString().PadLeft(2, '0') + ":" + Time.Second.ToString().PadLeft(2, '0');

            if (!Directory.Exists("Daten"))
                Directory.CreateDirectory("Daten");

            StreamWriter Datei = new StreamWriter(Dateiname, true);
            Datei.WriteLine(Data);
            Datei.Close();

            Datei.Dispose();
            System.GC.Collect();
        }

        #endregion Methods

        public void drawStat()
        {
            double tageMonat = DateTime.DaysInMonth(DateTime.Now.Year, selectedMonth);
            float width = pictureBox1.Width;
            float height = pictureBox1.Height;
            Bitmap b1 = new Bitmap((int) width, (int) height);
            Graphics g = Graphics.FromImage(b1);

            for (int i = 1; i <= tageMonat; i++)
            {
                DateTime Time = DateTime.Now;
                String Jahr = Time.Year.ToString();
                String Monat = selectedMonth.ToString().PadLeft(2, '0');
                String Tag = i.ToString().PadLeft(2, '0');
                String Dateiname = "Daten\\" + Jahr + "_" + Monat + "_" + Tag + ".dat";
                double posY = pictureBox1.Height * ((float)(i - 1) / tageMonat);
                double posY2 = pictureBox1.Height * ((float)i / tageMonat);


                if (i % 2 == 0)
                {
                    Brush c = Brushes.LightGray;
                    g.FillRectangle(c, new Rectangle((int)0, (int)posY+1, (int)width, (int)(posY2 - posY + 1)));
                }

                if (i == Time.Day)
                {
                    Brush c = Brushes.LightCoral;
                    g.FillRectangle(c, new Rectangle((int)0, (int)posY + 1, (int)width, (int)(posY2 - posY + 1)));
                }

                if (File.Exists(Dateiname))
                {
                    StreamReader Datei = new StreamReader(Dateiname);
                    double begin = -1;
                    double end = -1;
                    while (!Datei.EndOfStream)
                    {
                        String Data = Datei.ReadLine();
                        TimeSpan a = new TimeSpan();
                        a = TimeSpan.Parse(Data);
                        if (begin == -1)
                        {
                            end = a.TotalSeconds;
                            a -= TimeSpan.FromSeconds(10);
                            begin = a.TotalSeconds;
                        }
                        else if (end + 30 >= a.TotalSeconds)
                        {
                            end = a.TotalSeconds;
                        }
                        else
                        {
                            double interval = (double)pictureBox1.Width / ((double)60 * 60 * 24) * (end - begin);
                            if (interval < 1) interval = 1;
                            double pos = (double)pictureBox1.Width * (begin / ((double)60 * 60 * 24));

                            Brush c = Brushes.Green;
                            if (i == DateTime.Now.Day && end + 300 >= DateTime.Now.Hour * 60 * 60 + DateTime.Now.Minute * 60 + DateTime.Now.Second)
                                c = Brushes.LimeGreen;
                            g.FillRectangle(c, new Rectangle((int)pos, (int)posY, (int)interval, (int)(posY2 - posY+1)));

                            end = a.TotalSeconds;
                            a -= TimeSpan.FromSeconds(10);
                            begin = a.TotalSeconds;
                        }
                    }

                    if (begin != -1)
                    {
                        double interval = (double)pictureBox1.Width / ((double)60 * 60 * 24) * (end - begin);
                        if (interval < 1) interval = 1;
                        double pos = (double)pictureBox1.Width * (begin / ((double)60 * 60 * 24));
                        
                        Brush c = Brushes.Green;
                        if (i == DateTime.Now.Day && end + 300 >= DateTime.Now.Hour * 60 * 60 + DateTime.Now.Minute * 60 + DateTime.Now.Second)
                            c = Brushes.LimeGreen;
                        g.FillRectangle(c, new Rectangle((int)pos, (int)posY, (int)interval, (int)(posY2-posY+1)));
                    }

                    Datei.Close();
                }

                Pen linie = new Pen(Color.Black, 1);
                linie.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                g.DrawLine(linie, (int)0, (int)posY2, (int)width, (int)posY2);
            }

            Pen linie2 = new Pen(Color.Blue,1);
            linie2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            // 6 Uhr
            g.DrawLine(linie2, (int)(pictureBox1.Width / 4), (int)0, (int)(pictureBox1.Width / 4), (int)height);
            // 12 Uhr
            g.DrawLine(linie2, (int)(pictureBox1.Width / 2), (int)0, (int)(pictureBox1.Width / 2), (int)height);
            // 18 Uhr
            g.DrawLine(linie2, (int)(pictureBox1.Width * 0.75), (int)0, (int)(pictureBox1.Width * 0.75), (int)height);
            
            pictureBox1.Image = b1;
            g.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("Daten"))
            {
                TimeSpan Heute = new TimeSpan();
                TimeSpan Monat = new TimeSpan();

                string[] Dateien = Directory.GetFiles("Daten");
                DateTime Time = DateTime.Now;
                String Jahr = Time.Year.ToString();
                String Mon = selectedMonth.ToString().PadLeft(2, '0');
                String Tag = Time.Day.ToString().PadLeft(2, '0');
                String DateinameMonat = "Daten\\" + Jahr + "_" + Mon + "_";

                StreamWriter Datei = new StreamWriter("result_" + Jahr + "_" + Mon + ".dat");

                for (int i = 0; i < Dateien.Length; i++)
                {
                    if (Dateien[i].Substring(0, DateinameMonat.Length) == DateinameMonat )
                    {
                        // heute ermitteln
                        StreamReader inp = new StreamReader(Dateien[i]);
                        String anfang = inp.ReadLine();
                        inp.Close();
                        FileInfo data = new FileInfo(Dateien[i]);
                        Heute = TimeSpan.FromSeconds(data.Length);
                        Heute -= TimeSpan.FromSeconds(Heute.Seconds);
                        TimeSpan anfang2 = TimeSpan.Parse(anfang);
                        if (Heute.Minutes % 15 != 0)
                        {
                            if (Heute.Minutes % 15 >= 5)
                            {
                                Heute += TimeSpan.FromSeconds(60 * (15 - (Heute.Minutes % 15)));
                            }else if (Heute.Minutes % 15 < 5)
                            {
                                Heute -= TimeSpan.FromSeconds(60 * (Heute.Minutes % 15));
                            }
                                
                        }
                        TimeSpan HeuteEnd = anfang2 + Heute;
                        Datei.WriteLine(Dateien[i] + ": " + (anfang2.Hours + anfang2.Days * 24).ToString().PadLeft(2, '0') + ":" + anfang2.Minutes.ToString().PadLeft(2, '0') + " bis " + (HeuteEnd.Hours + HeuteEnd.Days * 24).ToString().PadLeft(2, '0') + ":" + HeuteEnd.Minutes.ToString().PadLeft(2, '0') + "  " + (Heute.Hours + Heute.Days * 24).ToString() + (Heute.Minutes>0 ? "," + ((int) (Heute.Minutes/60.0*100.0)).ToString().TrimEnd('0'):""));
                        Monat += Heute;
                    }
                }
                Datei.WriteLine("Monat: " + (Monat.Hours + Monat.Days * 24).ToString() + (Monat.Minutes > 0 ? "," + ((int)(Monat.Minutes / 60.0 * 100.0)).ToString().TrimEnd('0') : ""));
                Datei.Close();
                System.Diagnostics.Process.Start(@"result_" + Jahr + "_" + Mon + ".dat");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selectedMonth > 1)
            {
                selectedMonth--;
                initialisieren();
                drawStat();
                drawFields();
            }

            if (selectedMonth != DateTime.Now.Month)
            {
                Zeit.Abschalten();
                button1.Text = "Start";
                button1.ForeColor = Color.Black;
                timer2.Enabled = false;
                timer3.Enabled = false;
                button1.Hide();
            }
            else
            {
                button1.Show();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (selectedMonth < 12)
            {
                selectedMonth++;
                initialisieren();
                drawStat();
                drawFields();
            }

            if (selectedMonth != DateTime.Now.Month)
            {
                Zeit.Abschalten();
                button1.Text = "Start";
                button1.ForeColor = Color.Black;
                timer2.Enabled = false;
                timer3.Enabled = false;
                button1.Hide();
            }
            else
            {
                button1.Show();
            }
        }
    }
}