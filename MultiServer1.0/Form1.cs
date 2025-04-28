using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace MultiServer1._0
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private Thread listenerThread;
        private List<TcpClient> clients;
        private const int PORT = 5000;
        public Form1()
        {
            InitializeComponent();
            clients = new List<TcpClient>();
        }

        private void StartServerButton_Click(object sender, EventArgs e)
        {
            StartServer();
        }
        private void StartServer()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, PORT);
                tcpListener.Start();
                UpdateUI("Server started on port " + PORT);

                listenerThread = new Thread(ListenForClients);
                listenerThread.IsBackground = true;
                listenerThread.Start();
            }
            catch (Exception ex)
            {
                UpdateUI("Error starting server: " + ex.Message);
            }
        }

        private void ListenForClients()
        {
            while (true)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    clients.Add(tcpClient);
                    UpdateUI("Client connected");

                    // Start a new thread for handling this client
                    Thread clientThread = new Thread(HandleClientComm);
                  
                    clientThread.IsBackground = true;
                    clientThread.Start(tcpClient);
                 
                }
                catch (Exception ex)
                {
                    UpdateUI("Error accepting client: " + ex.Message);
                }
            }
        }

        private void UpdateUI(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateUI), message);
            }
            else
            {
                listBoxMessages.Items.Add(message);
            }
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

                    // Update server UI
                    UpdateUI("Received: " + receivedData);

                    // Send the same drawing data to all other connected clients
                    BroadcastToAllClients(receivedData, tcpClient);
                }
            }
            catch (Exception ex)
            {
                UpdateUI("Error with client communication: " + ex.Message);
            }
            finally
            {
                // Clean up - close the connection for the current client
                tcpClient.Close();
                lock (clients)
                {
                    clients.Remove(tcpClient); // Remove the client from your list of connected clients
                }
                UpdateUI("Client disconnected");
            }
        }

        private void BroadcastToAllClients(string message, TcpClient senderClient)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            lock (clients)
            {
                foreach (var client in clients)
                {
                    try
                    {
                        if (client != senderClient && client.Connected)
                        {
                            NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateUI("Error sending to client: " + ex.Message);
                    }
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void closeServer()
        {
            tcpListener.Dispose();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            closeServer();
        }
    }
}
