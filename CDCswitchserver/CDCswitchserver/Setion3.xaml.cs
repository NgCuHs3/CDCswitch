using CDCswitchserver.interfaceUI;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CDCswitchserver
{
    /// <summary>
    /// Interaction logic for Setion3.xaml
    /// </summary>
    public partial class Setion3 : Page, Pageindex
    {
        public Setion3()
        {
            InitializeComponent();
        }

        public int getCurrentPageIndex()
        {
            return 3;
        }

        public void SetBootwindow(Bootwindow bootwindow)
        {
            bootwindow.ToRightButton.Visibility = Visibility.Hidden;
            bootwindow.ToLeftButton.Visibility = Visibility.Visible;
        }
    }
}
