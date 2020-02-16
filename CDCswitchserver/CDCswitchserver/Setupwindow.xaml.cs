using ADBTool;
using CDCswitchserver.interfaceUI;
using CDCswitchserver.minicap;
using CDCswitchserver.net;
using CDCswitchserver.switcher;
using CDCswitchserver.Tool;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;


namespace CDCswitchserver
{
    /// <summary>
    /// Interaction logic for setupwindow.xaml
    /// </summary>
    public partial class setupwindow : Window,Mainchildremote
    {
        public setupwindow()
        {

            InitializeComponent();
            //set o giua mang hinh
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SetUpValue();
        }
        #region Valuable
        private Mainwindow mainwindow;
        private ADBTask dBTask;
        private Socketworker socketworker;
        public static Button Selected;
        private static ControlShell Selectedshell;
        private double Old_x;
        private double Old_y;
        private string FileDatausercontrol = "datacontrol.xml";
        private string FileDataorigincontrol = "dataorigin.xml";
        private string Dataprogramspath = @"";
        private string FileSourceuser = "sourceuser.xml";
        private string Filesourceorigin = "sourceorigin.xml";
        public static ControlShellDataList listorigin;
        public ControlShellDataList listuser;
        public Sourceuser sourceuser;
        private Sourceuser sourceorigin;
        private Thread NotifyThread;
        private DecodeMinicap decodeMinicap;
        #endregion

        #region Parameter
    
        public void SetUpValue()
        {
            Readcontroldataformfile();
            ReadSourceuserfromfile();
            Selected = JOY_STICK;
            //Set bindding
            PoCard.DataContext = Selected;
        }
        private void SetCardMirrorHeight()
        {
            var sizescree = dBTask.GetSizeScreen();
            double heightcard = sizescree.Width * ((double)ControlsCard.Width / (double)sizescree.Height);
            ControlsCard.Height = heightcard;
        }

        private void SetupMirror()
        {
            new Thread(() =>
            {
                //forward port
                dBTask.CreateForwardlocalabstract(Neter.CAPTURER_PORT, Neter.minicap);
                // Runminicap 
                Adbminihelper adbminihelper = new Adbminihelper(dBTask);
                var sizescreen = dBTask.GetSizeScreen();
                adbminihelper.Runminicapfile(sizescreen.Height,sizescreen.Width);
                Thread.Sleep(1000);
                //Connect minicap server
                socketworker.Connectminicap();
                 //get frame
                decodeMinicap = new DecodeMinicap(Neter.Capturer);
                decodeMinicap.FrameBodyFPS += DecodeMinicap_FrameBodyFPS;
            }).Start();
        }

        private void SetUpAdbEvent()
        {
            socketworker.Ontalkstop += Socketworker_Ontalkstop;
        }

      
        public void InputMain(Mainwindow mainwindow)
        {
            this.mainwindow = mainwindow;
            this.dBTask = mainwindow.TADB;
            this.socketworker = mainwindow.socketworker;
            SetCardMirrorHeight();
            SetupMirror();
            SetUpAdbEvent();
        }
        #endregion

        #region Event
        private void Socketworker_Ontalkstop()
        {
            if (this.Visibility == Visibility.Visible)
            {
                Environment.Exit(0);
            }
        }



        private void DecodeMinicap_FrameBodyFPS(BitmapImage framebody)
        {
            this.Dispatcher.Invoke(() =>
            {
                Mirrorscreen.Source = framebody;
            });
        }
        private void Button_MouseDown_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            FunctionListView.ItemsSource = ListfunctionKey.GetListFunc();
        }

        private void BtImage_MouseMovePre(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var onbutton = sender as Button;
                var lc = e.GetPosition(MangagerControl_PS);
                lc.X -= Old_x;
                lc.Y -= Old_y;
                if ((lc.X) >= (ControlsCard.Width - onbutton.Width))
                {
                    lc.X = (ControlsCard.Width - onbutton.Width);
                }
                if ((lc.Y) >= (ControlsCard.Height - onbutton.Height))
                {
                    lc.Y = (ControlsCard.Height - onbutton.Height);
                }
                if ((lc.X) <= 0 && !(lc.Y <= 0))
                {
                    lc.X = 0;
                }
                if ((lc.Y) <= 0 && !(lc.X <= 0))
                {
                    lc.Y = 0;
                }
                if ((lc.X >= (ControlsCard.Width - onbutton.Width) && (lc.Y >= (ControlsCard.Height - onbutton.Height))) || ((lc.X) <= 0 && (lc.Y <= 0)))
                {
                    return;
                }
                onbutton.Margin = new Thickness(lc.X, lc.Y, 0, 0);
            }

        }
        private void BtImage_MouseDownPre(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Button onButton = sender as Button;
                var lcx = e.GetPosition(onButton);

                Old_x = lcx.X;
                Old_y = lcx.Y;
                if (Selected.Uid != onButton.Uid)
                {
                    if (IsmainControl(onButton))
                    {
                        Style NormalStyle = this.FindResource("ControlctChoose") as Style;
                        onButton.Style = NormalStyle;
                    }
                    else
               if (IsVehicleControl(onButton))
                    {
                        Style NormalStyle = this.FindResource("VehicleControlctChoose") as Style;
                        onButton.Style = NormalStyle;
                    }
                    else
               if (IsSwimControl(onButton))
                    {
                        Style NormalStyle = this.FindResource("SwimControlctChoose") as Style;
                        onButton.Style = NormalStyle;
                    }
                    if (IsmainControl(Selected))
                    {
                        Style NormalStyle = this.FindResource("Controlct") as Style;
                        Selected.Style = NormalStyle;
                    }
                    else
                    if (IsVehicleControl(Selected))
                    {
                        Style NormalStyle = this.FindResource("VehicleControlct") as Style;
                        Selected.Style = NormalStyle;
                    }
                    else
                    if (IsSwimControl(Selected))
                    {
                        Style NormalStyle = this.FindResource("SwimControlct") as Style;
                        Selected.Style = NormalStyle;
                    }
                    //gán lại cho selected
                    Selected = onButton;
                    //gắn resourcho pocard
                    PoCard.DataContext = Selected;
                    Selectedshell = FindControl(Selected.Name, listuser);
                    if (Selectedshell != null) ToogleKeyButton.DataContext = Selectedshell;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Writecontroldatatofile();
            Notify("Saved");
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Readcontroldataformfile();
            Notify("Cancel complete");
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ApplyDataControlto(listorigin);
            Writecontroldatatofile();
            Notify("Reset default ");
        }
        private void Choosepattdialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == true)
            {
                string folderPath = System.IO.Path.GetDirectoryName(folderBrowser.FileName);
                PATHSAVESNAPSHOOT.Text = folderPath;
            }
        }

        private void Buttonct_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var bt = sender as Button;
            bt.Width = bt.Height;
            bt.Margin = new Thickness(bt.Margin.Left + bt.Width > ControlsCard.Width ? ControlsCard.Width - bt.Width : bt.Margin.Left ,
                                      bt.Margin.Top + bt.Height > ControlsCard.Height ? ControlsCard.Height - bt.Height : bt.Margin.Top,
                                      0,0);
        }

        private void Upsize_Click(object sender, RoutedEventArgs e)
        {
            SizeSliderControlHeight.Value +=1;
        }

        public void Downsize_Click(object sender, RoutedEventArgs e)
        {
            SizeSliderControlHeight.Value -= 1;
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            
            Selected.Margin = new Thickness(Selected.Margin.Left, Selected.Margin.Top - 1, 0, 0);
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {

            Selected.Margin = new Thickness(Selected.Margin.Left, Selected.Margin.Top + 1, 0, 0);
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {

            Selected.Margin = new Thickness(Selected.Margin.Left-1, Selected.Margin.Top, 0, 0);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {

            Selected.Margin = new Thickness(Selected.Margin.Left+1, Selected.Margin.Top, 0, 0);
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            InfoSnackbar.IsActive = false;
        }

        private void StartGame_Button_Click(object sender, RoutedEventArgs e)
        {
            //close stream
            decodeMinicap.Dispose();
            socketworker.StopConnectminicap();
            //save control param
            WriteSourceusertofile();
            Notify("Saved");
            //
            mainwindow.lockscreen = new Lockscreen();
            mainwindow.lockscreen.Show();
            mainwindow.lockscreen.InputMain(mainwindow);
            this.Hide();
          
        }

        private void Saveuser_Click(object sender, RoutedEventArgs e)
        {
            WriteSourceusertofile();
            Notify("Saved");
        }

        private void Canceluser_Click(object sender, RoutedEventArgs e)
        {
            ReadSourceuserfromfile();
            Notify("Cancel");
        }

        private void Resetuser_Click(object sender, RoutedEventArgs e)
        {
            ApplySourceuser(sourceorigin);
            WriteSourceusertofile();
            Notify("Reset complete");
        }

        private async void Capturescreenclick_Click(object sender, RoutedEventArgs e)
        {
            
            Task<bool> saveta = new Task<bool>(() =>
            {
                string pahtsaveimage = "";
                Dispatcher.Invoke(() =>
                {
                 
                    if (PATHSAVESNAPSHOOT.Text == "Documents")
                    {
                        pahtsaveimage = @"C:\Users\" + Environment.UserName + @"\Documents\snapshot" + GetDatetimeTosave() + ".png";
                    }
                    else
                    {
                        pahtsaveimage = PATHSAVESNAPSHOOT.Text + @"\snapshoot" + GetDatetimeTosave() + ".png";                       
                    }
                    SaveBitmapimage(Mirrorscreen, pahtsaveimage);
                });
                
                return true;
            });
            saveta.Start();
            await saveta;
            Notify("Snapshoot successfull");
        }
        #endregion

        #region Method
        private bool IsVehicleControl(Button button)
        {
            int Id = Int32.Parse(button.Uid);
            if (Id == 13 || Id == 31 || Id == 32 || Id == 33 || Id == 34 || Id == 35 ||
                Id == 37 || Id == 38)
            {
                return true;
            }
            return false;
        }

        private bool IsSwimControl(Button button)
        {
            int Id = Int32.Parse(button.Uid);
            if (Id == 39 || Id == 40)
            {
                return true;
            }
            return false;
        }

        private bool IsmainControl(Button button)
        {
            return !IsVehicleControl(button) && !IsSwimControl(button);
        }

        private async void Writecontroldatatofile()
        {  
            Task<bool> writeta = new Task<bool>(() =>
            {
                ControlShellDataList controlShellDataList = new ControlShellDataList();
                string Fk = "None";
                string Bk = "None";
                this.Dispatcher.Invoke(() =>
                {
                    foreach (var item in MangagerControl_PS.Children)
                    {
                        try
                        {
                            var itemf = item as Button;
                            if (itemf == null) continue;
                            TypeControl type = (TypeControl)Enum.Parse(typeof(TypeControl), itemf.Uid, true);
                            switch (type)
                            {
                                case TypeControl.JOY_STICK: //1
                                    Fk = "W-S";
                                    Bk = "A-D";
                                    break;
                                case TypeControl.MOUSE_VIEW: //2
                                    Fk = "Mou";
                                    Bk = "Mou";
                                    break;
                                case TypeControl.FIRE_BUTTON_R: //3
                                    Fk = "Left";
                                    Bk = "Clik";
                                    break;
                                case TypeControl.TPPORFPP: //4
                                    Fk = "M";
                                    Bk = "Key";
                                    break;
                                case TypeControl.LAY_BUTTON: //5
                                    Fk = "X";
                                    Bk = "Key";
                                    break;
                                case TypeControl.SIT_BUTTON: //6
                                    Fk = "C";
                                    Bk = "Key";
                                    break;
                                case TypeControl.SCOPE_ON_OF: //7
                                    Fk = "Right";
                                    Bk = "Clik";
                                    break;
                                case TypeControl.LOAD_BULLET: //8
                                    Fk = "R";
                                    Bk = "Key";
                                    break;
                                case TypeControl.JUMP_BUTTON: //9
                                    Fk = "Spa";
                                    Bk = "Key";
                                    break;
                                case TypeControl.TILT_LEFT_BUTTON: //10
                                    Fk = "Q";
                                    Bk = "Key";
                                    break;
                                case TypeControl.TILT_RIGHT_BUTTON: //11
                                    Fk = "E";
                                    Bk = "Key";
                                    break;
                                case TypeControl.GUN_THIRD: //12
                                    Fk = "3";
                                    Bk = "Key";
                                    break;
                                case TypeControl.VEHICLECHANGESIT: //13
                                    Fk = "0";
                                    Bk = "Num";
                                    break;
                                case TypeControl.EXIT_VEHICLE: //14
                                    Fk = "F";
                                    Bk = "Key";
                                    break;
                                case TypeControl.DRIVE_VEHICLE: //15
                                    Fk = "F";
                                    Bk = "Key";
                                    break;
                                case TypeControl.GET_IN_VEHICLE: //16
                                    Fk = "G";
                                    Bk = "Key";
                                    break;
                                case TypeControl.OPEN_CLOSE_DOOR: //17
                                    Fk = "G";
                                    Bk = "Key";
                                    break;
                                case TypeControl.CANCEL_BOM: //18
                                    Fk = "T";
                                    Bk = "Key";
                                    break;
                                case TypeControl.REVICE: //19
                                    Fk = "Z";
                                    Bk = "Key";
                                    break;
                                case TypeControl.GUN_FIRST: //20
                                    Fk = "1";
                                    Bk = "Key";
                                    break;
                                case TypeControl.GUN_SECOND: //21
                                    Fk = "2";
                                    Bk = "Key";
                                    break;
                                case TypeControl.BOMB: //22
                                    Fk = "B";
                                    Bk = "Key";
                                    break;
                                case TypeControl.ADD_BLOOD: //23
                                    Fk = "4";
                                    Bk = "Key";
                                    break;
                                case TypeControl.OPEN_MAP: //24
                                    Fk = "M";
                                    Bk = "Key";
                                    break;
                                case TypeControl.OPEN_BAG: //25
                                    Fk = "Tab";
                                    Bk = "Key";
                                    break;
                                case TypeControl.SETTING: //26
                                    Fk = "Esc";
                                    Bk = "Key";
                                    break;
                                case TypeControl.TAKE: //27
                                    Fk = "J";
                                    Bk = "Key";
                                    break;
                                case TypeControl.MOUSE_EYE: //28
                                    Fk = "Mid";
                                    Bk = "Clik";
                                    break;
                                case TypeControl.QUICKCHAT: //29
                                    Fk = "4-6";
                                    Bk = "8-5";
                                    break;
                                case TypeControl.HOLD_BOX: //30
                                    Fk = "T";
                                    Bk = "Key";
                                    break;
                                case TypeControl.VEHICLEBOOSTER: //31
                                    Fk = "Up";
                                    Bk = "Key";
                                    break;
                                case TypeControl.VEHICLEBRAKE: //32
                                    Fk = "Down";
                                    Bk = "Key";
                                    break;
                                case TypeControl.VEHICLERIGHT: //33
                                    Fk = "Right";
                                    Bk = "Key";
                                    break;
                                case TypeControl.VEHICLELEFT: //34
                                    Fk = "Left";
                                    Bk = "Key";
                                    break;
                                case TypeControl.VEHICLELOOKOUT: //35
                                    Fk = "Ctrl";
                                    Bk = "Right";
                                    break;
                                case TypeControl.PARACHUTE: //36
                                    Fk = "L";
                                    Bk = "Key";
                                    break;
                                case TypeControl.VEHICLEUP: //37
                                    Fk = "Del";
                                    Bk = "key";
                                    break;
                                case TypeControl.VEHICLEDOWN: //38
                                    Fk = "Ent";
                                    Bk = "key";
                                    break;
                                case TypeControl.SWIMUP: //39
                                    Fk = "U";
                                    Bk = "Key";
                                    break;
                                case TypeControl.SWIMDOWN: //40
                                    Fk = "J";
                                    Bk = "Key";
                                    break;
                                default:
                                    break;
                            }
                            ControlShell controlShell = new ControlShell();
                            controlShell.Fontkey = Fk;
                            controlShell.Behindkey = Bk;
                            controlShell.Height = itemf.Height;
                            controlShell.Width = itemf.Width;
                            controlShell.X = itemf.Margin.Left;
                            controlShell.Y = itemf.Margin.Top;
                            controlShell.Namecontrol = itemf.Name;
                            controlShellDataList.ControlShellList.Add(controlShell);
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
                XmlSerializer serializer = new XmlSerializer(typeof(ControlShellDataList));
                FileStream fileStream = new FileStream(FileDatausercontrol, FileMode.Create);
                serializer.Serialize(fileStream, controlShellDataList);
                fileStream.Close();

                return true;
            });
            writeta.Start();
            await writeta;
        }

        private async void Readcontroldataformfile()
        {
            Task<bool> readta = new Task<bool>(() => 
            {
                //data user
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ControlShellDataList));
                FileStream fileStreamuser = new FileStream(FileDatausercontrol, FileMode.Open);
                listuser = (ControlShellDataList)xmlSerializer.Deserialize(fileStreamuser);
                fileStreamuser.Close();
                //data origin
                FileStream fileStreamorigin = new FileStream(FileDataorigincontrol, FileMode.Open);
                listorigin = (ControlShellDataList)xmlSerializer.Deserialize(fileStreamorigin);
                fileStreamorigin.Close();
                ApplyDataControlto(listuser);
                return true;
            });
            readta.Start();
            await readta;
            ToogleKeyButton.DataContext = FindControl(Selected.Name, listuser);
        }

        private void ApplyDataControlto(ControlShellDataList controlShellDataList)
        {
            foreach (var item in  controlShellDataList.ControlShellList)
            {
                string Name = item.Namecontrol;
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        Button itembdf = FindChild<Button>(MangagerControl_PS, Name);
                        itembdf.Height = item.Height;
                        itembdf.Width = item.Width;
                        itembdf.Margin = new Thickness(item.X, item.Y, 0, 0);
                    }
                    catch (Exception)
                    {
                    }
                });
            }
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
  where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        
        public static ControlShell FindControl(string NAME, ControlShellDataList datavia)
        {
            if (datavia != null)
            {
                var Listct = datavia.ControlShellList.Where(p => p.Namecontrol == NAME);
                if (Listct.Count() > 0) return Listct.First();
            }
            return null;
        }

        private void Notify(String Content)
        {
            Notify(Content, 2000);
        }

        private void Notify(String Content, int Time)
        {
            if (NotifyThread != null)
            {
                if (NotifyThread.IsAlive) NotifyThread.Abort();
            }
            this.Dispatcher.Invoke(() =>
            {
                if (InfoSnackbar.IsActive) InfoSnackbar.IsActive = false;
                InfoSnackbar.IsActive = true;
                InfoSnackbar_Content.Content = Content;
            });
            NotifyThread = new Thread(() =>
            {
                Thread.Sleep(Time); this.Dispatcher.Invoke(() =>
                {
                    InfoSnackbar.IsActive = false;
                });
            })
            { IsBackground = true };
            NotifyThread.Start();
        }

        public void StopFrameCapture()
        {
            //ngưng frame
            if (decodeMinicap != null) decodeMinicap.Dispose();
        }

        private async void WriteSourceusertofile()
        {
            Task<bool> writeta = new Task<bool>(() =>
            {
                Sourceuser sourceuser = new Sourceuser();
                Dispatcher.Invoke(() =>
                {
                    sourceuser.Allowmirrorscreen = (bool)ALLOWMIRRORSCREEN.IsChecked;
                    sourceuser.Opacityofmirrorscreen = (int)OPACITYOFMIRROR.Value;
                    sourceuser.Pathsavesnapshoot = PATHSAVESNAPSHOOT.Text;
                });
                FileStream fileStream = new FileStream(FileSourceuser, FileMode.Create);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Sourceuser));

                xmlSerializer.Serialize(fileStream, sourceuser);
                return true;
            });
            writeta.Start();
            await writeta;
        } 

        private async void ReadSourceuserfromfile()
        {
            Task<bool> readta = new Task<bool>(() =>
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Sourceuser));
                FileStream fileStreamuser = new FileStream(FileSourceuser, FileMode.Open);
                sourceuser = (Sourceuser)xmlSerializer.Deserialize(fileStreamuser);
                FileStream fileStreamorigin = new FileStream(Filesourceorigin, FileMode.Open);
                sourceorigin = (Sourceuser)xmlSerializer.Deserialize(fileStreamorigin);
                
                return true;
            });
            readta.Start();
            await readta;
            ApplySourceuser(sourceuser);
        }
        private void ApplySourceuser(Sourceuser sourceuser)
        {
            ALLOWMIRRORSCREEN.IsChecked = sourceuser.Allowmirrorscreen;
            OPACITYOFMIRROR.Value = sourceuser.Opacityofmirrorscreen;
            PATHSAVESNAPSHOOT.Text = sourceuser.Pathsavesnapshoot;
        }

        public  void SaveBitmapimage(Image qrImg, string filePath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)qrImg.Source));
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                encoder.Save(stream);
        }


        private string GetDatetimeTosave()
        {
            return DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second+"_"+DateTime.Now.Millisecond;
        }
        #endregion


    }
}
