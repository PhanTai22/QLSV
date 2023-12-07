using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Truonghocdb2Context db = new Truonghocdb2Context();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            XoaThongBaoLoi();
            HienthiDataGrid();
            HienthiComboBoxLophoc();
        }
        private void HienthiDataGrid()
        {
            var query = from sv in db.Sinhviens select sv;
            dgDanhsach.ItemsSource = query.ToList();
        }

        private void HienthiComboBoxLophoc()
        {
            var query = from lophoc in db.Lophocs select lophoc;
            cboLophoc.ItemsSource = query.ToList();
            cboLophoc.DisplayMemberPath = "Tenlop";
            cboLophoc.SelectedValuePath = "Malop";
            cboLophoc.SelectedIndex = 0;
        }

        private void XoaThongBaoLoi()
        {
            errDiachi.Content = "";
            errDiem.Content = "";
            errHoten.Content = "";
            errlophoc.Content = "";
            errMasv.Content = "";
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private bool IsValidInput()
        {
            bool isValid = true;
            string masv = txtMasv.Text;
            string hoten = txtHoten.Text;
            string diem = txtDiem.Text;
            if (!Regex.IsMatch(masv, "^\\d{1,}$"))
            {
                isValid = false;
                errMasv.Content = "Bạn phải nhập mã sinh viên là số";
            }
            else
                errMasv.Content = "";
            if (!Regex.IsMatch(hoten, "^[\\p{L} .'-]+$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled))
            {
                isValid = false;
                errHoten.Content = "Bạn phải nhập họ tên hợp lệ";
            }
            else
                errHoten.Content = "";
            if (!Regex.IsMatch(diem, "^\\d{1,}$"))
            {
                isValid = false;
                errDiem.Content = "Bạn phải nhập điểm là số";
            }
            else
            {
                errDiem.Content = "";
                int d = int.Parse(diem);
                if (d < 0 || d > 10)
                {
                    isValid = false;
                    errDiem.Content = "Điểm phải trong khoảng 1-10";
                }
                else
                {
                    errDiem.Content = "";
                }
            }
            return isValid;
        }


        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = IsValidInput();
            if(isValid)
            {
                try
                {
                    Sinhvien s = new Sinhvien
                    {
                        Masv = int.Parse(txtMasv.Text),
                        Hoten = txtHoten.Text,
                        Diachi = cboDiachi.Text,
                        Diem = byte.Parse(txtDiem.Text),
                        Malop = int.Parse(cboLophoc.SelectedValue.ToString())
                    };

                    db.Sinhviens.Add(s);
                    db.SaveChanges();
                    HienthiDataGrid();
                    XoaDulieu();
                }
                catch (Exception x)
                {
                    MessageBox.Show("Có lỗi khi thêm" + x.Message);
                    db.ChangeTracker.Clear();
                }
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var svXoa = db.Sinhviens.SingleOrDefault(sv=>sv.Masv == int.Parse(txtMasv.Text));
                db.Sinhviens.Remove(svXoa);
                db.SaveChanges();
                HienthiDataGrid();
                XoaDulieu();
            }
            catch (Exception x)
            {
                MessageBox.Show("Có lỗi khi xóa" + x.Message);
                db.ChangeTracker.Clear();
            }
            
        }

        private void XoaDulieu()
        {
            txtMasv.Clear();
            txtHoten.Clear();
            txtDiem.Clear();
            cboDiachi.SelectedIndex = 0;
            cboLophoc.SelectedIndex = 0;
            txtMasv.Focus();
        }

        private void Button_XoaDongClick(object sender, RoutedEventArgs e)
        {
            object obj = dgDanhsach.SelectedItem;
            if (obj != null)
            {
                try
                {
                    Type t = obj.GetType();
                    PropertyInfo[] p = t.GetProperties();
                    int masv = int.Parse(p[0].GetValue(obj).ToString());
                    MessageBoxResult mbr = MessageBox.Show("Bạn có chắc muốn xóa không?",
                        "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbr == MessageBoxResult.Yes)
                    {
                        var svXoa = db.Sinhviens.SingleOrDefault(sv => sv.Masv == masv);
                        if (svXoa != null)
                        {
                            db.Sinhviens.Remove(svXoa);
                            db.SaveChanges();
                            HienthiDataGrid();
                            XoaDulieu();
                        }
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + x.Message);
                }
            }
            else
            {
                MessageBox.Show("Bạn phải chọn 1 hàng trong bảng");
            }
        }


        private void SelectedChanged_Click(object sender, SelectedCellsChangedEventArgs e)
        {
            object obj = dgDanhsach.SelectedItem;
            if (obj != null)
            {
                try
                {
                    Type t = dgDanhsach.SelectedItem.GetType();
                    PropertyInfo[] p = t.GetProperties();
                    txtMasv.Text = p[0].GetValue(dgDanhsach.SelectedItem).ToString();
                    txtHoten.Text = p[1].GetValue(dgDanhsach.SelectedItem).ToString();
                    cboDiachi.SelectedItem = p[2].GetValue(dgDanhsach.SelectedItem).ToString().ToLower();
                    
                    txtDiem.Text = p[3].GetValue(dgDanhsach.SelectedItem).ToString();
                    cboLophoc.SelectedValue = p[4].GetValue(dgDanhsach.SelectedItem).ToString();
                }
                catch (Exception x)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + x.Message);
                }
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var svSua = db.Sinhviens.SingleOrDefault(sv => sv.Masv == int.Parse(txtMasv.Text));
                if (svSua != null)
                {
                    if(IsValidInput())
                    {
                        svSua.Hoten  = txtHoten.Text;
                        svSua.Diachi = cboDiachi.Text;
                        svSua.Diem = Byte.Parse(txtDiem.Text);
                        svSua.Malop = int.Parse(cboLophoc.SelectedValue.ToString());

                        db.SaveChanges();
                        HienthiDataGrid();
                    }    
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Có lỗi khi sửa: " + x.Message);
            }
        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            Window2 obj = new Window2();
            obj.ShowDialog();
        }
    }
}