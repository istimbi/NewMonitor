using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace NewMonitor
{
    
    public partial class Form1 : Form
    {
        Boolean scroll_move = false;
        int difference = 0;
        int index = 0;
        int clickNumber = 0;
        int timerCounter = 0;
        int timerCounternew = 0;
        public int[] collection = new int [1];
        public int[] collection1 = new int[1];
        public int[] collection2 = new int[50];
        int min_temp = 0;
        public string[] collection_string = new string [1];
        public int[] collection_d = new int[1];
        public int[] temporarry = new int[800];
        public int[] firstarray = new int[1];
        public int[] secondarray = new int[1];
        public double[] stepx = new double[1];
        int length = 0;
        String all;
        String[] values = new string[3];
        int maxindex = 0;
        int fifteens = 0;
        int startdraw = 0;
        int stopdraw = 0;
        int maxval = 0;
        int maxvallast = 0;
        double step = 0;
        double bottomline = 0.56;
        double topline = 0.7;
        Boolean movesecond = false;
        Boolean movefirst = false;
        HitTestResult seriesHit;
        DateTime end = DateTime.Now.AddMinutes(0);//таймер на фазу ишемии
        public int[]  io = new int[5];
        int iosr = 0;
        public double[]  ij = new double[5];
        double ijsr = 0;
        public int[]  io1 = new int[5];
        int io1sr = 0;
        public double[]  ij1 = new double[5];
        double ij1sr = 0;


        MainForm frm = new MainForm();


        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit; //Подписываемся на событие
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            frm.Owner = this;           //Открываем вторую форму
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                try
                {
                    button_press(); // если нажата кнопка Давление то открываем метод для обработки давления      
                }
                catch (Exception e)
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream("error.txt", FileMode.OpenOrCreate, FileAccess.Write)))
                    {
                        sw.WriteLine(DateTime.Now + ": ToDo executing -  " + e.Message);
                    }
                }

            }
            if (frm.DialogResult == DialogResult.Cancel) Process.GetCurrentProcess().Kill();        //Если нажата кнопка выхода из формы2, то вырубаем всё         
           
        }

        public void button_press()
        {
            this.Show();
            bottomline = Properties.Settings.Default.bottom_line;
            topline = Properties.Settings.Default.top_line;
            
            if (Device.chek)
            {
                try
                {

                    tabControl1.SelectTab(1);
                    Device.Port.Write("3");
                    backgroundWorker1.RunWorkerAsyn​c();
                }
                catch (Exception e)
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream("errorDavlenie.txt", FileMode.OpenOrCreate, FileAccess.Write)))
                    {
                        sw.WriteLine(DateTime.Now + ": ToDo executing -  " + e.Message);
                    }
                }


                //tabControl1.SelectTab(3);
                //end = DateTime.Now.AddMinutes(Device.time);
                //Device.Port.Write("3");
                //backgroundWorker4.RunWorkerAsyn​c();
            }
            else
            {
                tabControl1.SelectTab(2);
                end = DateTime.Now.AddMinutes(0);
                label4.Text = TimeLeft().Minutes.ToString() + ":" + TimeLeft().Seconds.ToString();
                timer2.Enabled = true;
            }          
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //На давление 1
        {
            while (Convert.ToInt32(Device.values[0]) < 2200 && timerCounternew<100)
            {
                Device.Read();
                if (Convert.ToInt32(Device.values[0]) > 500)
                {                    
              
                    Device.Read();
                    values = Device.values;
                    this.Invoke((MethodInvoker)delegate
                    {
                        chart1.Series[0].Points.AddXY(timerCounter + 1, Convert.ToInt32(values[1]));
                    });
                    collection[timerCounter] = Convert.ToInt32(values[1]);
                    temporarry[timerCounternew] = Convert.ToInt32(values[1]);
                    Array.Resize(ref collection, collection.Length + 1);
                    maxval = temporarry.Max();
                    timerCounter++;
                    timerCounternew++;
                    if (timerCounternew == 100 && maxval > maxvallast)
                    {
                        maxvallast = maxval;
                        timerCounternew = 0;
                        Array.Clear(temporarry, 0, temporarry.Length);
                    }
                }
            }
           
           if (maxval < maxvallast)
            {
                difference = Convert.ToInt32(Device.values[0]);
                Device.Port.Write("1"); //Останавливаем мотор и поддерживаем давление на одном уровне
            }
        }


        private void backgroundWorker2_DoWork_1(object sender, DoWorkEventArgs e)
        {
            Array.Resize(ref collection, 1);
            Array.Resize(ref collection_string, 1);
            if (Device.Port.IsOpen == false)
            {
                Device.Open(frm.Data);
            }

            if (Device.Port.IsOpen == true)
            {
                all = Device.Port.ReadLine();
                Device.Port.DiscardInBuffer();
                all = all.Trim('r', ' ');
                values = all.Split('\t');
                timerCounter = 0;
                
                    while (values.Length < 4)
                    {
                        all = Device.Port.ReadLine();
                        all = all.Trim('r', ' ');
                        values = all.Split('\t');
                        timerCounter = 0;
                    }

                if (values.Length > 3)
                {
                    do
                    {
                        all = Device.Port.ReadLine();
                        all = all.Trim('\r', ' ');
                        values = all.Split('\t');
                        if (values.Length > 3 && values[3] != "")
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                if (timerCounter > 180 && timerCounter < 1250)
                                {
                                    chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = Convert.ToDouble(timerCounter + 1) / 100 - 1.79;

                                }
                                if (timerCounter > 1250)
                                {
                                    chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = Convert.ToDouble(timerCounter + 1) / 100 - 1.99;
                                }
                            });

                            this.Invoke((MethodInvoker)delegate
                            {
                                chart2.Series[0].Points.AddXY(Convert.ToDouble(timerCounter+1) / 100,Convert.ToInt32(values[3]));
                                //Thread.Sleep(100);
                            });

                            collection_string[timerCounter] = (collection[timerCounter] = Convert.ToInt32(values[3])).ToString();


                            if (double.IsNaN(chart2.ChartAreas[0].AxisY.Maximum))
                            {

                                this.Invoke((MethodInvoker)delegate
                                {
                                    chart2.ChartAreas[0].AxisY.Maximum = collection.Max() + 20;
                                    chart2.ChartAreas[0].AxisY.Minimum = collection.Min() - 20;
                                });

                            }
                            else
                            {

                                if (collection.Max() + 20 > Convert.ToInt32(chart2.ChartAreas[0].AxisY.Maximum)) //Меняем масштаб
                                {
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        chart2.ChartAreas[0].AxisY.Maximum = collection.Max() + 20;
                                    });
                                }

                                if (Convert.ToInt32(chart2.ChartAreas[0].AxisY.Minimum) - 20 > collection.Min()) //Меняем масштаб
                                {
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        chart2.ChartAreas[0].AxisY.Minimum = collection.Min() - 20;
                                    });
                                }

                            }

                            Array.Resize(ref collection, collection.Length + 1);
                            Array.Resize(ref collection_string, collection_string.Length + 1);
                            timerCounter++;
                          
                          
                        }
                        else
                        {
                            while (values.Length < 4)
                            {
                                all =Device.Port.ReadLine();
                                all = all.Trim('\r', ' ');
                                values = all.Split('\t');
                            }
                        }          
                    }
                    while (Convert.ToInt32(timerCounter) < 1301);
                    collection[collection.Length - 1] = collection[collection.Length - 2];

                    //this.Invoke((MethodInvoker)delegate {
                    //    chart2.Series[0].Points.Clear();
                    //});
                    //int min = collection.Min();
                    //for (int i = 0; i < collection.Length - 3; i++)
                    //{

                    //   collection[i] = (collection[i] + collection[i + 1] + collection[i + 2]) / 3 - min;
                    //   collection_string[i] = (collection[i]).ToString();
                    //   this.Invoke((MethodInvoker)delegate {

                    //        chart2.Series[0].Points.AddXY(i+1, collection[i]);
                    //    });

                    //}
                    //collection[collection.Length-1] = collection[collection.Length - 4];
                    //collection[collection.Length - 2] = collection[collection.Length - 4];
                    //collection[collection.Length - 3] = collection[collection.Length - 4];
                    //this.Invoke((MethodInvoker)delegate
                    //{
                    //    chart2.ChartAreas[0].AxisY.Maximum = collection.Max() + 20;
                    //    chart2.ChartAreas[0].AxisY.Minimum = collection.Min() - 20;
                    //});
                    this.Invoke((MethodInvoker)delegate
                    {
                        int max = collection.Max();
                        int maxvalue = Array.IndexOf(collection, max);
                        if (maxvalue < collection.Length/2 && maxvalue > 5 )
                        {
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue + 1)/100, max + 1);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue + 1)/100, max + 3);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue + 1)/100, max + 5);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue + 1)/100, max + 7);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue + 1)/100, max + 9);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue + 10)/100, max + 1);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue + 10)/100, max + 3);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue + 10)/100, max + 5);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue + 10)/100, max + 7);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue + 10)/100, max + 9);
                        }
                        else
                        {
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue - 10)/100, max + 1);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue - 10)/100, max + 3);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue - 10)/100, max + 5);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue - 10)/100, max + 7);
                            chart2.Series[1].Points.AddXY(Convert.ToDouble(maxvalue - 10)/100, max + 9);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue - 1)/100, max + 1);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue - 1)/100, max + 3);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue - 1)/100, max + 5);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue - 1)/100, max + 7);
                            chart2.Series[2].Points.AddXY(Convert.ToDouble(maxvalue - 1)/100, max + 9);
                        }
                        
                    });
                    button2.BackColor = Color.LightGreen;
                }
            }
        } //На плетизмограмму                


      

        private void backgroundWorker4_DoWork( object sender, DoWorkEventArgs e)
        {
            Array.Resize(ref collection, 1);
            Array.Resize(ref collection_string, 1);
            if (Device.Port.IsOpen == false)
            {
                Device.Open(frm.Data);
            }

            if (Device.Port.IsOpen == true)
            {
                all = Device.Port.ReadLine();
                Device.Port.DiscardInBuffer();
                all = all.Trim('r', ' ');
                values = all.Split('\t');
                timerCounter = 0;

                while (values.Length < 4)
                {
                    all = Device.Port.ReadLine();
                    all = all.Trim('r', ' ');
                    values = all.Split('\t');
                    timerCounter = 0;
                }

                if (values.Length > 3)
                {
                    do
                    {
                        all = Device.Port.ReadLine();
                        all = all.Trim('\r', ' ');
                        values = all.Split('\t');
                        if (values.Length > 3 && values[3] != "")
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                if (timerCounter > 178)
                                {
                                    chart3.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = Convert.ToDouble(timerCounter + 1) / 100 - 1.49;

                                }
                            });

                            this.Invoke((MethodInvoker)delegate
                            {
                                chart3.Series[0].Points.AddXY(Convert.ToDouble(timerCounter + 1) * 0.02, Convert.ToInt32(values[0]));
                                chart3.Series[1].Points.AddXY(Convert.ToDouble(timerCounter + 1) * 0.02, Convert.ToInt32(values[3]));
                                //Thread.Sleep(100);
                            });
                            collection_string[timerCounter] = (collection[timerCounter] = Convert.ToInt32(values[3])).ToString();
                            collection_d[timerCounter] = Convert.ToInt32(values[0]);

                            Array.Resize(ref collection, collection.Length + 1);
                            Array.Resize(ref collection_string, collection_string.Length + 1);
                            Array.Resize(ref collection_d, collection_d.Length + 1);
                            timerCounter++;
                        }
                        else
                        {
                            while (values.Length < 4)
                            {
                                all = Device.Port.ReadLine();
                                all = all.Trim('\r', ' ');
                                values = all.Split('\t');
                            }
                        }
                    }
                    while (TimeLeft() > TimeSpan.Zero);
                    collection[collection.Length - 1] = collection[collection.Length - 2];      
                }
            }
        } //На плетизмограмму         +      давление       




        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Device.Port.IsOpen == false)
            {
                Device.Open(frm.Data);                
            }
            if (Device.Port.IsOpen == true)
            {               
                all = Device.Port.ReadLine();
                all = all.Trim('r', ' ');
                values = all.Split('\t');
                timerCounter = 0;
              
                    while (values.Length < 4)
                    {
                        all = Device.Port.ReadLine();
                        all = all.Trim('r', ' ');
                        values = all.Split('\t');
                        timerCounter = 0;
                    }
                
                if (values.Length > 3 && values[0] != null && Convert.ToInt32(values[0]) < 600)
                {
                    do
                    {
                        all = Device.Port.ReadLine();
                        all = all.Trim('\r', ' ');
                        values = all.Split('\t');
                        while (values.Length < 4)
                        {
                            all = Device.Port.ReadLine();
                            all = all.Trim('r', ' ');
                            values = all.Split('\t');
                        }
                        length++;
                    }
                    while (Convert.ToInt32(values[0]) < 600);
                }
                if (values.Length > 3 && values[1] != null && Convert.ToInt32(values[0]) > 600)
                {
                    timer1.Stop();
                    backgroundWorker1.RunWorkerAsyn​c();
                }
                }
        }

        private void button1_Click(object sender, EventArgs e) // Кнопка Анализировать на вкладке Давления
        {

            int maxval = collection.Max();

            maxindex = Array.IndexOf(collection, maxval);
            //Array.Resize(ref firstarray, maxindex);
            //firstarray = collection.Take(maxindex).ToArray();
            //Array.Resize(ref secondarray, collection.Length - maxindex);
            //secondarray = collection.Skip(maxindex).ToArray();
            int seventeens = Convert.ToInt32((collection.Max() - 2000) * bottomline + 2000);
            fifteens = Convert.ToInt32((collection.Max() - 2000) * topline + 2000);
            //Array.Resize(ref stepx, collection.Length);
            step = Math.Round( (double) (difference-600)/10 / collection.Length, 1);

            for (int i = collection.Length - 2; i > maxindex; i--)
            {
                if (collection[i] > fifteens)
                {
                    stopdraw = i;
                    break;
                }
            }
            for (int i = 0; i < maxindex; i++)
            {
                if (collection[i] > seventeens)
                {
                    startdraw = i;
                    break;
                }
            }

            if (startdraw - 10 < 10 || stopdraw + 10 > collection.Length)
            {
                startdraw = startdraw + 11;
                stopdraw = stopdraw - 11;
            }

            for (int i = startdraw - 10; i < stopdraw + 10; i++)
            {
                chart1.Series[1].Points.AddXY(i*step+40, collection[i]);
            }
            for (int i = 0; i < collection.Length-1; i++)
            {
                chart1.Series[0].Points[i].XValue = i*step+40;
            }
            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = Math.Round(chart1.Series[0].Points[collection.Length - 2].XValue);



           

          
            

            tabControl1.SelectTab(2);
            end = DateTime.Now.AddMinutes(1);
            label4.Text = TimeLeft().Minutes.ToString() + ":" + TimeLeft().Seconds.ToString();
            timer2.Start();            
        }

        private TimeSpan TimeLeft()
        {
            return end - DateTime.Now;
        }
                
        private void timer2_Tick(object sender, EventArgs e)
        {
            button4.Visible = false;
            label4.Text = TimeLeft().Minutes.ToString() + ":" + TimeLeft().Seconds.ToString();

            if (TimeLeft() < TimeSpan.Zero)
            {
                timer2.Stop();
                Device.Port.Write("0");
                MessageBox.Show("Фаза ишемии завершилась");
                backgroundWorker2.RunWorkerAsyn​c();
            }
        }


        public int  getindex(int value)
        {
            int index = Array.IndexOf(collection, value);
            return index;
        }

        public void printfirst(int value)
        {
            for (int i = value; i < maxindex; i++)
            {
                chart1.Series[1].Points.AddXY(i, collection[i]);
            }
        }

        public void printlast(int value)
        {
            for (int i = value; i > maxindex; i--)
            {
                chart1.Series[1].Points.AddXY(i, collection[i]);
            }
        }

        public void changesize(int value)
        {
            for (int i = value - 5;  i < value + 5; i++)
            {
                chart1.Series[1].Points.AddXY(i, firstarray[i]);
            }
            firstarray = collection.Take(value).ToArray();
            Array.Resize(ref firstarray, value);          
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();
                chart2.Series[0].Points.Clear();
                chart2.Series[1].Points.Clear();
                chart2.Series[2].Points.Clear();
                this.Hide();
                frm.Owner = this;           //Открываем вторую форму
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK) button_press(); // если нажата кнопка Давление то открываем метод для обработки давления            
                if (frm.DialogResult == DialogResult.Cancel) Process.GetCurrentProcess().Kill();        //Если нажата кнопка выхода из формы2, то вырубаем всё         
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (io[0] != 0 && label11.Text != "")
            {
                collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[0].XValue)).ToArray();
                if (collection1.Length - 50 > 0)
                {
                    collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                }
                else
                {
                    collection2 = collection1.Skip(0).ToArray();
                }
                double dno = collection2.Min();

                io1[0] = Convert.ToInt32((chart2.Series[2].Points[0].YValues[0] - dno) / (chart2.Series[1].Points[0].YValues[0] - dno) * 100);
                    collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[1].XValue)).ToArray();
                    collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
                io1[1] = Convert.ToInt32((chart2.Series[2].Points[1].YValues[0] - dno) / (chart2.Series[1].Points[1].YValues[0] - dno) * 100);
                    collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[2].XValue)).ToArray();
                    collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
                io1[2] = Convert.ToInt32((chart2.Series[2].Points[2].YValues[0] - dno) / (chart2.Series[1].Points[2].YValues[0] - dno) * 100);
                    collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[3].XValue)).ToArray();
                    collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
                io1[3] = Convert.ToInt32((chart2.Series[2].Points[3].YValues[0] - dno) / (chart2.Series[1].Points[3].YValues[0] - dno) * 100);
                    collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[4].XValue)).ToArray();
                    collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
                io1[4] = Convert.ToInt32((chart2.Series[2].Points[4].YValues[0] - dno) / (chart2.Series[1].Points[4].YValues[0] - dno) * 100);
                ij1[0] = Device.L / ((chart2.Series[2].Points[0].XValue - chart2.Series[1].Points[0].XValue)*20)*10;
                ij1[1] = Device.L / ((chart2.Series[2].Points[1].XValue - chart2.Series[1].Points[1].XValue)*20)*10;
                ij1[2] = Device.L / ((chart2.Series[2].Points[2].XValue - chart2.Series[1].Points[2].XValue)*20)*10;
                ij1[3] = Device.L / ((chart2.Series[2].Points[3].XValue - chart2.Series[1].Points[3].XValue)*20)*10;
                ij1[4] = Device.L / ((chart2.Series[2].Points[4].XValue - chart2.Series[1].Points[4].XValue)*20)*10;
                io1sr = Convert.ToInt32(io1.Average());
                label13.Text = io1sr.ToString() + "%";
                ij1sr = Convert.ToInt32(ij1.Average());
                ij1sr = Math.Round(ij1sr, 2);
                label12.Text = ij1sr.ToString();
                double test1 = iosr - io1sr;
                test1 = test1 / iosr * 100;
                label15.Text = Convert.ToString(Math.Round(test1,2));
                button4.Visible = false;
            }

            else
            {
                 collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[0].XValue)).ToArray();

                if (collection1.Length - 50 > 0 )
                {
                    collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                }

                else
                {
                    collection2 = collection1.Skip(0).ToArray();
                }

                 double dno = collection2.Min();
            io[0] = Convert.ToInt32((chart2.Series[2].Points[0].YValues[0] - dno) / (chart2.Series[1].Points[0].YValues[0] - dno) * 100);
                collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[1].XValue)).ToArray();
                collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
            io[1] = Convert.ToInt32((chart2.Series[2].Points[1].YValues[0] - dno) /( chart2.Series[1].Points[1].YValues[0] - dno) * 100);
                collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[2].XValue)).ToArray();
                collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
            io[2] = Convert.ToInt32((chart2.Series[2].Points[2].YValues[0] - dno) /( chart2.Series[1].Points[2].YValues[0] - dno) * 100);
                collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[3].XValue)).ToArray();
                collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
            io[3] = Convert.ToInt32((chart2.Series[2].Points[3].YValues[0] - dno) / (chart2.Series[1].Points[3].YValues[0] - dno) * 100);
                collection1 = collection.Take(Convert.ToInt32(chart2.Series[1].Points[4].XValue)).ToArray();
                collection2 = collection1.Skip(collection1.Length - 20).ToArray();
                dno = collection2.Min();
            io[4] = Convert.ToInt32((chart2.Series[2].Points[4].YValues[0] - dno) /( chart2.Series[1].Points[4].YValues[0] - dno)*100);

            ij[0] = Device.L / ((chart2.Series[2].Points[0].XValue - chart2.Series[1].Points[0].XValue) * 20)*10;
            ij[1] = Device.L / ((chart2.Series[2].Points[1].XValue - chart2.Series[1].Points[1].XValue) * 20)*10;
            ij[2] = Device.L / ((chart2.Series[2].Points[2].XValue - chart2.Series[1].Points[2].XValue) * 20)*10;
            ij[3] = Device.L / ((chart2.Series[2].Points[3].XValue - chart2.Series[1].Points[3].XValue) * 20)*10;
            ij[4] = Device.L / ((chart2.Series[2].Points[4].XValue - chart2.Series[1].Points[4].XValue) * 20)*10;
            iosr = Convert.ToInt32(io.Average());
            label11.Text = iosr.ToString() + "%";
            ijsr = Convert.ToDouble(ij.Average());
                ijsr = Math.Round(ijsr, 2);
            label7.Text = ijsr.ToString();
            button4.Visible = true;

            }
        }



        //Обработка нажатий на chart2
        public void myChart_MouseDown(object sender, MouseEventArgs e)
        {
            
            movefirst = false;
            HitTestResult seriesHit = chart2.HitTest(e.X, e.Y);            
            if (seriesHit.Series != null && seriesHit.Series.Name == "Точки А1")
            {
                var prop = seriesHit.Object as DataPoint;
                index = seriesHit.PointIndex;               
                movefirst = true;
            }
            if (seriesHit.Series != null && seriesHit.Series.Name == "Точки А2")
            {
                var prop = seriesHit.Object as DataPoint;
                index = seriesHit.PointIndex;                
                movesecond = true;
            }

        }

        public void chart3_AxisViewChanged(object sender, ViewEventArgs e)
        {
            //chart3.ChartAreas[0].AxisX.RoundAxisValue(1);
            chart3.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
            chart3.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
            chart3.ChartAreas[0].AxisX.LabelStyle.Format = "0.###";        
        }


        public void myChart_MouseWheel(object sender, MouseEventArgs e)
        {

            if (e.Delta > 0 && chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position < 11)
            {
                chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = Math.Round(chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position + 0.1, 1);
            }

            if (e.Delta < 0 && chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position > 0.01)
            {
                chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = Math.Round(chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position - 0.1, 1);
            }
        }

        public void myChart3_MouseWheel(object sender, MouseEventArgs e)
        {

            if (e.Delta > 0 && chart3.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position < 11)
            {
                chart3.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position + 0.1;
            }

            if (e.Delta < 0 && chart3.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position > 0.01)
            {
                chart3.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position - 0.1;
            }
        }

        public void myChart_Doubleclick(object sender, MouseEventArgs e)
        {
            if (chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled == true)
            {
                chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
                chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
            }
            else
            {
                chart2.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart2.ChartAreas[0].CursorY.IsUserEnabled = true;
                chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            }          
           
        }
        private void scroll_press(object sender, ScrollBarEventArgs e)
        {
            scroll_move = true;
        }


        private void chart2_AxisViewChanged(object sender, ViewEventArgs e)
        {
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
            chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "0.###";
        }

        public void myChart_MouseMove(object sender, MouseEventArgs e)
        {
            movesecond = true;
            movefirst = true;
        }

        public void myChart_MouseUp(object sender, MouseEventArgs e)
        {
            if (scroll_move)
            {
                if (chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position < 0.2)
                {
                    chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = 0;
                }
                else
                {
                    chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = Math.Round(chart2.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position, 2);
                }
            }

            HitTestResult seriesHit = chart2.HitTest(e.X, e.Y);

            if (movefirst == true && seriesHit.Series != null && seriesHit.Series.Name == "Ритм")
            {
                var prop = seriesHit.Object as DataPoint;
                chart2.Series[1].Points.RemoveAt(index);
                chart2.Series[1].Points.AddXY(prop.XValue, prop.YValues[0]);
                movefirst = false;

            }
            if (movesecond == true && seriesHit.Series != null && seriesHit.Series.Name == "Ритм")
            {
                var prop = seriesHit.Object as DataPoint;
                chart2.Series[2].Points.RemoveAt(index);
                chart2.Series[2].Points.AddXY(prop.XValue, prop.YValues[0]);
                movesecond = false;

            }
            
        }
        

       
        private void OnApplicationExit(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker2.CancelAsync();
            Device.Port.Write("0");
            Device.Port.Write("0");
            Application.Exit();
        }
        public void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker2.CancelAsync();
            Device.Port.Write("0");
            Device.Port.Write("0");
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            collection_string[0] = ("ИОисхСР = " +iosr + " ИЖисхСР = " + ijsr + " | " + " ИО3минСР = " + io1sr + " ИЖ3минСР = " + ij1sr + " | ПФЭ = " + label15.Text).ToString();
            
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog1.FileName , collection_string, Encoding.UTF8);
            }
        }

        private void button4_Click(object sender, EventArgs e) //Кнопка продолжить, после первого замера плетизмограммы.
        {
            tabControl1.SelectTab(1);
            Device.Port.Write("3"); //Отправляем команду старта
            Thread.Sleep(500);
            timer1.Enabled = true;
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            chart2.Series[2].Points.Clear();
            Array.Resize(ref collection, 1);
            Array.Resize(ref collection_string, 1);

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}