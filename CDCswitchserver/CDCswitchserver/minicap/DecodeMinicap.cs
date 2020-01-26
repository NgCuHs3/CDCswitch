using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CDCswitchserver.minicap
{
    class DecodeMinicap : IDisposable
    {
        public delegate void OnFrameBody(BitmapImage framebody);
        public event OnFrameBody FrameBodyFPS;

        //it need a socket connect to server
        public DecodeMinicap(Socket socket)
        {
            banner = new Banner();
            IsBreakThread = false;
            GetSteamDataFromSocket(socket);
        }

        private bool IsBreakThread;
        private int readBannerBytes = 0;
        private int bannerLength = 2;
        private int readFrameBytes = 0;
        private int frameBodyLength = 0;
        private Banner banner;
        private const int BUFFERSIZE = 1024 * 1024;
        private byte[] frambody = null;
        private void GetFrameBody(byte[] mbyte, int lenght)
        {
            byte[] data = new byte[lenght];
            Array.Copy(mbyte, 0, data, 0, lenght);
            for (int cusor = 0; cusor < lenght;)
            {
                if (readBannerBytes < bannerLength)
                {
                    switch (readBannerBytes)
                    {
                        case 0:
                            // version
                            banner.Version = data[cusor];
                            break;
                        //lenght
                        case 1:
                            banner.Length = bannerLength = data[cusor];
                            break;
                        //pid
                        case 5:
                            banner.Pid += (int)((uint)(data[cusor] << ((readBannerBytes - 2) * 8)) >> 0);
                            break;
                        //read width
                        case 9:
                            banner.RealWidth += (int)((uint)(data[cusor] << ((readBannerBytes - 6) * 8)) >> 0);
                            break;
                        //read height
                        case 13:
                            banner.RealHeight += (int)((uint)(data[cusor] << ((readBannerBytes - 10) * 8)) >> 0);
                            break;
                        //virtual width
                        case 17:
                            banner.VirtualWidth += (int)((uint)(data[cusor] << ((readBannerBytes - 14) * 8)) >> 0);
                            break;
                        //virtual height
                        case 21:
                            banner.VirtualHeight += (int)((uint)(data[cusor] << ((readBannerBytes - 18) * 8)) >> 0);
                            break;
                        //orentation
                        case 22:
                            banner.Orientation += data[cusor] * 90;
                            break;
                        case 23:
                            banner.Quirks = data[cusor];
                            break;
                    }
                    cusor += 1;
                    readBannerBytes += 1;
                }
                else if (readFrameBytes < 4)
                {
                    frameBodyLength += (int)((uint)(data[cusor] << ((readFrameBytes) * 8)) >> 0);
                    cusor += 1;
                    readFrameBytes += 1;
                }
                else
                {
                    if (lenght - cusor >= frameBodyLength)
                    {
                        if (frambody == null)
                        {
                            //tao mot framboyd
                            frambody = new byte[frameBodyLength];
                            //chep du lieu vao frambody
                            Array.Copy(data, cusor, frambody, 0, frameBodyLength);
                        }
                        else
                        {
                            byte[] addarray = new byte[frameBodyLength];
                            //do du lieu tu data vao addarrry
                            Array.Copy(data, cusor, addarray, 0, frameBodyLength);
                            //Noi mang  2 va franebody
                            frambody = frambody.Concat(addarray);
                        }

                        if (FrameBodyFPS != null)
                        {
                            BitmapImage bitmapImage = ToImage(frambody);
                            bitmapImage.Freeze();
                            FrameBodyFPS(bitmapImage);
                        }
                        cusor += frameBodyLength;
                        frameBodyLength = readFrameBytes = 0;
                        frambody = null;
                    }
                    else
                    {
                        byte[] addarray = new byte[lenght - cusor];
                        //do du lieu tu data vao add arrry
                        Array.Copy(data, cusor, addarray, 0, lenght - cusor);
                        //tao mot mang moi de mo rong kich cua mang framebody
                        if (frambody != null)
                        {
                            frambody = frambody.Concat(addarray);
                        }
                        else
                        {  //new chi co add arryay thoi
                            frambody = addarray;
                        }

                        //
                        frameBodyLength -= lenght - cusor;
                        readFrameBytes += lenght - cusor;
                        cusor = lenght;
                    }
                }
            }
        }

        private void GetSteamDataFromSocket(Socket socket)
        {
            Debug.WriteLine("Start stream video");
            new Thread(() =>
            {
                while (!IsBreakThread)
                {
                    try
                    {
                        byte[] data = new byte[BUFFERSIZE];
                        int lenghtdata = socket.Receive(data);
                        GetFrameBody(data, lenghtdata);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Sorry get data stream have problem !");
                    }
                }
            })
            { IsBackground = true }.Start();
        }
        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                BitmapImage image;
                try
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // here
                    image.StreamSource = ms;
                    image.EndInit();
                }
                catch (Exception)
                {
                    return null;

                }
                return image;
            }
        }

        public void Dispose()
        {
            IsBreakThread = true;
        }
    }
    public class Banner
    {
        public int Version;
        public int Length;
        public int Pid;
        public int RealWidth;
        public int RealHeight;
        public int VirtualWidth;
        public int VirtualHeight;
        public int Orientation;
        public int Quirks;
    }
    public static class ArrayEx
    {
        public static T[] Concat<T>(this T[] x, T[] y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");
            int oldLen = x.Length;
            Array.Resize<T>(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);
            return x;
        }
    }
}
