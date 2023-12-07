using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            WindowStartupLocation= WindowStartupLocation.CenterScreen;
        }

        Truonghocdb2Context db = new Truonghocdb2Context();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var query = from sv in db.Sinhviens
                        join lh in db.Lophocs
                        on sv.Malop equals lh.Malop
                        where sv.Diem > 5 where sv.Malop == 3
                        select new
                        {
                            sv.Masv,
                            sv.Hoten,
                            sv.Diem,
                            lh.Malop,
                            lh.Tenlop
                        };
            dgWindow2.ItemsSource = query.ToList();
        }
    }
}
