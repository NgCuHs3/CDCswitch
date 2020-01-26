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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CDCswitchserver
{
    /// <summary>
    /// Interaction logic for Setion2.xaml
    /// </summary>
    public partial class Setion2 : Page, Pageindex,Mainchildremotecs
    {
        public Setion2()
        {
            InitializeComponent();
        }
        #region Method
        private void setinfodevice()
        {
            try
            {
                var adb = mainwindow.TADB;
                Modeldv.Text = "Model " + adb.GetNameDevice();
                Heightdv.Text = "Height " + adb.GetSizeScreen().Height;
                Widthdv.Text = "Width " + adb.GetSizeScreen().Width;
                Statusdv.Text = "Status " + adb.GetDeviceState();
                Cpudv.Text = "CPU " + adb.GetChipType();
                Apidv.Text = "Api " + adb.GetApiLevelDevice();
                Androiddv.Text = "Android " + adb.GetAndroidVersion();
                Baterrydv.Text = "Baterry level " + adb.GeyBaterryLevel() + "%";
                Manufacturerdv.Text = "Manufacturer " + adb.GetManufacturer();
                Serialdv.Text = "Serial " + adb.GetSerial();
            }
            catch (Exception)
            {

            }       
        }
        #endregion
        //implement
        List<Device> list;
        Bootwindow bootWindow;
        Mainwindow mainwindow;
        public void InputMain(Mainwindow mainwindow)
        {
            this.mainwindow = mainwindow;
            this.bootWindow = mainwindow.bootwindow;
            list = mainwindow.TADB.GetDevices();
            listdevice.ItemsSource = list;
            listdevice.DisplayMemberPath = "IPAddress";
            int indexdevice = list.IndexOf(list.Find(s => { return s.IPAddress.Contains(mainwindow.TADB.GetDeivce().IPAddress); }));
            listdevice.SelectedIndex = indexdevice;
            bootWindow.ToRightButton.Visibility = Visibility.Visible;
            bootWindow.ToLeftButton.Visibility = Visibility.Hidden;
        }
        public int getCurrentPageIndex()
        {
            return 2;
        }

        #region Event
        private void Listdevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selentdevice = (sender as ComboBox).SelectedIndex;
            mainwindow.TADB.SetDevice(list[selentdevice]);
            setinfodevice();
        }

       
        #endregion
    }
}
