using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sunucurev2
{
    public partial class Form1 : Form
    {
        TcpListener serverSocket;
        public readonly object lockObject = new object();
        public Dictionary<string, TcpClient> clientsList = new Dictionary<string, TcpClient>();

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
          
        }
        bool isServerRunning = false; // Sunucu durumunu takip etmek için bayrak
        private void Form1_Load(object sender, EventArgs e)
        {


            if (serverSocket != null)
            {
                serverSocket.Stop(); // Önceki sunucu soketini durdur
            }
       
            serverSocket = new TcpListener(IPAddress.Any, 8888);
            serverSocket.Start();
            if (!textBox1.Text.Contains("Sunucu başlatıldı..."))
            {
                textBox1.AppendText("Sunucu başlatıldı...\n");
            }
            isServerRunning = true;
              // İstemci bağlantılarını dinleyen bir iş parçacığı başlat
            Thread serverThread = new Thread(ListenForClients);
            serverThread.Start();

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isServerRunning)
            {
                serverSocket.Stop();
                isServerRunning = false;

                // İstemci soketleri ve iş parçacıklarını temizleme
                foreach (TcpClient client in clientsList.Values)
                {
                    client.Close();
                }
                clientsList.Clear();
            }
        }
        private void ListenForClients()
        {
            while (true)
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = serverSocket.AcceptTcpClient();
                textBox1.Invoke((MethodInvoker)delegate
                {
                    textBox1.AppendText("Bağlantı alındı...\n");
                });

                ClientHandler client = new ClientHandler();
                client.StartClient(clientSocket, this);
            }
        }
        public void AddMessageToTextBox(string message)
        {
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.AppendText(message + "\n");
            });
        }
        public class ClientHandler
        {
            TcpClient clientSocket;
            NetworkStream networkStream;
            string clientId;

            public void StartClient(TcpClient inClientSocket, Form1 form)
            {
                this.clientSocket = inClientSocket;
                this.networkStream = clientSocket.GetStream();
                this.clientId = Guid.NewGuid().ToString();

                lock (form.lockObject)
                {
                    form.clientsList.Add(clientId, clientSocket);
                }

                Thread clientThread = new Thread(() => DoChat(form));
                clientThread.Start();
            }

            private void DoChat(Form1 form)
            {
                byte[] bytesFrom = new byte[10025];
                int bytesRead;

                string dataFromClient = string.Empty; // dataFromClient burada tanımlanmalı

                while (true)
                {
                    try
                    {
                        bytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                        if (bytesRead <= 0)
                        {
                            break;
                        }

                        dataFromClient = Encoding.UTF8.GetString(bytesFrom, 0, bytesRead);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                        if (dataFromClient.StartsWith("IP-")) // Gelen mesaj IP ile başlıyorsa
                        {
                            string clientIP = dataFromClient.Substring(3); // IP adresini al
                            form.AddMessageToTextBox("İstemciden bağlantı IP'si: " + clientIP);
                        }
                        else
                        {
                            form.AddMessageToTextBox("İstemciden gelen: " + dataFromClient);
                            Broadcast(dataFromClient, form);
                        }
                    }
                    catch (Exception ex)
                    {
                        form.AddMessageToTextBox("Bir istemci ayrıldı.");
                        break;
                    }
                }

                lock (form.lockObject)
                {
                    form.clientsList.Remove(clientId);
                }

                clientSocket.Close();
                networkStream.Close();
            }

            public void Broadcast(string msg, Form1 form)
            {
                TcpClient[] connectedClients;

                lock (form.lockObject)
                {
                    connectedClients = form.clientsList.Values.ToArray();
                }

                foreach (TcpClient broadcastSocket in connectedClients)
                {
                    try
                    {
                        NetworkStream broadcastStream = broadcastSocket.GetStream();
                        Byte[] broadcastBytes = Encoding.UTF8.GetBytes(msg + "$");
                        broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                        broadcastStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        form.AddMessageToTextBox(ex.ToString());
                    }
                }
            }
        }
    }
}