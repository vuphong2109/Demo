using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLySinhVien
{
    public partial class formDangNhap : Form
    {
        static string connString = @"Data Source=DESKTOP-GK2QNSC;Initial Catalog=QLSinhVien;Integrated Security=True";
        public formDangNhap()
        {
            InitializeComponent();
        }
        private String Scalar(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddRange(parameters);
                    return (cmd.ExecuteScalar() == null) ? null : (cmd.ExecuteScalar().ToString());
                }
            }
            catch(Exception ex) {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if(txbTaiKhoan.Text.Length > 0 && txbMatKhau.Text.Length > 0) {
                string query = "Select MatKhau from Account where TaiKhoan = @TK";
                string MK_check = Scalar(query, new SqlParameter("@TK", txbTaiKhoan.Text));
                if(!string.IsNullOrEmpty(MK_check))
                {
                    if(MK_check.Equals(txbMatKhau.Text))
                    {
                        Form1 form1 = new Form1();
                        form1.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Sai mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txbMatKhau.Clear();
                        txbMatKhau.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Tài khoản không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txbTaiKhoan.Clear();
                    txbMatKhau.Clear();
                    txbTaiKhoan.Focus();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txbTaiKhoan.Text))
                {
                    MessageBox.Show("Chưa nhập tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txbTaiKhoan.Focus();
                }
                else if(string.IsNullOrEmpty(txbMatKhau.Text))
                {
                    MessageBox.Show("Chưa nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txbMatKhau.Focus();
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Bạn có muốn thoát không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void pbShowPass_MouseDown(object sender, MouseEventArgs e)
        {
            txbMatKhau.UseSystemPasswordChar = false;
        }

        private void pbShowPass_MouseUp(object sender, MouseEventArgs e)
        {
            txbMatKhau.UseSystemPasswordChar = true;
        }
    }
}
