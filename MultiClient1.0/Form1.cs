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


        private void HandleClientComm(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            NetworkStream stream = tcpClient.GetStream();

            byte[] buffer = new byte[1024]; // Buffer for reading data
            int bytesRead;

            try
            {
                // Keep reading until the client disconnects or there is no more data to read
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    // Deserialize the received drawing data
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    var drawingMessage = JsonConvert.DeserializeObject<dynamic>(receivedData);

                    // Extract values from the received drawing data
                    string action = drawingMessage.action;
                    int x = drawingMessage.x;
                    int y = drawingMessage.y;
                    bool isDrawing = drawingMessage.isDrawing;

                    // Now, process the drawing data based on the action
                    if (action == "start")
                    {
                        // Handle the starting point (client begins drawing)
                        //UpdateUI($"Drawing started at: ({x}, {y})");
                    }
                    else if (action == "draw")
                    {
                        // Handle the drawing data (client is drawing)
                        DrawOnCanvas(x, y);  // Call method to draw on canvas
                    }
                    else if (action == "stop")
                    {
                        // Handle the stopping point (client stops drawing)
                        //UpdateUI($"Drawing stopped at: ({x}, {y})");
                    }
                }
            }
            catch (Exception ex)
            {
                //UpdateUI("Error with client communication: " + ex.Message);
            }
            finally
            {
                // Clean up - close the connection for the current client
                tcpClient.Close();
                //clients.Remove(tcpClient); // Remove the client from your list of connected clients
                //UpdateUI("Client disconnected");
            }
        }

        private void DrawOnCanvas2(int x, int y)
        {
            // This is the method that will be used to draw on the server's canvas.
            // You need to call this method to draw on the server side when a drawing data is received.
            // Assuming you have a panel (drawingPanel) to draw on the UI of your server.
            if (InvokeRequired)
            {
                // Ensure thread-safety if you are updating the UI from another thread
                Invoke(new Action<int, int>(DrawOnCanvas2), x, y);
            }
            else
            {
                using (Graphics g = drawingPanel.CreateGraphics())
                {
                    g.DrawEllipse(new Pen(Color.Black, 2), x, y, 1, 1);  // Draw a point on the canvas
                }
            }
        }

    }
}
