using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDCswitchserver.net
{
    public  class Socketworker 
    {

        #region Valueable
        private Thread ConnectThread;
        private CancellationTokenSource CancelConnectThread;
        #endregion

        public void StopBegingConnect()
        {
            if(ConnectThread?.IsAlive == true)CancelConnectThread?.Cancel();
        }
        public void BegingConnect()
        {
            
            //token dùng cancel thread
            CancelConnectThread = new CancellationTokenSource();
           
           ConnectThread = new Thread(() =>
            {
                while (!CancelConnectThread.IsCancellationRequested)
                {
                    try
                    {
                        /* Khi mà kết nối khi server chưa được bật lên á thì nó sẽ kết nối vẫn được với adb do tính chất gì đó mà 
                         * mà tôi không biết nó sẽ đọc dữ liệu luỗn vấn được nhưng là các byte trống do vây nên cần có kiểm dư liệu có được 
                         * gửi từ server hay không nếu được thì cho là kết nối thành công nếu không là kết nối thất bại
                         * do vậy việt nhân được liệu cần bị trên hơn một tí so với việc gửi đi ở phía server 
                         */
                        Neter.Datatranfer = Connect(Neter.DATATRANFER_PORT);
                        Neter.Mouseuser = Connect(Neter.MOUSEUSER_PORT);
                        Thread.Sleep(10);
                        FirstTalk();
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }
                TalkTogether();
            });

            ConnectThread.Start();
        }

        public void Connectminicap()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Neter.CAPTURER_PORT);
            // tao ra client
            Neter.Capturer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // client connect den cong
            Neter.Capturer.Connect(iep);
        }

        
        private Socket Connect(int Port)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1",Port);
            return socket;
        }

        private void FirstTalk()
        {
            Talkcontent talkcontent;
            byte[] data = new byte[1024];
            int len = Neter.Datatranfer.Receive(data);
            talkcontent = JsonConvert.DeserializeObject<Talkcontent>(Encoding.UTF8.GetString(data,0,len));
            if (talkcontent == null) throw new Exception("Data is not valid");
            if(TalkCode.JUSTTALK == talkcontent.TalkCode)
            {
                if (Ontalkstart != null) Ontalkstart();
            }
        }

        private void TalkTogether()
        {
            try
            {
                while (!CancelConnectThread.IsCancellationRequested)
                {
                    var talkcon = ReadData();
                    /*
                     * Ở đây Pc sẽ kết nối tới phone có 2 trường hợp xảy ra
                     * một là phone gửi dữ liệu lên
                     * hai là pc yêu cầu phone trả dữ lên bằn code là request data
                     */                 
                    if(talkcon.TalkCode != TalkCode.JUSTTALK)
                    {
                        if (OnTakeData != null) OnTakeData(talkcon);
                    }
                }
            }
            catch (Exception e)
            {              
                if (Ontalkstop != null) Ontalkstop();
                
            }                
        }

      
        private Talkcontent ReadData()
        {
            Talkcontent Talkcontent = null;
            string json;
            byte[] data = new byte[1024];
            int len = Neter.Datatranfer.Receive(data);
            json = Encoding.UTF8.GetString(data,0,len);
             Talkcontent = JsonConvert.DeserializeObject<Talkcontent>(json);
            if (Talkcontent == null)
                throw new Exception("Data is not valid");
            return Talkcontent;
        }

        private void SendData(Talkcontent talkcontent)
        {
            string Json = JsonConvert.SerializeObject(talkcontent);
            byte[] data = ASCIIEncoding.ASCII.GetBytes(Json);
            Neter.Datatranfer.Send(data);
        }
        

        #region Event
        public delegate void Contenoftalk(Talkcontent talkcontent);
        public event Contenoftalk OnTakeData;

        public delegate void Talkserver();
        public  event Talkserver Ontalkstart;
        public event Talkserver Ontalkstop;
        #endregion
    }
}
