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
using static ADBTool.ADBTask;

namespace CDCswitchserver
{
    /// <summary>
    /// Interaction logic for Setion1.xaml
    /// </summary>
    public partial class Setion1 : Page, Pageindex,Mainchildremote
    {
        public Setion1()
        {
            InitializeComponent();
        }

        public int getCurrentPageIndex()
        {
            return 1;
        }

        public void InputMain(Mainwindow mainwindow)
        {
            var bootwindow = mainwindow.bootwindow;

            bootwindow.ToRightButton.Visibility = Visibility.Hidden;
            bootwindow.ToLeftButton.Visibility = Visibility.Hidden;

            var listdv = mainwindow.TADB.GetDevices();
            if (listdv.Count() >= 1)
            {
                //select the working  device
                mainwindow.TADB.SetDevice(listdv[0]);

                bootwindow.NaviSetion2();
            }
            else
            {
                DeviceChange deviceChange = null;
                deviceChange = (e, s) =>
                {
                    mainwindow.TADB.SetDevice(s);
                    bootwindow.NaviSetion2();
                    mainwindow.TADB.OndeivceConnect -= deviceChange;
                };
                mainwindow.TADB.OndeivceConnect += deviceChange;
            }
        }

     
    }
}
