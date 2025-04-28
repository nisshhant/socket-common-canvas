using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace MultiClient1._0
{
    public partial class Form1 : Form
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private const string SERVER_IP = "127.0.0.1"; // Server IP address (local server for now)
        private const int SERVER_PORT = 5000; // Server Port

        private bool isDrawing = false;
        private Point lastPoint;

        public Form1()
        {
            InitializeComponent();
            Thread client = new Thread(ConnectToServer);
            client.Start();

            
        }

        

        private void ConnectToServer()
        {
            try
            {
                // Connect to the server
                tcpClient = new TcpClient(SERVER_IP, SERVER_PORT);
                stream = tcpClient.GetStream();
                Thread receiveThread = new Thread(ReceiveDrawingData);
                receiveThread.IsBackground = true;
                receiveThread.Start();
                // Send a "Hello" message to the server
                //SendHelloMessage();
            }
            catch (Exception ex)
            {
                // Handle error in connection
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }

        //private void SendHelloMessage()
        //{
        //    try
        //    {
        //        string message = "Hello from client!";
        //        byte[] data = Encoding.ASCII.GetBytes(message);
        //        stream.Write(data, 0, data.Length);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error sending message: " + ex.Message);
        //    }
        //}

        private void SendDrawingData(string action, int x, int y, bool isDrawing)
        {
            var drawingData = new
            {
                action = action,
                x = x,
                y = y,
                isDrawing = isDrawing
            };

            string dataString = JsonConvert.SerializeObject(drawingData);

            // Add a special symbol as delimiter
            dataString += "|";  // <-- using '|' as the separator

            byte[] data = Encoding.ASCII.GetBytes(dataString);

            if (tcpClient != null && tcpClient.Connected)
            {
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
            }
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            lastPoint = e.Location;
            // Send the starting point to the server
            SendDrawingData("start", e.X, e.Y, true);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Draw while the user moves the mouse (dragging)
                using (Graphics g = drawingPanel.CreateGraphics())
                {
                    g.DrawLine(new Pen(Color.Black, 2), lastPoint, e.Location);
                }

                // Send drawing data to the server as the user moves the mouse
                SendDrawingData("draw", e.X, e.Y, true);
                lastPoint = e.Location;
            }
        }

        private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            // Send stopping point to the server
            SendDrawingData("stop", e.X, e.Y, false);
        }

        private void DrawOnCanvas(int x, int y)
        {
            using (Graphics g = drawingPanel.CreateGraphics())
            {
                g.DrawEllipse(new Pen(Color.Black, 2), x, y, 1, 1);  // Draw a point
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }


        private void ReceiveDrawingData()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                StringBuilder receivedData = new StringBuilder();

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    receivedData.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    // Now process complete messages
                    string allData = receivedData.ToString();
                    string[] messages = allData.Split('|');  // Split at '|'

                    // Process all complete messages except the last one
                    for (int i = 0; i < messages.Length - 1; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(messages[i]))
                        {
                            var drawingMessage = JsonConvert.DeserializeObject<dynamic>(messages[i]);

                            string action = drawingMessage.action;
                            int x = drawingMessage.x;
                            int y = drawingMessage.y;
                            bool isDrawing = drawingMessage.isDrawing;

                            if (action == "start")
                            {
                                lastPoint = new Point(x, y);
                            }
                            else if (action == "draw")
                            {
                                DrawLineFromLastPoint(x, y);
                            }
                            else if (action == "stop")
                            {
                                // Optionally reset if needed
                            }
                        }
                    }

                    // Keep the incomplete last message (after last delimiter)
                    receivedData.Clear();
                    receivedData.Append(messages[^1]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error receiving data: " + ex.Message);
            }
        }


        private void DrawLineFromLastPoint(int x, int y)
        {
            if (drawingPanel.InvokeRequired)
            {
                drawingPanel.Invoke(new Action<int, int>(DrawLineFromLastPoint), x, y);
            }
            else
            {
                using (Graphics g = drawingPanel.CreateGraphics())
                {
                    g.DrawLine(new Pen(Color.Blue, 2), lastPoint, new Point(x, y));
                }
                lastPoint = new Point(x, y);
            }
        }



    }
}
