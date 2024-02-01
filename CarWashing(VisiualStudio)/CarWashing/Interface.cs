using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CarWashing
{
    public partial class Interface : Form
    {
        private UdpClient udpServer;
        private IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
        private string serverIp = "192.168.1.2";
        static private int serverPort = 5555;
        private bool isBlinking = false;
        private bool carInsýde = false;
        private bool carCheck = false;
        private bool carCheck2 = false;
        private bool carCheck3 = false;
        public Interface()
        {
            InitializeComponent();
            InitializeUdpListener();
        }

        private void InitializeUdpListener()
        {
            // UdpClient'ý baþlat
            int serverPort = 5555;
            udpServer = new UdpClient(serverPort);

            // Arka planda dinleme iþlemini baþlat
            Thread udpListenerThread = new Thread(new ThreadStart(UdpListenerThread));
            udpListenerThread.Start();
        }

        private void UdpListenerThread()
        {
            try
            {
                while (true)
                {
                    byte[] data = udpServer.Receive(ref clientEndpoint);
                    string message = Encoding.UTF8.GetString(data);

                    // Gelen mesajý iþle

                    if (message == "YesCar")
                    {
                        Invoke(new Action(() =>
                        {
                            if (carInsýde == false && carCheck == false)
                            {
                                label2.ForeColor = System.Drawing.Color.Black;
                                label3.ForeColor = System.Drawing.Color.Red;
                                if (isBlinking==false)
                                {
                                    BlinkControl(pictureBox2);
                                    progressBar1.Value = 20;
                                    label8.Text = "Processing...%20";
                                }
                                if (checkedListBox1.Items.Count > 0)
                                {
                                    checkedListBox1.SetItemChecked(0, true);
                                    checkedListBox1.SetItemChecked(1, true);
                                }
                            }
                            else if(carInsýde == true && (carCheck2 == true || carCheck3 == true))
                            {
                                label4.ForeColor = System.Drawing.Color.Purple;
                                pictureBox5.ForeColor = System.Drawing.Color.Purple;
                            }                        
                        }));
                    }
                    else if (message == "NoCar")
                    {
                        Invoke(new Action(() =>
                        {
                            label2.ForeColor = System.Drawing.Color.Red;
                            label3.ForeColor = System.Drawing.Color.Black;
                        }));
                    }
                    else if(message == "PassedCar")
                    {
                        Invoke(new Action(() =>
                        {
                            carInsýde = true;
                            carCheck = true;
                            label3.ForeColor= System.Drawing.Color.Green;
                            if (checkedListBox2.Items.Count > 0)
                            {
                                checkedListBox2.SetItemChecked(0, true);
                            }
                            if (checkedListBox1.Items.Count > 0)
                            {
                                checkedListBox1.SetItemChecked(2, true);
                            }
                            StopBlinking(pictureBox2);
                        }));
                    }
                    else if(message == "YesWash")
                    {
                        Invoke(new Action(() =>
                        {
                            if (carInsýde == true)
                            {
                                if (isBlinking == false && carCheck2 == false)
                                {
                                    progressBar1.Value = 50;
                                    label8.Text = "Processing...%50";
                                    carCheck2 = true;
                                    label5.ForeColor = System.Drawing.Color.Red;
                                    BlinkControl(pictureBox3);
                                }
                            }
                            else if(carInsýde == false)
                            {
                                MessageBox.Show("Please, Enter from entry!");
                            }
                        }));
                    } 
                    else if(message == "YesCooling")
                    {
                        carCheck2 = false;
                        Invoke(new Action(() =>
                        {
                            
                            if(carInsýde == true)
                            {
                                label6.ForeColor = System.Drawing.Color.Red;
                                if (isBlinking == false && carCheck3 == false)
                                {
                                    BlinkControl(pictureBox4);
                                    progressBar1.Value = 80;
                                    label8.Text = "Processing...%80";
                                    carCheck3 = true;
                                }
                            }
                            else if(carInsýde == false)
                            {
                                MessageBox.Show("Please, Enter from entry!");
                            }
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UDP dinleme hatasý: " + ex.Message);
            }
        }

        private void Interface_FormClosed(object sender, FormClosedEventArgs e)
        {
            udpServer.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (carCheck3 == true)
            {
                progressBar1.Value = 90;
                label8.Text = "Processing...%90";
            }
            if (comboBox3.SelectedItem != null)
            {
                string selectedOption = comboBox3.SelectedItem.ToString();
                if (selectedOption != "Select Option")
                {
                    //UDP ile gönder
                    SendSelectedOption(selectedOption);
                }
                else
                {
                    MessageBox.Show("Lütfen bir seçenek seçin.");
                }

            }
            
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() == "Select Option")
            {
                MessageBox.Show("Select option.");
            }
        }

        private void Interface_Load(object sender, EventArgs e)
        {

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Select Option");
            comboBox1.Items.Add("Slow");
            comboBox1.Items.Add("Normal");
            comboBox1.Items.Add("Fast");


            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.Items.Add("Select Option");
            comboBox2.Items.Add("%30");
            comboBox2.Items.Add("%60");
            comboBox2.Items.Add("%100");

            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.Items.Add("Select Option");
            comboBox3.Items.Add("Low");
            comboBox3.Items.Add("Medium");
            comboBox3.Items.Add("High");

        }
        
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() == "Select Option")
            {
                MessageBox.Show("Select option.");
            }

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null && comboBox3.SelectedItem.ToString() == "Select Option")
            {
                MessageBox.Show("Select option.");
            }

        }
        private async void BlinkControl(Control control)
        {
            isBlinking = true;
            while (isBlinking)
            {
                control.Visible = !control.Visible; 
                await Task.Delay(500); 
            }
            control.Visible = true; 
        }
        private void StopBlinking(Control control)
        {
            Thread.Sleep(300);
            control.Visible = true; 
            isBlinking = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
               
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendSelectedOption("CloseCooling");
            if (carCheck3 == true)
            {
                progressBar1.Value = 100;
                label8.Text = "Processing...%100";
            }
            Invoke(new Action(() =>
            {
                if (checkedListBox1.Items.Count > 0)
                {
                    checkedListBox1.SetItemChecked(6, true);
                    checkedListBox1.SetItemChecked(7, true);
                }
                StopBlinking(pictureBox4);
                label6.ForeColor = System.Drawing.Color.Black;
            }));
            carInsýde = false;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedOption = comboBox1.SelectedItem.ToString();
                Console.WriteLine(selectedOption);

                if (selectedOption != "Select Option" && carInsýde==false)
                {
                    //UDP ile gönder
                    SendSelectedOption(selectedOption);
                }
                else if (carInsýde)
                {
                    MessageBox.Show("Please, Wait!!!");
                }
                else
                {
                    MessageBox.Show("Lütfen bir seçenek seçin.");
                }
            }
        }

        private void SendSelectedOption(string selectedOption)
        {
            TcpClient tcpClient = new TcpClient();

            try
            {
                IPAddress serverIP = IPAddress.Parse("192.168.1.33");  // Raspberry Pi'nin IP 
                int port = 5555; 

                tcpClient.Connect(serverIP, port);

                NetworkStream stream = tcpClient.GetStream(); // Veri gönderimi için bir akýþ al

                string message = selectedOption;
                byte[] data = Encoding.ASCII.GetBytes(message);

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Gönderilen mesaj : {0} ", selectedOption);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                tcpClient.Close(); 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendSelectedOption("CloseDoor");
            Invoke(new Action(() =>
            {
                if (checkedListBox1.Items.Count > 0)
                {
                    checkedListBox1.SetItemChecked(3, true);
                }
                label3.ForeColor = System.Drawing.Color.Black;
                StopBlinking(pictureBox2);
                progressBar1.Value = 40;
                label8.Text = "Processing...%40";
            }));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendSelectedOption("DefaultAngle");
            if (carCheck2 == true)
            {
                progressBar1.Value = 70;
                label8.Text = "Processing...%70";
            }

            Invoke(new Action(() =>
            {
                if (checkedListBox1.Items.Count > 0)
                {
                    checkedListBox1.SetItemChecked(4, true);
                    checkedListBox1.SetItemChecked(5, true);
                }
                label5.ForeColor = System.Drawing.Color.Black;
                StopBlinking(pictureBox3);
            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(carCheck2 == true)
            {
                progressBar1.Value = 60;
                label8.Text = "Processing...%60";
            }
            if (comboBox2.SelectedItem != null)
            {
                string selectedOption = comboBox2.SelectedItem.ToString();
                if (selectedOption != "Select Option")
                {
                    //UDP ile gönder
                    SendSelectedOption(selectedOption);
                }
                else
                {
                    MessageBox.Show("Lütfen bir seçenek seçin.");
                }
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Clean()
        {
            if (checkedListBox1.Items.Count > 0)
            {
                checkedListBox1.SetItemChecked(0, false);
                checkedListBox1.SetItemChecked(1, false);
                checkedListBox1.SetItemChecked(2, false);
                checkedListBox1.SetItemChecked(3, false);
                checkedListBox1.SetItemChecked(4, false);
                checkedListBox1.SetItemChecked(5, false);
                checkedListBox1.SetItemChecked(6, false);
                checkedListBox1.SetItemChecked(7, false);
            }
            if (checkedListBox2.Items.Count > 0)
            {
                checkedListBox2.SetItemChecked(0, false);
            }
            label8.Text = "Processing...%0";
            progressBar1.Value = 0;

            isBlinking = false;
            carInsýde = false;
            carCheck = false;
            carCheck2 = false;
            carCheck3 = false;
            
            label3.ForeColor = System.Drawing.Color.Black;
            label5.ForeColor = System.Drawing.Color.Black;
            label6.ForeColor = System.Drawing.Color.Black;
            label2.ForeColor = System.Drawing.Color.Black;
            label4.ForeColor = System.Drawing.Color.Black;

            StopBlinking(pictureBox2);
            StopBlinking(pictureBox3);
            StopBlinking(pictureBox4);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Clean();
        }
    }

}
