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

namespace CourseWork
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

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

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                isReading = !isReading;
                isConnected = true;
                string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
                serialPort.PortName = selectedPort;
                serialPort.Open();
                button1.Text = "Отсоединиться";
                int y = 50;
                int i = 0;
                string str;
                int res;
                bool write;
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Filter = "Text files(*.txt) | *.txt";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    write = true;
                    using (FileStream theFile = new FileStream(ofd.FileName, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(theFile))

                        {

                            sw.WriteLine("Результаты работы программы:");

                        }

                    }
                }
                else
                    write = false;
                string filename = ofd.FileName;

                while (isReading)
                {
                    str = serialPort.ReadLine();
                    str = str.Replace(".00\r", "");
                    res = Int32.Parse(str);
                    if (write)
                    {
                        using (FileStream theFile = new FileStream(ofd.FileName, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(theFile))

                            {

                                sw.WriteLine(res);

                            }

                        }
                    }
                    chart1.Series[0].Points.AddXY(i++, res);
                    chart1.Update();
                    chart1.ChartAreas[0].AxisX.Maximum = i + 25;
                    chart1.ChartAreas[0].AxisX.Minimum = i - 25;
                }

                
            }
            else
            {
                isConnected = false;
                serialPort.Close();
            }
        }
    }
}
