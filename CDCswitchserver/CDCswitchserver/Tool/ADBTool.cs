
using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
/// <summary>
/// This tool is written by nguyen ba cu on 22/5/2019
/// The excute process folder default is /data/local/tmp
/// The capture screen be move to pc 
/// ADB task will take the fisrt device of list
/// Note that you must use adb version 1.0.41 or later
/// The first device is working device
/// </summary>
namespace ADBTool
{
    public class ADBTask : IDisposable
    {     
        /// <summary>
        /// Event and Delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        public delegate void DeviceChange(object sender, Device device);
        public delegate void OnProcessKill(string ProcessName);
        //delegate is type of function
        public event DeviceChange OndeivceConnect;
        public event DeviceChange OndeivceDisconect;
        public static string ERR_code = "ERR";
        private CancellationTokenSource cancellationTokenSource; 
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="WorkingDir"></param>
        #region Contructor
        public ADBTask(string Adbpath)
        {
            this.Filename = Adbpath;       
            cancellationTokenSource = new CancellationTokenSource();
            StartServer();
            ListenDevice(cancellationTokenSource.Token);        
        }
        ~ADBTask()
        {
            cancellationTokenSource.Cancel();
            //this.KillServer();
        }
        #endregion Contructor

        /// <summary>
        /// It run in orther thread 
        /// </summary>
        #region Broadcast
        private Device FDevice;
        private void ListenDevice(CancellationToken  token)
        {    
            new Thread(() =>
            {
                //check device
                List<Device>ProListDevice = null;

                while (!token.IsCancellationRequested)
                {
                    var ListDevice = GetDevices();
                    if (ListDevice.Count() == 0)
                    {
                        ExcuteWaitingHaveChange("wait-for-device");
                        ListDevice = GetDevices();

                        foreach (var item in ListDevice)
                        {
                            if (OndeivceConnect != null) OndeivceConnect(this, item);                            
                            new Thread(() =>
                            {
                                //wait for disconnect
                                var dv = ExcuteWaittingDisconnectDevice(item);
                                if (OndeivceConnect != null) OndeivceDisconect(this, item);
                            }).Start();
                        }
                        ProListDevice = GetDevices();
                    }                
                    else if(ListDevice.Count() >= 1)
                    {
                        if(ProListDevice == null)
                        {
                            foreach (var item in ListDevice)
                            {
                                if (OndeivceConnect != null) OndeivceConnect(this, item);
                                new Thread(() => 
                                {
                                    //wait for disconnect
                                    var dv = ExcuteWaittingDisconnectDevice(item);
                                    if (OndeivceDisconect != null) OndeivceDisconect(this, dv);
                                }).Start();
                            }
                            ProListDevice = GetDevices();
                        }
                        else
                        {
                            List<Device> olddv = new List<Device>();
                            //find old item in list
                            foreach (var itemi in ListDevice)
                            {
                                foreach (var itemj in ProListDevice)
                                {
                                    if(itemi.IPAddress == itemj.IPAddress)
                                    {
                                        olddv.Add(itemi);
                                    }
                                }
                            }
                      
                            foreach (var item in olddv)
                            {
                                ListDevice.Remove(item);//Clear old item from list just have new item
                            }

                            foreach (var item in ListDevice)
                            {
                                if (OndeivceConnect != null) OndeivceConnect(this, item);
                                new Thread(() => 
                                {
                                    //wait for new device disconnect
                                    var dv = ExcuteWaittingDisconnectDevice(item);
                                    if (OndeivceDisconect != null) OndeivceDisconect(this, dv);
                                }
                                ).Start();
                            }

                            //update old list device
                            ProListDevice = GetDevices();
                        }
                    }          
                }
            })
            { IsBackground = true }.Start();
        }


        #endregion Broadcast

        /// <summary>
        /// Function
        /// </summary>
        #region Function
        //server control
        public void StartServer()
        {
            ExecuteGiveResultImmediately("start-server");
        }
        public void KillServer()
        {
            ExecuteGiveResultImmediately("kill-server");
        }
        public void ReConnect()
        {
            ExecuteGiveResultImmediately("reconnect");
        }
        public void ReConnectDevice()
        {
            ExecuteGiveResultImmediately("reconnect device");
        }
        public void ReConnectOffline()
        {
            ExecuteGiveResultImmediately("reconnect offline");
        }

        public void Disconnect()
        {
            ExcuteWithDeviceTargetImmediately("disconnect");
        }
        //thread permision
        public void CanncelPermission()
        {
            cancellationTokenSource.Cancel();
        }
        //process and fileprocess folder is data/local/tmp
        public bool RunProcess(string ProcessName,bool IsOrtherThread = true)
        {
            if (FDevice == null) return false;
            //check process is run before
            bool IsrunBefore = CheckProcessRunning(ProcessName);
            if (IsrunBefore) KillProcess(ProcessName);
            //change mod
            string cmd = "shell chmod 777 /data/local/tmp/" + ProcessName;
            ExcuteWithDeviceTargetImmediately(cmd);
            //run tcclient in back ground
            cmd = "shell /data/local/tmp/" + ProcessName;
            if (IsOrtherThread)
            {
                new Thread(() =>
                {
                    //run the processs
                    ExcuteToRunApplication(cmd);
                })
                { IsBackground = true }.Start();
                Thread.Sleep(100);
            }
            else
            {
                //run the processs
                ExcuteToRunApplication(cmd);
            }
            return CheckProcessRunning(ProcessName);
        }

        public bool KillProcess(string ProcessName)
        {
            int ID = GetIdOfProcess(ProcessName);
            string cmd = "shell kill " + ID;
            ExcuteWithDeviceTargetImmediately(cmd);
            return GetIdOfProcess(ProcessName) == 0 ? true : false;
        }
        public bool CheckProcessRunning(string ProcessName)
        {
            return GetIdOfProcess(ProcessName) != 0 ? true : false; 
        }
        public string RunProcessGetResult(string ProcessName, string InputParameters)
        {
            if (FDevice == null) return "Device null";
            string cmd = "shell /data/local/tmp/" + ProcessName + " " + InputParameters;
            string outcode = ExcuteWithDeviceTargetImmediately(cmd);
            return outcode;
        }
        // -->Dirpro/(arm64|x86|...)/FileName
        
        public bool PushFileProcess(string DirPro, string FileName)
        {
            string PcFolder = DirPro + "\\" + GetChipType();
            return PushFile(PcFolder, "/data/local/tmp", FileName);
        }
        public int GetIdOfProcess(string ProcessName)
        {
            if (FDevice == null) return 0;
            string cmd ="shell pidof " + ProcessName;
            return intRegexID(ExcuteWithDeviceTargetImmediately(cmd));
        }
        private int intRegexID(string Output)
        {
            Regex re = new Regex(@"(?<id>\d+)");
            var match = re.Match(Output);
            int ID = match.Groups["id"].ToString() == "" ?
                0 : Int32.Parse(match.Groups["id"].ToString());
            return ID;
        }
        public bool RemoveFileProcess(string NameFileProcess)
        {            
            string file = "/data/local/tmp/"+NameFileProcess;            
            return RemoveFile(file);
        }
        public bool CheckFileProcessExist(string FileName)
        {
            string File = "/data/local/tmp/" + FileName;
            return CheckFileExist(File);
        }

        /// <summary>
        /// Commnad file and directory
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>		n	error CS0103: The name 'n' does not exist in the current context	

        public bool CheckFileExist(string File)
        {
            string Name = "FILE=\"" + File + "\";";
            string Code = "if [ -f \"$FILE\" ];then echo \"EXIST\"; else echo \"N\";fi ";
            string Shell = "shell " + Name + Code;
            return ExcuteWithDeviceTargetImmediately(Shell).Contains("EXIST") ? true : false;
        }
        public bool CheckFolderExist(string Dir)
        {
            string Name = "DIR=\""+Dir+"\";";
            string Code = "if [ -d \"$DIR\" ];then echo \"EXIST\"; else echo \"N\";fi ";
            string Shell = "shell " + Name + Code;
            return ExcuteWithDeviceTargetImmediately(Shell).Contains("EXIST") ? true : false;
        }
        public bool PushFile(string PcFolder, string DeviceFolder, string FileName)
        {
            string FileTo = PcFolder + "\\" + FileName;
            string cmd = "push " + FileTo + " " + DeviceFolder;
            // push file 
            ExcuteWithDeviceTargetImmediately(cmd);
            return CheckFileExist(DeviceFolder + "/" + FileName);
        }
        public bool PullFile(string PcFolder, string DeviceFolder, string FileName)
        {
            string FileTo = DeviceFolder + "/" + FileName;
            string cmd = "pull " + FileTo + " " + PcFolder;
            //pull file   
            return ExcuteWithDeviceTargetImmediately(cmd).Contains("file pulled");
        }
        public bool RemoveFile(string File)
        {
            ExcuteWithDeviceTargetImmediately("shell rm " + File);
            return !CheckFileExist(File);
        }
        
        /// <summary>
        /// Capture screen
        /// </summary>
        /// <param name="DeviceFolder"></param>
        /// <param name="PcFolder"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool ScreenCapture(string SdcardShareFolder,string PcFolder,string FileName)
        {
            if (!CheckFolderExist("/mnt/sdcard/" + SdcardShareFolder)) return false;
            string File = "/mnt/sdcard/"+ SdcardShareFolder + "/"+FileName;
            string cmd = "shell screencap -p " + File;
            //get capture screen
            ExcuteWithDeviceTargetImmediately(cmd);
            bool Iscap = CheckFileExist(File);            
            // pull file
            bool Ispull = PullFile(PcFolder, "/mnt/sdcard/"+ SdcardShareFolder, FileName);
            //
            bool IsRemove = RemoveFile(File);
            return Iscap && Ispull && IsRemove;
        }
   
        //Device exflore just use on selected device
        public string GetNameDevice()
        {
            string cmd = "shell getprop ro.product.model";
            return ExcuteWithDeviceTargetImmediately(cmd).Trim();
        }
        public string GetIPAddress()
        {
            return FDevice != null ? FDevice.IPAddress : ERR_code;
        }

        public DeviceState GetDeviceState()
        {
            return FDevice.DeviceState;
        }
        public bool IsDeviceNull()
        {
            return FDevice == null ? true : false;
        }
        public string GetChipType()
        {
            string cmd = "shell getprop ro.product.cpu.abi";
            string abicpu = ExcuteWithDeviceTargetImmediately(cmd);
            var delspace = String.Join("", abicpu.Where(c => !char.IsWhiteSpace(c)));
            return delspace;
        }

        public string GetSerial()
        {
            string cmd = "shell getprop ro.serialno";
            string abicpu = ExcuteWithDeviceTargetImmediately(cmd);
            var delspace = String.Join("", abicpu.Where(c => !char.IsWhiteSpace(c)));
            return delspace;
        }

        public SizeScreen GetSizeScreen()
        {
            string cmd = "shell wm size";
            string outcode = ExcuteWithDeviceTargetImmediately(cmd);
            try
            {
              return  RegexDataADB.MakeSizeDevice(outcode);
            }
            catch (Exception)
            {
                return new SizeScreen() { Height = 0,Width = 0 };
            }
        }

        public string GetApiLevelDevice() {
            return ExcuteWithDeviceTargetImmediately("shell getprop ro.build.version.sdk").Trim();
        }

        public string GetAndroidVersion()
        {
            return ExcuteWithDeviceTargetImmediately("shell getprop ro.build.version.release").Trim();
        }

        public string GeyBaterryLevel()
        {
            string outcode = ExcuteWithDeviceTargetImmediately("shell dumpsys battery");
            return RegexDataADB.MakeBatteryLevel(outcode);
        }

        public string GetManufacturer()
        {
            return ExcuteWithDeviceTargetImmediately("shell getprop ro.product.manufacturer").Trim();
        }
        //Port and socket
        public bool CheckPortExsit(int Port)
        {        
            //get list port
            var listport = GetListForwardTcpPort();
            //check exsit por
            var Okport = listport.Where(p => p.ClientPort == Port.ToString());
            if (Okport.Count() >= 1) return true;
            return false;
        }
        public bool CheckPortAbstractExsit(int Port,string Dm)
        {
            //get list port
            var listportab = GetListForwardAbstracts();
            //check exsit por
            var Okport = listportab.Where(p => (p.Port == Port.ToString()) &&(p.PsName == Dm));
            if (Okport.Count() >= 1) return true;
            return false;
        }
        public bool CheckPortAbstractExsit(int Port)
        {
            //get list port
            var listportab = GetListForwardAbstracts();
            //check exsit por
            var Okport = listportab.Where(p => (p.Port == Port.ToString()));
            if (Okport.Count() >= 1) return true;
            return false;
        }
       
        public List<Device> GetDevices()
        {
            String outcode = ExecuteGiveResultImmediately("devices");
            return  RegexDataADB.MakeListDevices(outcode).ToList();
        }

        public Device GetDeivce()
        {
            return FDevice;
        }
        public void  SetDevice(Device device)
        {
            FDevice = device;
        }
        public IEnumerable<ForwardAbstract> GetListForwardAbstracts()
        {
            string outcode = ExecuteGiveResultImmediately("forward --list");
            return RegexDataADB.MadeListforwardAbstracts(outcode);
        }

        public IEnumerable<ReverseTcp> GetListReverseTcpPort()
        {
            string outcode = ExcuteWithDeviceTargetImmediately("reverse --list");
            return RegexDataADB.MakeListreverseTcp(outcode);
        }

        public IEnumerable<ForwardTcp> GetListForwardTcpPort()
        {
            string outcode = ExecuteGiveResultImmediately("forward --list");
            return RegexDataADB.MadeListforwardTcp(outcode);
        }

        public void RemoveAllTcpPort()
        {
            ExecuteGiveResultImmediately("forward --remove-all");
        }
        public bool CloseTcp(int port)
        {
            ExecuteGiveResultImmediately("forward --remove tcp:"+port);
            return !CheckPortAbstractExsit(port) && !CheckPortExsit(port);
        }
        public bool CreateForwardlocalabstract(int port, string Domain)
        {
            ExcuteWithDeviceTargetImmediately("forward tcp:" + port + " localabstract:" + Domain);
            return CheckPortAbstractExsit(port, Domain);
        }
        public void CreateForwardTcp(int Fp,int Sp)
        {         
            ExcuteWithDeviceTargetImmediately("forward tcp:" + Fp + " tcp:" + Sp);          
        }
        public void CreateReverseTcp(int Fp,int Sp)
        {
            ExcuteWithDeviceTargetImmediately("reverse tcp:" + Fp + " tcp:" + Sp);
        }
        #endregion Function

        /// <summary>
        /// Excute
        /// </summary>
        #region Excute Command
        private  string Filename;
        public  static string LibsProcessPath;
        public string ExcuteWithDeviceTargetImmediately(string Command)
        {           
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Filename;
             process.StartInfo.Arguments = " -s " + FDevice.IPAddress + " " + Command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        }
        public string ExecuteGiveResultImmediately(string Command)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Filename;
            process.StartInfo.Arguments = Command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;        
            process.Start(); 
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd()+process.StandardError.ReadToEnd();     
        }        
        private TimeSpan ExcuteWaitingHaveChange(string Command)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Filename;
            process.StartInfo.Arguments = Command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
            return process.ExitTime - process.StartTime;
        }
        private Device ExcuteWaittingDisconnectDevice(Device device)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Filename;
            process.StartInfo.Arguments = " -s "+ device.IPAddress+ " wait-for-disconnect";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
            return device;
        }
        public void ExcuteToRunApplication(string Command)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = "/C \"" + Filename + " -s " + FDevice.IPAddress + " " + Command + "\"";
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
            catch (Exception)
            {

            }     
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            this.KillServer();
        }
        #endregion Excute Command
    }
    #region Type Class
    public class Device
    {
        public string IPAddress { get; set; }
        public DeviceState DeviceState { get; set; }
    }

    public class SizeScreen
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public enum DeviceState
    {
        OFFLINE,
        DEVICE,
        UNAUTHORIZED,
        BOOTLOADER
    }
    public class ForwardAbstract
    {
        private string serialdevices;
        string port;
        string psName;
        public string Serialdevices { get => serialdevices; set => serialdevices = value; }
        public string Port { get => port; set => port = value; }
        public string PsName { get => psName; set => psName = value; }
        public ForwardAbstract(string Serialdevices, string Port, string PsName)
        {
            this.Serialdevices = Serialdevices;
            this.Port = Port;
            this.PsName = PsName;
        }
        public ForwardAbstract() { }
    }
    public class ForwardTcp : FRTcp
    {
        
    }   
    public class ReverseTcp : FRTcp
    {

    }
    public class FRTcp
    {
        protected string serialDevice;
        protected string serverPort;
        protected string clientPort;
        public string SerialDevice { get => serialDevice; set => serialDevice = value; }
        public string ServerPort { get => serverPort; set => serverPort = value; }
        public string ClientPort { get => clientPort; set => clientPort = value; }
        public FRTcp() { }
        public FRTcp(string SerialDevice, string ServerPort, string ClientPort)
        {
            this.SerialDevice = SerialDevice;
            this.ServerPort = ServerPort;
            this.ClientPort = ClientPort;
        }
    }
    static public class RegexDataADB
    {
        static public IEnumerable<ForwardAbstract> MadeListforwardAbstracts(string OutCode)
        {        
            Regex re = new Regex(@"(?<serial>\S+) tcp:(?<port>\d+) localabstract:(?<psname>\w+)");
            foreach (Match item in re.Matches(OutCode))
            {
                ForwardAbstract forwardAbstract = new ForwardAbstract();
                forwardAbstract.Serialdevices = item.Groups["serial"].ToString();
                forwardAbstract.Port = item.Groups["port"].ToString();
                forwardAbstract.PsName = item.Groups["psname"].ToString();
                yield return forwardAbstract;
            }
        }
        static public IEnumerable<ForwardTcp> MadeListforwardTcp(string OutCode)
        {         
            Regex re = new Regex(@"(?<serial>\S+) tcp:(?<serverport>\d+) tcp:(?<clientport>\d+)");
            foreach (Match item in re.Matches(OutCode))
            {
                ForwardTcp forwardTcp = new ForwardTcp();
                forwardTcp.SerialDevice = item.Groups["serial"].ToString();
                forwardTcp.ServerPort = item.Groups["serverport"].ToString();
                forwardTcp.ClientPort = item.Groups["clientport"].ToString();
                yield return forwardTcp;
            }
        }

        static public IEnumerable<ReverseTcp> MakeListreverseTcp(string OutCode)
        {
            Regex re = new Regex(@"(?<serial>\S+) tcp:(?<serverport>\d+) tcp:(?<clientport>\d+)");
            foreach (Match item in re.Matches(OutCode))
            {
                ReverseTcp reverseTcp = new ReverseTcp();
                reverseTcp.SerialDevice = item.Groups["serial"].ToString();
                reverseTcp.ServerPort = item.Groups["serverport"].ToString();
                reverseTcp.ClientPort = item.Groups["clientport"].ToString();
                yield return reverseTcp;
            }
        }
        static public IEnumerable<Device> MakeListDevices(string OutCode)
        {
            //\t it mean tab char
            Regex re = new Regex(@"(?<ipaddress>\S+)\t(?<state>device|offline|bootloader)");
            foreach (Match item in re.Matches(OutCode))
            {
                Device device = new Device();
                device.IPAddress = item.Groups["ipaddress"].ToString();
                //parse
                DeviceState state;
                string status = item.Groups["state"].ToString().ToUpper();                
                Enum.TryParse<DeviceState>(status, out state);
                device.DeviceState = state;
                yield return device;
            }
        }  

        static public SizeScreen MakeSizeDevice(string Outcode)
        {
            Regex re = new Regex(@"(?<width>\d+)x(?<height>\d+)");
            var m = re.Match(Outcode);
            SizeScreen sizeScreen = new SizeScreen();
            sizeScreen.Width = Int32.Parse(m.Groups["width"].ToString());
            sizeScreen.Height = Int32.Parse(m.Groups["height"].ToString());
            return sizeScreen;
        }

        static public string MakeBatteryLevel(string Outcode)
        {
            Regex re = new Regex(@"level: (?<level>\d+)");
            var m = re.Match(Outcode);
            return m.Groups["level"].ToString();
        }
    }

    #endregion Type Class 


}
