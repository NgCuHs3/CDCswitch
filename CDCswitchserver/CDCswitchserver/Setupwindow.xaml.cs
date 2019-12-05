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

namespace CDCswitchserver
{
    /// <summary>
    /// Interaction logic for setupwindow.xaml
    /// </summary>
    public partial class setupwindow : Window
    {
        public setupwindow()
        {
            InitializeComponent();
            //set o giua mang hinh
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        #region Event
        private void Button_MouseDown_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion
    }
}
