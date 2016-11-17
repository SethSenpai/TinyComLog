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

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //handle folder selection and output it to the in program console
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string dir = fbd.SelectedPath;
                pathToFolder = dir;
                addConsoleText("Set log folder to: " + dir);
                
            }
            //set the filepath used to reflect the changes made.
            //this function will be called multiple times since it allows on the fly changes to the logging that is being done without interupting measurements
            setFilePath();

        }

        //function for easy access to the in program console
        public void addConsoleText(string msg)
        {
            textBox2.AppendText(msg + "\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //check if we are already connected to a device if not open a new connection
            if (connected == false)
            {
                dataStream = new SerialPort(textBox1.Text, int.Parse(textBox3.Text));
                dataStream.ReadTimeout = 1000;
                dataStream.DtrEnable = true;
                setFilePath();

                //try to open a new connection if not possible print an error
                try
                {
                    dataStream.Open();
                    //if we could make a connection add an event to datastream
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

            //if we are connected already we are going to close the connection and stop logging
            else
            {
                //try to close the connection and reset booleans
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

        //if we get any data coming in invoke the lineRecieved function
        private void dataStream_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string line = dataStream.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }

        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {
            //display the value in the console and check if we should log it
            addConsoleText(line);
            if (checkBox2.Checked == true)
            {
                //check if we have made a new file otherwise write to the file
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

        //function that sets the path variable to reflect the current settings
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

        //detect com ports and print them in the console
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

        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }
}
