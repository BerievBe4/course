using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace CourseWork
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            // Получаем список COM портов доступных в системе
            string[] portnames = SerialPort.GetPortNames();
            // Проверяем есть ли доступные
            if (portnames.Length == 0)
            {
                MessageBox.Show("COM PORT not found");
            }
            foreach (string portName in portnames)
            {
                //добавляем доступные COM порты в список           
                comboBox1.Items.Add(portName);
                Console.WriteLine(portnames.Length);
                if (portnames[0] != null)
                {
                    comboBox1.SelectedItem = portnames[0];
                }
            }

        }

        private void graphItem_Click(object sender, EventArgs e)
        {
            
        }

        bool isConnected = false;
        static SerialPort serialPort = new SerialPort();
        bool isReading = false;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // При закрытии программы, закрываем порт
            if (serialPort.IsOpen) 
                serialPort.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                isReading = !isReading;
                isConnected = true;
                long maxTime = Int32.MaxValue;
                string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
                serialPort.PortName = selectedPort;
                serialPort.Open();
                button1.Text = "Отсоединиться";
                bool first = true;
                int i = 0;
                int j = 0;
                string str;
                int res;
                long currentTime = 0;
                bool write;
                double prevRes = 0;
                double floatres;
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Filter = "Text files(*.txt) | *.txt";
                Stopwatch stopwatch = new Stopwatch();

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    write = true;
                    using (FileStream theFile = new FileStream(ofd.FileName, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(theFile))

                        {

                            sw.WriteLine("Давление Производная");

                        }

                    }
                }
                else
                    write = false;
                string filename = ofd.FileName;
                chart1.ChartAreas[0].AxisY.Maximum = 120;
                chart1.ChartAreas[0].AxisY.Minimum = 80;
                chart2.ChartAreas[0].AxisY.Maximum = 6;
                chart2.ChartAreas[0].AxisY.Minimum = -6;

                if (!String.IsNullOrEmpty(textBox1.Text))
                {
                    maxTime = Int32.Parse(textBox1.Text);
                }

                while (isReading && (currentTime < maxTime))
                {
                    if (first)
                    {
                        str = serialPort.ReadExisting();
                        stopwatch.Start();
                        var n = str.IndexOf("\r");
                        if (!string.IsNullOrEmpty(str) && n > 0)
                        {
                            str = str.Substring(0, n);
                            if (!string.IsNullOrEmpty(str))
                            {
                                res = Int32.Parse(str);
                                floatres = res * 5;
                                floatres /= 1024;
                                floatres *= 1000 / 90 * 7.5;
                                prevRes = floatres;
                                chart1.Series[0].Points.AddXY(i++, floatres);
                                chart1.ChartAreas[0].AxisX.Maximum = i + 25;
                                chart1.ChartAreas[0].AxisX.Minimum = i - 25;
                                chart1.Update();
                            }
                        }
                        first = false;                      
                    }
                    else
                    {
                        
                        var time = stopwatch.ElapsedMilliseconds;
                        if (serialPort.BytesToRead > 1)
                        {
                            str = serialPort.ReadExisting();
                            var n = str.IndexOf("\r");
                            if (!string.IsNullOrEmpty(str) && n > 0)
                            {
                                str = str.Substring(0, n);
                                if (!string.IsNullOrEmpty(str))
                                {
                                    stopwatch.Stop();
                                    if (maxTime != Int32.MaxValue)
                                    {
                                        currentTime += stopwatch.ElapsedMilliseconds / 1000;
                                    }
                                    res = Int32.Parse(str);
                                    floatres = res * 5;
                                    floatres /= 1024;
                                    floatres *= 1000 / 90 * 7.5;

                                    if (floatres >= 80 && floatres <= 120)
                                    {


                                        chart1.Series[0].Points.AddXY(i++, floatres);
                                        chart1.ChartAreas[0].AxisX.Maximum = i + 25;
                                        chart1.ChartAreas[0].AxisX.Minimum = i - 25;
                                        chart1.Update();




                                        chart2.Series[0].Points.AddXY(j++, (floatres - prevRes) / (stopwatch.ElapsedMilliseconds / 1000.00));
                                        chart2.ChartAreas[0].AxisX.Maximum = j + 25;
                                        chart2.ChartAreas[0].AxisX.Minimum = j - 25;
                                        chart2.Update();

                                        if (write)
                                        {
                                            using (FileStream theFile = new FileStream(ofd.FileName, FileMode.Append, FileAccess.Write))
                                            {
                                                using (StreamWriter sw = new StreamWriter(theFile))

                                                {
                                                    sw.WriteLine();
                                                    sw.Write(floatres + " ");
                                                    sw.Write((floatres - prevRes) / stopwatch.ElapsedMilliseconds / 1000.00 + " ");

                                                }

                                            }
                                        }

                                        prevRes = floatres;
                                        stopwatch.Start();
                                    }
                                }
                            }
                        }
                            
                        first = false;
                    }
                }

                
            }
            else
            {
                isConnected = false;
                serialPort.Close();
            }
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
