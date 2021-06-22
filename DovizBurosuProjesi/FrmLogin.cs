using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DovizBurosuProjesi
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        public static string tckn;

        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-INL76RD3\SQLEXPRESS;Initial Catalog=DbDovizProje;Integrated Security=True");

        private void btnForgetPassword_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            txtConfirmPassword.Visible = true;
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            maskTCKN.Focus();
            btnLogin.Visible = false;
            btnForgetPassword.Visible = false;
            btnAccept.Visible = true;
            btnUyeOl.Visible = true;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text==txtConfirmPassword.Text)
            {
                MessageBox.Show("Parola değiştirme işlemi gerçekleştirildi.");
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * From TblDoviz where TCKN = @p1 and Password = @p2",con);
            cmd.Parameters.AddWithValue("@p1",maskTCKN.Text);
            cmd.Parameters.AddWithValue("@p2",txtPassword.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            if(dr.Read())
            {
                Form1 fr = new Form1();
                fr.Show();
            }
            else
            {
                MessageBox.Show("Lütfen bilgileri doğru giriniz.");
            }
            con.Close();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            maskTCKN.Focus();
            tckn = maskTCKN.Text;
        }

        private void btnUye_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == txtConfirmPassword.Text)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Insert into TblDoviz (TCKN, Password, TL, Dolar, Euro) values (@p1, @p2, 0, 0, 0)", con);
                cmd.Parameters.AddWithValue("@p1", maskTCKN.Text);
                cmd.Parameters.AddWithValue("@p2", txtPassword.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Üyelik işlemi gerçekleştirildi.");
                btnLogin.Visible = true;
                btnForgetPassword.Visible = true;
                label3.Visible = false;
                txtConfirmPassword.Visible = false;
                btnAccept.Visible = false;
                btnUyeOl.Visible = false;
            }
            else
            {
                MessageBox.Show("Parolalar uyuşmuyor.");
                txtPassword.Clear();
                txtConfirmPassword.Clear();
            }
            
        }
    }
}
