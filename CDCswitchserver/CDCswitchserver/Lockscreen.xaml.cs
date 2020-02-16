using CDCswitchserver.interfaceUI;
using CDCswitchserver.Keybroadservice.GlobalLowLevelHooks;
using CDCswitchserver.Keybroadservice.WindowsAccessibilityKeys;
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
    public partial class Lockscreen : Window, Mainchildremote
    {
        public Lockscreen()
        {
            InitializeComponent();
            DowaitJob();
        }
        #region Valueable
        private KeyboardHook keyboard;
        private SolidColorBrush bluecolor = new SolidColorBrush(Color.FromRgb(33, 140, 243));
        private SolidColorBrush yellowcolor = new SolidColorBrush(Color.FromRgb(255, 234, 0));
        private SolidColorBrush orangecolor = new SolidColorBrush(Color.FromRgb(255, 87, 34));
        #endregion

        #region Parameter
        public void InputMain(Mainwindow mainwindow)
        {
            DowaitJob();
        }
        #endregion

        #region Method

        private void DowaitJob()
        {
            keyboard = new KeyboardHook();
            keyboard.Install();
            keyboard.KeyDown += Keyboard_KeyDown;
            keyboard.KeyUp += Keyboard_KeyUp;



            //Done waitjob

            WaitingWall.Visibility = Visibility.Hidden;
            WorkingWall.Visibility = Visibility.Visible;
             
            //Turn off wAccessibilityShortcutKeys
            WindowsHelperAccessibilityKeys.AllowAccessibilityShortcutKeys(false);
            //set Appcet key for orther hook for input bindding\\\
            keyboard.SetAllowkey(Acpectkey);
        }

        private void ChangecolorButton(VKeys key,bool iskeyDown)
        {
            if (iskeyDown)
            {
                switch (key)
                {
                    case VKeys.KEY_W:
                        W_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_S:
                        S_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_A:
                        A_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_D:
                        D_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_Q:
                        Q_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_E:
                        E_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_Z:
                        Z_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_X:
                        X_key.Background = yellowcolor;
                        break;
                    case VKeys.KEY_C:
                        C_key.Background = yellowcolor;
                        C_O_key.Background = yellowcolor;
                        break;
                    case VKeys.LCONTROL:
                        Ctrl_O_key.Background = yellowcolor;
                        break;
                    case VKeys.RCONTROL:
                        Ctrl_O_key.Background = yellowcolor;
                        break;
                    case VKeys.LSHIFT:
                        Shift_O_key.Background = yellowcolor;
                        break;
                    case VKeys.RSHIFT:
                        Shift_O_key.Background = yellowcolor;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case VKeys.KEY_W:
                        W_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_S:
                        S_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_A:
                        A_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_D:
                        D_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_Q:
                        Q_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_E:
                        E_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_Z:
                        Z_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_X:
                        X_key.Background = bluecolor;
                        break;
                    case VKeys.KEY_C:
                        C_key.Background = bluecolor;
                        C_O_key.Background = orangecolor;
                        break;
                    case VKeys.LCONTROL:
                        Ctrl_O_key.Background = orangecolor;
                        break;
                    case VKeys.RCONTROL:
                        Ctrl_O_key.Background = orangecolor;
                        break;
                    case VKeys.LSHIFT:
                        Shift_O_key.Background = orangecolor;
                        break;
                    case VKeys.RSHIFT:
                        Shift_O_key.Background = orangecolor;
                        break;
                    default:
                        break;
                }
            }
        }
        private bool Acpectkey(VKeys key)
        {
            switch (key)
            {
                case VKeys.LCONTROL:
                    return true;
                case VKeys.RCONTROL:
                    return true;
                case VKeys.LSHIFT:
                    return true;
                case VKeys.RSHIFT:
                    return true;
                case VKeys.KEY_C:
                    return true;
                case VKeys.LWIN:
                    return true;
                case VKeys.RWIN:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

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


        private void Keyboard_KeyUp(VKeys key)
        {
            ChangecolorButton(key, false);
        }

        private void Keyboard_KeyDown(VKeys key)
        {
            ChangecolorButton(key, true);
        }
        #endregion

        #region Cmd Excuted

        private void QuitCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        #endregion




    }
}
