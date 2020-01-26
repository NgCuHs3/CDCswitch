using ADBTool;
using CDCswitchserver.net;
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
    /// Interaction logic for Mainwindow.xaml
    /// </summary>
    public partial class Mainwindow : Window
    {
        public Mainwindow()
        {
            InitializeComponent();
            //Center srceen 
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        #region Valueable
        public ADBTask TADB;
        public Socketworker socketworker;
        public setupwindow setupwindow;
        public Bootwindow bootwindow;
        #endregion

        #region Event
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //create main tool]
            //adb
            TADB = new ADBTask(@"C:\Users\fpc\AppData\Local\Android\sdk\platform-tools\adb.exe");
            //socket worket
            socketworker = new Socketworker();
            //setupwindow
            setupwindow = new setupwindow();
            //boot window
            bootwindow = new Bootwindow();

            Runbootwindow();
        }
        private void Runbootwindow()
        {
            bootwindow.InputMain(this);
            bootwindow.Show();
            //ân của sổ chính
            this.Hide();
        }
        #endregion

        #region Mothod

        #endregion
    }
}
