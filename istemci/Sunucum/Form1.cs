using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Sunucum
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient(); // TcpClient nesnesi oluşturuluyor.
        NetworkStream serverStream; // NetworkStream değişkeni tanımlanıyor.

        public Form1()
        {
            InitializeComponent(); // Form bileşenlerini başlatan metot çağrılıyor.
        }
        public string kullanıcıname, baglanılacakip; // Kullanıcı adı ve bağlanılacak IP adresi string olarak tanımlanıyor.

        private void Form1_Load(object sender, EventArgs e)
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            Console.WriteLine(hostName);
            // Get the IP
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
           
         // textBoxMessage.Text = myIP;
           




            richTextBoxChat.ReadOnly = true;
            try
            {
                clientSocket.Connect(baglanılacakip, 8888); // Belirtilen IP ve porta bağlanmaya çalışılıyor.
                serverStream = clientSocket.GetStream(); // Sunucu ile iletişim için bir NetworkStream oluşturuluyor.
                DisplayMessage("Sunucuya bağlandı..."); // Mesajı ekrana yazdıran metot çağrılıyor.
                byte[] outStream = Encoding.UTF8.GetBytes("IP-" + myIP + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                // Sunucudan gelen mesajları dinlemek için bir görev başlatılıyor.
                Task.Run(() => ReceiveMessages());
            }
            catch (Exception ex)
            {
                DisplayMessage("Sunucuya bağlanılamadı: " + ex.Message); // Hata durumunda mesajı ekrana yazdıran metot çağrılıyor.
                MessageBox.Show("IP Adresini Kontrol Ediniz"); // Hata mesajını gösteren bir ileti kutusu görüntüleniyor.
                this.Hide(); // Form gizleniyor.
                home frm = new home(); // 'home' adında bir form nesnesi oluşturuluyor.
                frm.ShowDialog(); // Oluşturulan formu gösteren bir ileti kutusu görüntüleniyor.
            }
        }

        private void ConnectToServer() // gerek kalmadı
        {
        //   try
        //   {
        //       clientSocket.Connect("192.168.0.26", 8888);
        //       serverStream = clientSocket.GetStream();
        //       DisplayMessage("Sunucuya bağlandı...");
        //   }
        //   catch (Exception ex)
        //   {
        //       DisplayMessage("Sunucuya bağlanılamadı: " + ex.Message);
        //   }
        }

     
        
        // Bu metod, eğer Invoke gerekliyse metodu yeniden çağırarak mesajı ekrana yazdırmak için kullanılır.
        private void DisplayMessage(string message)
        {
            // Eğer çağrıyı yapan thread farklıysa (Invoke gerekliyse),
            // DisplayMessage metodunu uygun thread üzerinde yeniden çağırarak ekrana yazdırmayı sağlar.
            if (InvokeRequired)
            {
                // İş parçacığı güvenliği için Invoke kullanılır ve DisplayMessage metodu yeniden çağrılır.
                Invoke(new MethodInvoker(delegate
                {
                    DisplayMessage(message);
                }));
            }
            else
            {
                // Eğer mevcut thread uygunsa, richTextBoxChat'e mesaj eklenir.
                richTextBoxChat.AppendText(message + "\n");
            }
        }


        // Bu metod, sunucudan gelen mesajları almak ve ekrana yazdırmak için kullanılır.
        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    byte[] inStream = new byte[10025];
                    int bytesRead = serverStream.Read(inStream, 0, inStream.Length);

                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    // Gelen veri UTF-8 formatına dönüştürülür.
                    string dataFromServer = Encoding.UTF8.GetString(inStream, 0, bytesRead);
                    dataFromServer = dataFromServer.Substring(0, dataFromServer.IndexOf("$"));

                    if (!dataFromServer.StartsWith(kullanıcıname + ":"))
                    {
                        DisplayMessage(dataFromServer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // Bu metod, sunucudan çıkış yaparak bağlantıyı sonlandırmak için kullanılır.
        private void DisconnectFromServer()
        {
            if (clientSocket != null)
            {
                // Eğer clientSocket varsa, bağlantı kapatılır ve kullanıcıya sunucudan çıkıldığı bilgisi ekrana yazdırılır.
                clientSocket.Close();
                clientSocket.Dispose();
                DisplayMessage("Sunucudan çıkıldı...");
            }
        }


        // Bu metot, button1'e tıklandığında çalışır.
        private void button1_Click_1(object sender, EventArgs e)
        {
            DisconnectFromServer(); // Sunucudan bağlantıyı kesme işlemini gerçekleştirir.
            Application.Exit(); // Uygulamayı kapatır.
        }
        // Bu değişken fare ile sürükleme işleminin durumunu takip eder.
        private bool isDragging = false;

        // Fare konumunu saklamak için kullanılan bir nokta.
        private Point lastLocation;

        // panel1 üzerinde fare düğmesine basıldığında tetiklenen olay.;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true; // Fare sürükleme işlemi başladı.
                lastLocation = e.Location; // Son konumu güncelle.
            }
        }
        // panel1 üzerinde fare hareket ettiğinde tetiklenen olay.
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Yeni konumu hesapla ve panelin konumunu güncelle.
                Point newLocation = this.Location;
                newLocation.X += e.X - lastLocation.X;
                newLocation.Y += e.Y - lastLocation.Y;
                this.Location = newLocation;
            }
        }
        // panel1 üzerinde fare düğmesi bırakıldığında tetiklenen olay.
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false; // Fare sürükleme işlemi sona erdi.
            }

        }
        // Bu metot, button2'ye tıklandığında çalışır.
        private void button2_Click_1(object sender, EventArgs e)
        {
            string msg = textBoxMessage.Text.Trim();

            if (!string.IsNullOrEmpty(msg))
            {
                // Sunucuya gönderilen mesajın formatını belirler ve gönderir.
                byte[] outStream = Encoding.UTF8.GetBytes(kullanıcıname + ":" + msg + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                // Gönderilen mesajı görüntüler.
                DisplayMessage(kullanıcıname + ":" + msg);

                textBoxMessage.Clear(); // TextBox'ı temizler.
            }
        }
        // TextBox'ta bir tuşa basıldığında tetiklenen olay.
        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string msg = textBoxMessage.Text.Trim();

                if (!string.IsNullOrEmpty(msg))
                {
                    // Sunucuya gönderilen mesajın formatını belirler ve gönderir.
                    byte[] outStream = Encoding.UTF8.GetBytes(kullanıcıname + ":" + msg + "$");
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    // Gönderilen mesajı görüntüler.
                    DisplayMessage(kullanıcıname + ":" + msg);

                    textBoxMessage.Clear(); // TextBox'ı temizler.
                }
            }
        }
        }
    }

