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
using System.IO;
using System.Collections;

namespace ComLogger
{
    public partial class Form1 : Form
    {
        private SerialPort dataStream;
        private string pathToFolder;
        private bool fileMade = false;
        private string dtS;
        private string path;
        private string fileExtention;
        private bool connected = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //addConsoleText("Set COM port to: " + textBox1.Text);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string dir = fbd.SelectedPath;
                pathToFolder = dir;
                addConsoleText("Set log folder to: " + dir);
                
            }
            setFilePath();

        }

        public void addConsoleText(string msg)
        {
            textBox2.AppendText(msg + "\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (connected == false)
            {
                dataStream = new SerialPort(textBox1.Text, int.Parse(textBox3.Text));
                dataStream.ReadTimeout = 1000;
                dataStream.DtrEnable = true;
                setFilePath();

                try
                {
                    dataStream.Open();
                    dataStream.DataReceived += dataStream_DataReceived;
                    addConsoleText("connection opened");
                    button2.Text = "Stop Log";
                    connected = true;

                }
                catch
                {
                    addConsoleText("connection failed");
                }
            }
            else
            {
                try
                {
                    dataStream.Close();
                    addConsoleText("Connection closed");
                    addConsoleText("Stopped logging");
                    connected = false;
                    button2.Text = "Start Log";

                }
                catch
                {
                    addConsoleText("disconnect failed???");
                }
            }
        }

        private void dataStream_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string line = dataStream.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }

        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {
            //What to do with the received line here
            addConsoleText(line);
            if (checkBox2.Checked == true)
            {
                if (fileMade == false)
                {
                    try
                    {


                        using (StreamWriter sw = File.CreateText(path))
                        {
                            if (checkBox1.Checked == false)
                            {
                                sw.WriteLine(line);
                            }
                            else
                            {
                                sw.WriteLine(DateTime.Now + "," + line);
                            }

                        }

                        fileMade = true;
                    }
                    catch
                    {
                        addConsoleText("Cannot write to file! No folder was selected!");
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        if (checkBox1.Checked == false)
                        {
                            sw.WriteLine(line);
                        }
                        else
                        {
                            sw.WriteLine(DateTime.Now + "," + line);
                        }
                    }
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void setFilePath()
        {
            DateTime formatedDate = DateTime.Now;
            dtS = formatedDate.ToString("dd/MM/yyyy_HH-mm");
            if (radioButton1.Checked == true)
            {
                fileExtention = ".txt";
            }
            else
            {
                fileExtention = ".csv";
            }

            path = @pathToFolder + "\\log" + dtS + fileExtention;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            setFilePath();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            setFilePath();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            string portText = "";
            for(int i =0; i < ports.Length; i++)
            {
                portText = portText + " " + ports[i] + ",";
            }

            addConsoleText("Com ports detected: " + portText);
            textBox1.Text = ports[0];
        }
    }
}
