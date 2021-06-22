using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;

namespace DovizBurosuProjesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-INL76RD3\SQLEXPRESS;Initial Catalog=DbDovizProje;Integrated Security=True");

        bool alim = false;
        bool satim = false;
        bool euroAlim = false;
        bool euroSatim = false;
        bool dolarAlim = false;
        bool dolarSatim = false;

        int tutar;
        int euroCuzdan, dolarCuzdan, tlCuzdan;

        private void Form1_Load(object sender, EventArgs e)
        {
            //Hangi TC Kimlik Numarası ile giriş yapılıyorsa onu Label' a aktarma
            
            this.Text = "Hoşgeldiniz " + FrmLogin.tckn;

            //XML sayfasından kur bilgilerini alma ve ilgili label' lara aktarma

            string today = "https://www.tcmb.gov.tr/kurlar/today.xml";

            var xmlFile = new XmlDocument();
            xmlFile.Load(today);
            string dolarAlis = xmlFile.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteBuying").InnerXml; //Xml sayfasında "Tarih_Date" -> "Currency" -> "BanknoteBuying" içerisinde kalan "USD" kolonuna ait değeri al.
            lblDolarAl.Text = dolarAlis;
            string dolarSatis = xmlFile.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling").InnerXml; //Xml sayfasında "Tarih_Date" -> "Currency" -> "BanknoteSelling" içerisinde kalan "USD" kolonuna ait değeri al.
            lblDolarSat.Text = dolarSatis;
            string euroAlis = xmlFile.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteBuying").InnerXml; //Xml sayfasında "Tarih_Date" -> "Currency" -> "BanknoteBuying" içerisinde kalan "EUR" kolonuna ait değeri al.
            lblEuroAl.Text = euroAlis;
            string euroSatis = xmlFile.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteSelling").InnerXml; //Xml sayfasında "Tarih_Date" -> "Currency" -> "BanknoteSelling" içerisinde kalan "EUR" kolonuna ait değeri al.
            lblEuroSat.Text = euroSatis;

            //Veritabanında mevcut kullanıcıya ait olan bakiyeyi görüntüleme
            con.Open();
            SqlCommand bakiyeGoruntule = new SqlCommand("Select DOLAR, EURO, TL from TblDoviz where TCKN = @p1",con);
            bakiyeGoruntule.Parameters.AddWithValue("@p1",FrmLogin.tckn);
            SqlDataReader dr = bakiyeGoruntule.ExecuteReader();
            while (dr.Read())
            {
                dolarCuzdan = Convert.ToInt32(dr[0]);
                lblDolarAdet.Text = dolarCuzdan.ToString();
                euroCuzdan = Convert.ToInt32(dr[1]);
                lblEuroAdet.Text = euroCuzdan.ToString();
                tlCuzdan = Convert.ToInt32(dr[2]);
                lblTLAdet.Text = tlCuzdan.ToString();
            }
            con.Close();
        }

        //"..." butonuna tıklandığında karşısındaki label' a ait değeri txtKur' a yazar.

        private void btnDolarAl_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarAl.Text;
            dolarAlim = true;
        }

        private void btnDolarSat_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblDolarSat.Text;
            dolarSatim = true;
        }

        private void btnEuroAl_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroAl.Text;
            euroAlim = true;
        }

        private void btnEuroSat_Click(object sender, EventArgs e)
        {
            txtKur.Text = lblEuroSat.Text;
            euroSatim = true;
        }

        private void btnSatis_MouseMove(object sender, MouseEventArgs e)
        {
            lblBilgi.Visible = true;
            lblBilgi.Text = "Satış işlemini gerçekleştirmek için satmak istediğiniz para biriminin karşısındaki butona tıklayın, ardından 'Miktar' bölümüne değer giriniz ve 'Satış Yap' butonuna basınız. (Tutar = Kur * Miktar)";
        }

        private void btnAlim_MouseMove(object sender, MouseEventArgs e)
        {
            lblBilgi.Visible = true;
            lblBilgi.Text = "Alım işlemini gerçekleştirmek için almak istediğiniz miktarı 'Miktar' bölümüne girin, satın almak istediğiniz para biriminin karşısındaki butona tıklayın ve 'Alım yap' butonuna basınız. (Tutar = Miktar / Kur)";
        }

        private void btnSatis_MouseLeave(object sender, EventArgs e)
        {
            lblBilgi.Visible = false;
        }

        SqlCommand cmd;

        private void btnOnay_Click(object sender, EventArgs e) //Burada yapılacak işlem; Alım veya Satış emri verildikten sonra elde edilen bütçenin veritabanına aktarılmasıdır.
        {
            con.Open();
            
            if (alim==true)
            {
                if (euroAlim==true)
                {
                    euroCuzdan = euroCuzdan + tutar;
                    cmd = new SqlCommand("Update TblDoviz set EURO = @p1 where TCKN = @p2",con);
                    cmd.Parameters.AddWithValue("@p1", euroCuzdan);
                    cmd.Parameters.AddWithValue("@p2", FrmLogin.tckn);
                }
                if (dolarAlim == true)
                {
                    dolarCuzdan = dolarCuzdan + tutar;
                    cmd = new SqlCommand("Update TblDoviz set DOLAR = @p1 where TCKN = @p2", con);
                    cmd.Parameters.AddWithValue("@p1", dolarCuzdan);
                    cmd.Parameters.AddWithValue("@p2", FrmLogin.tckn);
                }
            }
            if (satim==true)
            {
                if (euroSatim == true)
                {
                    euroCuzdan = euroCuzdan - tutar;
                    cmd = new SqlCommand("Update TblDoviz set EURO = @p1 where TCKN = @p2", con);
                    cmd.Parameters.AddWithValue("@p1", euroCuzdan);
                    cmd.Parameters.AddWithValue("@p2", FrmLogin.tckn);
                }
                if (dolarSatim == true)
                {
                    dolarCuzdan = dolarCuzdan - tutar;
                    cmd = new SqlCommand("Update TblDoviz set DOLAR = @p1 where TCKN = @p2", con);
                    cmd.Parameters.AddWithValue("@p1", dolarCuzdan);
                    cmd.Parameters.AddWithValue("@p2", FrmLogin.tckn);
                }
            }
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("İşlem başarılı.");
        }

        private void btnAlim_MouseLeave(object sender, EventArgs e)
        {
            lblBilgi.Visible = false;
        }

        private void btnAlim_Click(object sender, EventArgs e)
        {
            double kur = Convert.ToDouble(txtKur.Text);
            int miktar = Convert.ToInt32(txtMiktar.Text);
            tutar = Convert.ToInt32(miktar/kur);
            txtTutar.Text = tutar.ToString();
            double kalan = miktar % kur;
            txtKalan.Text = kalan.ToString();
            alim = true;
        }

        private void btnSatis_Click(object sender, EventArgs e)
        {
            double kur, miktar;
            kur = Convert.ToDouble(txtKur.Text);
            miktar = Convert.ToDouble(txtMiktar.Text);
            tutar = Convert.ToInt32(kur * miktar);
            txtTutar.Text = tutar.ToString();
            satim = true;
        }

        private void txtKur_TextChanged(object sender, EventArgs e) //İşlem hatasını önlemek için textBox' taki nokta ile ayrılan kur' u virgül ile ayırdık
        {
            txtKur.Text = txtKur.Text.Replace(".",",");
        }
    }
}
