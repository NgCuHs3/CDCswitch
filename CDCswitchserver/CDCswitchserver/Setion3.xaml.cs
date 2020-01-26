using CDCswitchserver.interfaceUI;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CDCswitchserver
{
    /// <summary>
    /// Interaction logic for Setion3.xaml
    /// </summary>
    public partial class Setion3 : Page, Pageindex,Mainchildremotecs
    {
        ~Setion3()
        {

        }
        public Setion3()
        {
            InitializeComponent();
        }
        #region Value 
        private Bootwindow bootwindow;
        private Mainwindow mainwindow;
        #endregion
        public int getCurrentPageIndex()
        {
            return 3;
        }

        public void InputMain(Mainwindow mainwindow)
        {
            this.mainwindow = mainwindow;
            this.bootwindow = mainwindow.bootwindow;
            bootwindow.ToRightButton.Visibility = Visibility.Hidden;
            bootwindow.ToLeftButton.Visibility = Visibility.Hidden;
            var tadb = mainwindow.TADB;
            //tạo port reverser cho kết nối
            tadb.CreateForwardTcp(Neter.DATATRANFER_PORT, Neter.DATATRANFER_PORT);
            tadb.CreateForwardTcp(Neter.MOUSEUSER_PORT, Neter.MOUSEUSER_PORT);
            //thực hiện nghe ở cổng
            Bindingconnect();
            mainwindow.socketworker.Ontalkstart += Socketworker_Ontalkstart;
            mainwindow.socketworker.Ontalkstop += Socketworker_Ontalkstop;
            mainwindow.socketworker.OnTakeData += Socketworker_OnTakeData;
        }

        #region Event
        private void Socketworker_OnTakeData(Talkcontent talkcontent)
        {
            if (talkcontent.TalkCode == TalkCode.STARTEDGAME)
            {
                this.Dispatcher.Invoke(() =>
                {
                    mainwindow.setupwindow = new setupwindow();
                    mainwindow.setupwindow.Show();
                    mainwindow.setupwindow.InputMain(mainwindow);
                    bootwindow.Hide();
                });
            }
        }
        private void Socketworker_Ontalkstart()
        {
            this.Dispatcher.Invoke(() =>
            {
                statuspic.Source = new BitmapImage(new Uri(@"/image/connected.png", UriKind.Relative));
                statustex.Text = "Start your game now :))";
            });
        }
        private void Socketworker_Ontalkstop()
        {
            this.Dispatcher.Invoke(() =>
            {
                if(bootwindow.Visibility == Visibility.Visible)
                {
                    Bindingconnect();
                    statuspic.Source = new BitmapImage(new Uri(@"/image/appicon.png", UriKind.Relative));
                    statustex.Text = "Oops! try start app agian :((";
                }
            });
        }
        #endregion

        #region Method
        private void Bindingconnect()
        {
            mainwindow.socketworker.BegingConnect();
        }

       
        #endregion
    }
}
