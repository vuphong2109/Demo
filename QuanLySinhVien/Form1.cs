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
using System.Xml.Linq;

namespace QuanLySinhVien
{
    public partial class Form1 : Form
    {
        static string connString = @"Data Source=DESKTOP-GK2QNSC;Initial Catalog=QLSinhVien;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
        }

        public DataTable GetData(string query, params SqlParameter[] args)
        {
            DataTable dt = new DataTable();
            using(SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(args);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                return dt;
            }
        }
        private bool ExcuteQuery(string query, params SqlParameter[] args)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddRange(args); // Thêm tham số vào câu lệnh sql
                    cmd.ExecuteNonQuery(); // Thực thi câu lệnh không trả về dữ liệu
                }
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string query = @"
                    Select MaSV as [Mã sinh viên], HoTen as [Họ Tên], NgaySinh as [Ngày Sinh], Lop as [Lớp], CASE WHEN GioiTinh = 1 THEN 'Nam' ELSE N'Nữ' END AS [Giới Tính] from SinhVien";
            dtgvSinhVien.DataSource = GetData(query);
        }   

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maSV = txbMaSV.Text;
            string hoTen = txbHoTen.Text;
            string ngaySinh = dtpNgaySinh.Value.ToString("dd/MM/yyyy");
            string lop = txbLop.Text;
            string gioiTinh = rdbNam.Checked ? "1" : "0";

            string query = "Insert into SinhVien Values(@MaSV, @HoTen, @NgaySinh, @Lop, @GioiTinh)";
            SqlParameter[] parameters =
            {
                new SqlParameter("@MaSV", maSV),
                new SqlParameter("@HoTen", hoTen),
                new SqlParameter("@NgaySinh", ngaySinh),
                new SqlParameter("@Lop", lop),
                new SqlParameter("@GioiTinh", gioiTinh)
            };
            if(ExcuteQuery(query, parameters))
            {
                MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form1_Load(sender, e);
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm sinh viên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string query = "Delete from SinhVien where MaSV = @MaSV";
            if (ExcuteQuery(query, new SqlParameter("@MaSV", txbMaSV.Text)))
            {
                MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form1_Load(sender, e);
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi xóa sinh viên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maSV = txbMaSV.Text;
            string hoTen = txbHoTen.Text;
            string ngaySinh = dtpNgaySinh.Value.ToString("dd/MM/yyyy");
            string lop = txbLop.Text;
            string gioiTinh = rdbNam.Checked ? "1" : "0";

            string query = "Update SinhVien set MaSV = @MaSV, HoTen = @HoTen, NgaySinh = @NgaySinh, Lop = @Lop, GioiTinh = @GioiTinh Where MaSV = @MaSV";
            SqlParameter[] parameters =
            {
                new SqlParameter("@MaSV", maSV),
                new SqlParameter("@HoTen", hoTen),
                new SqlParameter("@NgaySinh", ngaySinh),
                new SqlParameter("@Lop", lop),
                new SqlParameter("@GioiTinh", gioiTinh)
            };
            if(ExcuteQuery(query, parameters))
            {
                MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form1_Load(sender, e);
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật sinh viên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string search = txbSearch.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                Form1_Load(sender, e);
                return;
            }
            string query = @"Select MaSV as [Mã sinh viên], HoTen as [Họ Tên], NgaySinh as [Ngày Sinh], Lop as [Lớp], CASE WHEN GioiTinh = 1 THEN 'Nam' ELSE N'Nữ' END AS [Giới Tính]
                            from SinhVien
                            where MaSV like @search or HoTen like @search or Lop like @search";
            DataTable kq = GetData(query, new SqlParameter("@search", "%" + search + "%"));
            dtgvSinhVien.DataSource = kq;
        }
        private void dtgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = dtgvSinhVien.CurrentRow.Index;
            txbMaSV.Text = dtgvSinhVien.Rows[i].Cells[0].Value.ToString();
            txbHoTen.Text = dtgvSinhVien.Rows[i].Cells[1].Value.ToString();
            dtpNgaySinh.Text = dtgvSinhVien.Rows[i].Cells[2].Value.ToString();
            txbLop.Text = dtgvSinhVien.Rows[i].Cells[3].Value.ToString();
            //Giới tính
            if (dtgvSinhVien.Rows[i].Cells[4].Value.Equals("Nam"))
            {
                rdbNam.Checked = true;
            }
            else
            {
                rdbNu.Checked = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            formDangNhap fLogin = new formDangNhap();
            fLogin.FormClosed += (s, args) => Application.Exit();
            fLogin.Show();
            this.Hide();
            // this.FormClosed += (s, args) => Application.Exit();
        }
    }
}
