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
    /// Interaction logic for Lockscreen.xaml
    /// </summary>
    public partial class Lockscreen : Window
    {
        public Lockscreen()
        {
            InitializeComponent();
        }
        #region Event
        private void ToolCard_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var ps = e.GetPosition(WorkingWall);
                Gridtoolbar.Margin = new Thickness(ps.X- X_oldpiont.X,ps.Y - X_oldpiont.Y, 0, 0);
            }
        }
        private Point X_oldpiont;
        
        private void ToolCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            X_oldpiont = e.GetPosition(Gridtoolbar);
        }
        #endregion
    }
}
