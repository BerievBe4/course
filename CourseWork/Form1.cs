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

namespace CourseWork
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool isConnected = false;
        static SerialPort serialPort = new SerialPort();

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
                isConnected = true;
                string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
                serialPort.PortName = selectedPort;
                serialPort.ReadTimeout = 1000;
                serialPort.Open();
                button1.Text = "Отсоединиться";


                try
                {
                    textBox1.Text = serialPort.ReadLine();
                }
                catch
                {

                }
                
            }
            else
            {
                isConnected = false;
                serialPort.Close();
                textBox1.Text = "";
            }
        }
    }
}
