using ADBTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDCswitchserver.Tool
{
    class Adbminihelper
    {
        #region Valueable
        private ADBTask taskadb;
        #endregion

        public Adbminihelper(ADBTask taskADB)
        {
            this.taskadb = taskADB;
        }
        #region mothod
        public void Runminicapfile(int Width, int Height)
        {
            //kiểm tra có tiến trình nào đang chạy hay không
            if (taskadb.CheckProcessRunning("minicap"))
            {
                taskadb.KillProcess("minicap");
            }
            //đẩy hai file là minicap và thư viên minicap.so
            taskadb.PushFileProcess(@"cdclib\minicap", "minicap");
            string Apilevel = taskadb.GetApiLevelDevice();
            taskadb.PushFileProcess(@"cdclib\minicap\android-" + Apilevel, "minicap.so");
            //đổi quyền truy câp của minicap
            taskadb.ExcuteWithDeviceTargetImmediately("shell chmod 777 /data/local/tmp/minicap");
            //chạy file minicap
            string startcode = "shell LD_LIBRARY_PATH=/data/local/tmp /data/local/tmp/minicap -P " + Width + "x" + Height + "@" + Width + "x" + Height + "/0";
            new Thread(() =>
            {
                taskadb.ExcuteToRunApplication(startcode);
            }).Start();

        }

        public void Runtccclientfile()
        {
            taskadb.PushFileProcess(@"cdclib\tccclient", "tccclient");
            taskadb.RunProcess("tccclient", true);
        }
        #endregion
    }
}
