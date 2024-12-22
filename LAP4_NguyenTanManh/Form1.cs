using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LAP4_NguyenTanManh.Models;

namespace LAP4_NguyenTanManh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Faculty> listFalcultys = context.Faculties.ToList(); //lấy các khoa
                List<Student> listStudent = context.Students.ToList(); //lấy sinh viên
                FillFalcultyCombobox(listFalcultys);
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.cmbKhoa.DataSource = listFalcultys;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                ValidateInput();

                // Kiểm tra xem mã số sinh viên đã tồn tại hay chưa
                int selectedRow = GetSelectedRow(txtMSSV.Text);
                if (selectedRow == -1)
                {
                    // Thêm mới nếu không tìm thấy
                    selectedRow = dgvStudent.Rows.Add();
                    MessageBox.Show("Thêm sinh viên mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Nếu tìm thấy, cập nhật dữ liệu
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Cập nhật hoặc thêm dữ liệu
                InsertUpdate(selectedRow);

                // Reset các ô nhập liệu và combobox về trạng thái mặc định
                ResetInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetSelectedRow(string studentID)
        {
            for (int i = 0; i < dgvStudent.Rows.Count; i++)
            {

                if (dgvStudent.Rows[i].Cells[0].Value != null &&
                !string.IsNullOrEmpty(dgvStudent.Rows[i].Cells[0].Value.ToString()))

                {

                    if (dgvStudent.Rows[i].Cells[0].Value.ToString() == studentID)

                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        private void ValidateInput()
        {
            // Kiểm tra các trường nhập liệu có để trống không
            if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||
                string.IsNullOrWhiteSpace(txtTen.Text) ||
                string.IsNullOrWhiteSpace(txtDTB.Text))
            {
                throw new Exception("Vui lòng nhập đầy đủ thông tin sinh viên.");
            }

            // Kiểm tra định dạng mã sinh viên
            if (!IsValidStudentID(txtMSSV.Text))
            {
                throw new Exception("Mã số sinh viên không hợp lệ. Vui lòng nhập đúng định dạng (10 chữ số).");
            }

            // Kiểm tra định dạng tên sinh viên
            if (!IsValidFullName(txtTen.Text))
            {
                throw new Exception("Tên sinh viên không hợp lệ. Tên phải là chữ, độ dài từ 3 đến 100 ký tự, không chứa ký tự đặc biệt.");
            }

            // Kiểm tra điểm trung bình có phải số hợp lệ và nằm trong khoảng 0-10
            if (!float.TryParse(txtDTB.Text, out float dtb) || dtb < 0 || dtb > 10)
            {
                throw new Exception("Điểm trung bình sinh viên không hợp lệ. Vui lòng nhập số thập phân từ 0 đến 10.");
            }
        }

        private bool IsValidStudentID(string studentID)
        {
            // Kiểm tra mã sinh viên có phải là chuỗi số và có đúng 10 ký tự
            return studentID.Length == 10 && studentID.All(char.IsDigit);
        }

        private bool IsValidFullName(string fullName)
        {
            // Kiểm tra độ dài tên từ 3 đến 100 ký tự
            if (fullName.Length < 3 || fullName.Length > 100)
                return false;

            // Kiểm tra tên chỉ chứa chữ cái và khoảng trắng
            foreach (char c in fullName)
            {
                if (!char.IsLetter(c) && c != ' ')
                    return false;
            }
            return true;
        }

        private void InsertUpdate(int selectedRow)
        {
            // Thực hiện cập nhật dữ liệu vào bảng (khi dữ liệu đã được kiểm tra)
            dgvStudent.Rows[selectedRow].Cells[0].Value = txtMSSV.Text;
            dgvStudent.Rows[selectedRow].Cells[1].Value = txtTen.Text;
            dgvStudent.Rows[selectedRow].Cells[3].Value = float.Parse(txtDTB.Text).ToString("0.00");
            dgvStudent.Rows[selectedRow].Cells[2].Value = cmbKhoa.Text;
        }

        private void ResetInputFields()
        {
            txtMSSV.Clear();
            txtTen.Clear();
            txtDTB.Clear();
            if (cmbKhoa.Items.Count > 0)
            {
                cmbKhoa.SelectedIndex = 0; // Chọn khoa mặc định là khoa đầu tiên
            }
            txtMSSV.Focus(); // Đặt con trỏ vào ô nhập mã sinh viên
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn muốn thoát chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Nếu người dùng chọn "No", hủy sự kiện đóng form
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn không
                if (dgvStudent.CurrentRow == null || dgvStudent.CurrentRow.Index == -1)
                {
                    throw new Exception("Vui lòng chọn một sinh viên để sửa.");
                }

                // Lấy chỉ số dòng được chọn
                int selectedRow = dgvStudent.CurrentRow.Index;

                // Xác nhận sửa
                DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn sửa thông tin sinh viên này?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    // Kiểm tra dữ liệu đầu vào
                    ValidateInput();

                    // Cập nhật dữ liệu trong DataGridView
                    InsertUpdate(selectedRow);

                    MessageBox.Show("Sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Cập nhật các thông tin khác nếu cần
                    // UpdateTotalStudents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn không
                if (dgvStudent.CurrentRow == null || dgvStudent.CurrentRow.Index == -1)
                {
                    throw new Exception("Vui lòng chọn một sinh viên để xoá.");
                }

                // Lấy chỉ số dòng được chọn
                int selectedRow = dgvStudent.CurrentRow.Index;

                // Hỏi xác nhận từ người dùng
                DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn xoá sinh viên này?", "Xác nhận xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    // Xoá dòng được chọn
                    dgvStudent.Rows.RemoveAt(selectedRow);
                    MessageBox.Show("Xoá sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset các ô nhập liệu sau khi xoá
                    ResetInputFields();

                    // Cập nhật tổng số sinh viên nếu có chức năng này
                    // UpdateTotalStudents();
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu xảy ra lỗi
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kiểm tra xem dòng được nhấn có hợp lệ không (bỏ qua tiêu đề)
                if (e.RowIndex >= 0 && e.RowIndex < dgvStudent.Rows.Count)
                {
                    // Lấy dòng được nhấn
                    DataGridViewRow selectedRow = dgvStudent.Rows[e.RowIndex];

                    // Gán giá trị từ dòng được nhấn vào các ô nhập liệu
                    txtMSSV.Text = selectedRow.Cells[0].Value?.ToString();
                    txtTen.Text = selectedRow.Cells[1].Value?.ToString();
                    cmbKhoa.Text = selectedRow.Cells[2].Value?.ToString();
                    txtDTB.Text = selectedRow.Cells[3].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
