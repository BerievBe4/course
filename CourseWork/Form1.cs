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
using System.Windows.Forms.DataVisualization.Charting;

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
        static string PathToFile = null;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // При закрытии программы, закрываем порт
            if (serialPort.IsOpen) 
                serialPort.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            var str = serialPort.ReadExisting();
            var n = str.IndexOf("\r");
            if (!string.IsNullOrEmpty(str) && n > 0)
            {
                str = str.Substring(0, n);
                if (!string.IsNullOrEmpty(str))
                {
                    var res = Int32.Parse(str);
                    double floatres = res * 5;
                    floatres /= 1024;
                    floatres *= 1000 / 90 * 7.5;

                    if (floatres >= 80 && floatres <= 120)
                    {
                        using (FileStream theFile = new FileStream(PathToFile, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(theFile))

                            {
                                sw.WriteLine(floatres.ToString());
                            }
                        }
                    }
                }
            }
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
                int i = 0, j = 0;
                long currentTime = 0;
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Filter = "Text files(*.txt) | *.txt";
                Stopwatch stopwatch = new Stopwatch();

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream theFile = new FileStream(ofd.FileName, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(theFile))

                        {
                            sw.WriteLine("Давление Производная");
                        }

                    }
                }
                PathToFile = ofd.FileName;
                chart1.ChartAreas[0].AxisY.Maximum = 120;
                chart1.ChartAreas[0].AxisY.Minimum = 80;
                chart1.ChartAreas[0].AxisX.Maximum = i + 25;
                chart1.ChartAreas[0].AxisX.Minimum = i - 25;
                chart2.ChartAreas[0].AxisY.Maximum = 6;
                chart2.ChartAreas[0].AxisY.Minimum = -6;
                chart2.ChartAreas[0].AxisX.Maximum = j + 25;
                chart2.ChartAreas[0].AxisX.Minimum = j - 25;
                chart2.SuppressExceptions = true;


                if (!String.IsNullOrEmpty(textBox1.Text))
                {
                    maxTime = Int32.Parse(textBox1.Text);
                }
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                while (isReading && (currentTime < maxTime))
                {                  
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
