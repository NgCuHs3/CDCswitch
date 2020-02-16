using CDCswitchserver.net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.switcher
{
    class Handremote
    {
        //************************************************************************************************************************************// 
        //************************************************************************************************************************************// 
        /// <summary>
        /// SUPPORT FUNTION
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>

        public const int FINGER_COUTS = 4;

        private static byte[] addByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 0);
            newArray[bArray.Length] = newByte;
            return newArray;
        }
        /// <summary>
        /// Push Byte
        /// </summary>
        /// <param name="codeArray"></param>
        /// <returns></returns>
        public static bool PushByte_Touch(byte[] codeArray)
        {
            if (Neter.tccclient != null)
            {
                try
                {
                    Neter.Toucher.Send(codeArray);
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
            return false;
        }

        public static bool PushByte_MousePlayer(byte[] codeArray)
        {
            if (Neter.Mouseuser != null)
            {
                try
                {
                    Neter.Mouseuser.Send(codeArray);
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Finger remote
        /// </summary>
        /// <param name="PX"></param>
        /// <param name="PY"></param>
        /// <param name="sender"></param>
        /// <param name="finger"></param>
        public static void PutFinger(double PX, double PY, object sender)
        {
            var CT = sender as PutControl;
            var finger = CT.finger;
            string Commandcode = "d " + finger.FingerID + " " + //id
                                       (int)PX + " " + // x
                                       (int)PY + " " + // y
                                       CT.Pressure + " " + //pressure
                                       CT.Major_Mt + " " + //mt major
                                       CT.Major_Width + " \nc"; // mt width
            byte[] Commandbyte = Encoding.ASCII.GetBytes(Commandcode);
            byte[] Commandbyte_Cursor = addByteToArray(Commandbyte, 10);
            //10 is mean a cursor
            finger.X = PX;
            finger.Y = PY;
            finger.State = StateFinger.DOWN;
            PushByte_Touch(Commandbyte_Cursor);
        }
        public static void UpFinger(PutControl putControl)
        {
            //reset data
            var finger = putControl.finger;
            finger.State = StateFinger.UP;
            finger.X = putControl.X;
            finger.Y = putControl.Y;
  
            string Commandcode = "u " + finger.FingerID + " \nc";
            byte[] Commandbyte = Encoding.ASCII.GetBytes(Commandcode);
            byte[] Commandbyte_Cursor = addByteToArray(Commandbyte, 10);
            PushByte_Touch(Commandbyte_Cursor);
        }

        public static void MoveFinger(double DestinationX, double DestinationY, object sender, bool HasStep, double _stepMove = 5)
        {
            try
            {
                //UnBoxing
                var CT = sender as PutControl;
                var cancellationToken = CT.tokenSource.Token;
                var finger = CT.finger;
                //DO work
                double deltaX = -CT.finger.X + DestinationX;
                double deltaY = -CT.finger.Y + DestinationY;
                //code    
                string SubitemCommandK = null;
                int _StepCount = (int)Math.Abs(Math.Sqrt(deltaX * deltaX + deltaY * deltaY) / _stepMove);
                if (HasStep)
                    for (int i = 1; i <= _StepCount - 1 && !cancellationToken.IsCancellationRequested; i++)
                    {
                        finger.X += (deltaX / _StepCount);
                        finger.Y += (deltaY / _StepCount);
                        string Commandcodeitem = "m " + finger.FingerID + " " + //id
                                                    (int)finger.X + " " + //x
                                                    (int)finger.Y + " " + //y
                                                    CT.Pressure + " " + //pressure
                                                    CT.Major_Mt + " " + // mt major
                                                    CT.Major_Width + " \nc\n";//mt width
                        SubitemCommandK += Commandcodeitem;
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                //the final step
                string Commandcode = "m " + finger.FingerID + " " + //id
                                            (int)DestinationX + " " + //x
                                            (int)DestinationY + " " + //y
                                            CT.Pressure + " " + //pressure
                                            CT.Major_Mt + " " + // mt major
                                            CT.Major_Width + " \nc";//mt width

                SubitemCommandK += Commandcode;
                //10 is mean a cursor
                finger.X = DestinationX;
                finger.Y = DestinationY;
                byte[] CommandbyteSubK = Encoding.ASCII.GetBytes(SubitemCommandK);
                byte[] Commandbyte_CursorSubK = addByteToArray(CommandbyteSubK, 10);
                PushByte_Touch(Commandbyte_CursorSubK);
                SubitemCommandK = null;
            }
            catch (OperationCanceledException)
            {

            }
        }

        public static void UpForcefinger(int FingerID)
        {
            string Commandcode = "u " + FingerID + " \nc";
            byte[] Commandbyte = Encoding.ASCII.GetBytes(Commandcode);
            byte[] Commandbyte_Cursor = addByteToArray(Commandbyte, 10);
            PushByte_Touch(Commandbyte_Cursor);
        }

        public static void UpForceAllFinger()
        {
            for (int i = 0; i < FINGER_COUTS; i++)
            {
                UpForcefinger(i);
            }
        }

    }


}
