using ADBTool;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CDCswitchserver
{
    /// <summary>
    /// Interaction logic for Bootwindow.xaml
    /// </summary>
    public partial class Bootwindow : Window
    {
        public Bootwindow()
        {
            TADB = new ADBTask(@"C:\Users\fpc\AppData\Local\Android\sdk\platform-tools\adb.exe");
            //Adb should creat fisrt
            InitializeComponent();
            //Center srceen 
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //khoi dong server adb

            //mặc định là mở setion 1
            NaviSetion1();
            //cai disconnect event
            TADB.OndeivceDisconect += TADB_OndeivceDisconect;
        }
        #region Valueable
        public ADBTask TADB;
        #endregion
        /// <summary>
        /// Envent Off the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Event
        private void Button_MouseDown_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void MainFrame_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            var ta = new ThicknessAnimation();
            ta.Duration = TimeSpan.FromSeconds(0.3);
            ta.DecelerationRatio = 0.7;
            ta.To = new Thickness(0, 0, 0, 0);
            if (e.NavigationMode == NavigationMode.New)
            {
                ta.From = new Thickness(500, 0, 0, 0);
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                ta.From = new Thickness(0, 0, 500, 0);
            }
                (e.Content as Page).BeginAnimation(MarginProperty, ta);
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            int currentindex = ((Pageindex)Frameshow.Content).getCurrentPageIndex();
            if (currentindex == 1)
            {
                NaviIndexSetion(3);
            }
            else
            {
                NaviIndexSetion(currentindex - 1);
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            int currentindex = ((Pageindex)Frameshow.Content).getCurrentPageIndex();
            if (currentindex == 3)
            {
                NaviIndexSetion(1);
            }
            else
            {
                NaviIndexSetion(currentindex + 1);
            }
        }

        private void ToRightButton_Click(object sender, RoutedEventArgs e)
        {
            NaviSetion3();
        }

        private void ToLeftButton_Click(object sender, RoutedEventArgs e)
        {
            NaviSetion2();
        }
        private void TADB_OndeivceDisconect(object sender, Device device)
        {
            NaviSetion1();
        }
        #endregion
        /// <summary>
        /// Method to do thing
        /// </summary>
        #region Method
        public void NaviSetion1()
        {
            NaviIndexSetion(1);
        }

        public void NaviSetion2()
        {
            NaviIndexSetion(2);
        }

        public void NaviSetion3()
        {
            NaviIndexSetion(3);
        }
        private void NaviIndexSetion(int index)
        {

            this.Dispatcher.Invoke(() =>
            {
                if (Frameshow.Content != null)
                {
                    int currentindex = ((Pageindex)Frameshow.Content).getCurrentPageIndex();
                    if (currentindex == 1 && index == 2)
                    {
                        GotoPage("onetotwo", 2);
                    }
                    if (currentindex == 2 && index == 3)
                    {
                        GotoPage("twotothree", 3);
                    }
                    if (currentindex == 1 && index == 3)
                    {
                        GotoPage("onetotwo", 2);
                        GotoPage("twotothree", 3);
                    }
                    if (currentindex == 2 && index == 1)
                    {
                        GotoPage("twotoone", 1);
                    }
                    if (currentindex == 3 && index == 2)
                    {
                        GotoPage("threetotwo", 2);
                    }
                    if (currentindex == 3 && index == 1)
                    {
                        GotoPage("threetotwo", 2);
                        GotoPage("twotoone", 1);
                    }
                }
                else
                {
                    GotoPage("USEDEFAULTAMIN", index);
                }
            });
        }

        private void GotoPage(string KeyAmin, int index)
        {
            Page page = null;
            string DefaultAmin = null;
            switch (index)
            {
                case 1:
                    page = new Setion1();
                    DefaultAmin = null;
                    break;
                case 2:
                    page = new Setion2();
                    DefaultAmin = "onetotwo";
                    break;
                case 3:
                    page = new Setion3();
                    DefaultAmin = "onetothree";
                    break;
            }
            string Amin = KeyAmin == "USEDEFAULTAMIN" ? DefaultAmin : KeyAmin;
            if (Amin != null)
            {
                Storyboard sb = this.FindResource(Amin) as Storyboard;
                sb.Begin();
            }
            //navi
            Frameshow.Navigate(page);
            //
            ((Pageindex)page).SetBootwindow(this);
        }
        #endregion

    }
}
