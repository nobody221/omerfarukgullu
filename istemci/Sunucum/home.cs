using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sunucum
{
    public partial class home : Form
    {
        public home()
        {
            InitializeComponent();
        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
           
        }

        

        private void home_Load(object sender, EventArgs e)
        {
            
        }

       

        private void txtname_KeyPress_1(object sender, KeyPressEventArgs e)             //Olası hatalar düzeltildiği için gerek kalmadı
        // Türkçe karakter girilmesini engelleyen metot
        //sunucumuz konsol da çalıştıgı için olası hatalara karşı önlem aldık
        {
          //  char[] turkishCharacters = { 'ğ', 'Ğ', 'ı', 'İ', 'ö', 'Ö', 'ü', 'Ü', 'ş', 'Ş', 'ç', 'Ç' };//türkçe karakterleri listeye ekledik
          //
          //  if (turkishCharacters.Contains(e.KeyChar)) //girilen karaketrin türkçe olup olmadıgını sorguladık
          //  {
          //      e.Handled = true;
          //  }
        }

        private void txtname_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)  
            {
                // Buraya Enter tuşuna basıldığında çalışmasını istediğiniz işlemi yazdık
                Form1 frm = new Form1();
                frm.kullanıcıname = txtname.Text; //2. forma giriş yapan kullanıcının adını aktardık
                frm.Show();
                this.Hide();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text==""||txtname.Text=="")    //textboxlar boş iken butona basıldıysa işlemleri yaptırmadık
            {
                MessageBox.Show("Gerekli alanları doldurunuz.");

            }
            else //textboxlar dolu ise işleme aldık
            {
                Form1 frm = new Form1();
                frm.kullanıcıname = txtname.Text;
                frm.baglanılacakip = textBox1.Text;
                frm.Show();
                this.Hide();
               
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit(); //programdan çıkış butonumuz
        }
        private bool isDragging = false;
        private Point lastLocation;        // burada formun üst kısmında bulunan panele basılı tutuğumuzda haraket etmesi için gerekli kodları yazdık
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastLocation = e.Location;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)  // burada formun üst kısmında bulunan panele basılı tutuğumuzda haraket etmesi için gerekli kodları yazdık
        {
            if (isDragging)
            {
                Point newLocation = this.Location;
                newLocation.X += e.X - lastLocation.X;
                newLocation.Y += e.Y - lastLocation.Y;
                this.Location = newLocation;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e) // burada formun üst kısmında bulunan panele basılı tutuğumuzda haraket etmesi için gerekli kodları yazdık
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
           
        }
    }
}
