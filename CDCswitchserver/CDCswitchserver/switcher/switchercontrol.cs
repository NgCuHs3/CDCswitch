using CDCswitchserver.Keybroadservice.GlobalLowLevelHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDCswitchserver.switcher
{
    class PutControl
    { 
        public int Heigh;
        public int Width;
        public int X;
        public int Y;
        public int Pressure;
        public int  Major_Mt;
        public int Major_Width;
        public Finger finger;
        protected Switcher switcher;
        public CancellationTokenSource tokenSource;
        public virtual void Work_Key(VKeys key,bool IsDown,Switcher switcher) { }

        public virtual void Work_Mouse_Move(double Dx,double Dy,Switcher switcher) { }

        public virtual bool Isworkingkey(VKeys key, bool Isdown) { return false; }
    }



    class Joystick : PutControl
    {
        protected Keystroke Up = new Keystroke() { Isđown = false};
        protected Keystroke Down = new Keystroke() { Isđown = false };
        protected Keystroke Left = new Keystroke() { Isđown = false };
        protected Keystroke Right = new Keystroke() { Isđown = false };
        

        private const int corner = 45;
        public Joystick(VKeys Up,VKeys Down,VKeys Left,VKeys Right,int FingerId)
        {
            this.Up.keyValue = Up;
            this.Down.keyValue = Down;
            this.Left.keyValue = Left;
            this.Right.keyValue = Right;
            this.finger = new Finger() { FingerID = FingerId, State = StateFinger.UP };
        }
        public override void Work_Key(VKeys key, bool IsDown, Switcher switcher)
        {
            if (Isworkingkey(key, IsDown))
            {
                //select mode of sreeen to calculatim
                int R_x = 1;
                int R_y = 1;
                int R_Df = 1;
                int R_Ds = 1;
                switch (switcher.Currentinformation.ScreenOrentation)
                {
                    case ScreenOrentation.LEFT_HORIZONTAL: R_x = 1; R_y = 1; R_Ds = 1; R_Df = 0; break;
                    case ScreenOrentation.RIGHT_HORIZONTAL: R_x = -1; R_y = -1; R_Ds = 1; R_Df = 0; break;
                    case ScreenOrentation.MIDDLE_VERTICAL: R_x = 1; R_y = -1; R_Ds = 0; R_Df = 1; break;
                }
                //calular postion
                double DX = 0, DY = 0;
                double Lta = Math.Sin((corner * Math.PI) / 180) * 0.5 * this.Width;
                //sử lý các sự kiên khi key down hoạt key up
                if(finger.State == StateFinger.UP)
                {      
                    // sau khi put finger thì statet finger sẽ chuyển sang down
                        Handremote.PutFinger(this.X, this.Y, this);
                }
                else
                {
                    if(!Up.Isđown && !Down.Isđown && !Left.Isđown && !Right.Isđown)
                    {
                        Handremote.UpFinger(this);
                    }
                }
                //tất cả các tổ hợp phím sẽ xử lí

                //xử lí các trượng hơn key đơn
                if (key == Up.keyValue)
                {
                    DX = this.X + this.Width / 2 * R_Ds * R_y;
                    DY = this.Y - this.Width / 2 * R_Df * R_x;
                    goto DoublekeyStrkey;
                }
                if (key == Down.keyValue)
                {
                    DX = this.X - this.Width / 2 * R_Ds * R_y;
                    DY = this.Y + this.Width / 2 * R_Df * R_x;
                    goto DoublekeyStrkey;
                }
                if (key == Left.keyValue)
                {
                    DX = this.X - this.Width / 2 * R_Df * R_x;
                    DY = this.Y - this.Width / 2 * R_Ds * R_y;
                    goto DoublekeyStrkey;
                }
                if (key == Right.keyValue)
                {
                    DX = this.X + this.Width / 2 * R_Df * R_x;
                    DY = this.Y + this.Width / 2 * R_Ds * R_y;
                    goto DoublekeyStrkey;
                }
                //các trường hợp key đôi
                DoublekeyStrkey:
                if(Up.Isđown && Left.Isđown)
                {
                    DX = this.X + Lta * R_y;
                    DY = this.Y - Lta * R_x;
                    if (key == Up.keyValue || key == Left.keyValue) goto PutStrokeKey;
                }
                if(Up.Isđown && Right.Isđown)
                {
                    DX = this.X + Lta * R_x;
                    DY = this.Y + Lta * R_y;
                    if (key == Up.keyValue || key == Right.keyValue) goto PutStrokeKey;
                }
                if(Down.Isđown && Left.Isđown)
                {
                    DX = this.X - Lta * R_x;
                    DY = this.Y - Lta * R_y;
                    if (key == Down.keyValue || key == Left.keyValue) goto PutStrokeKey;
                }
                if(Down.Isđown && Right.Isđown)
                {
                    DX = this.X - Lta * R_y;
                    DY = this.Y + Lta * R_x;
                    if (key == Down.keyValue || key == Right.keyValue) goto PutStrokeKey;
                }
                //thực hiện nhấn vầ di chuyền phím nhấn
                PutStrokeKey:
                if (Up.Isđown ||  Down.Isđown || Left.Isđown ||Right.Isđown)
                {
                    if (tokenSource != null)
                    {
                        tokenSource.Cancel();
                    }
                    tokenSource = new CancellationTokenSource();
                    new Thread(() =>
                    {
                        Handremote.MoveFinger(DX, DY, this,false);
                    })
                    { IsBackground = true }.Start();
                }
            }
        }

        public override  bool Isworkingkey(VKeys key,bool Isdown)
        {
            if (key == Up.keyValue) { Up.Isđown = Isdown; return true;}
            if (key == Down.keyValue) { Down.Isđown = Isdown; return true; }
            if (key == Left.keyValue) { Left.Isđown = Isdown; return true; }
            if (key == Right.keyValue) {Right.Isđown = Isdown; return true; }
            return false;
        }
    }

    class AccelerationJoystick : Joystick
    {
        private List<Keystroke> Accers = new List<Keystroke>();

        private const int Coefficient_a = 2;
        private const int corner = 45;
        public AccelerationJoystick(VKeys Up, VKeys Down, VKeys Left, VKeys Right,int FingerId,List<VKeys> keys) : base(Up, Down, Left, Right,FingerId)
        {
            foreach (var item in keys)
            {
                Accers.Add(new Keystroke() { Isđown = false, keyValue = item });
            }
        }
        public override void Work_Key(VKeys key, bool IsDown, Switcher switcher)
        {
            if (base.Isworkingkey(key,IsDown))
            {
                if (Isworkingkey(key, IsDown))
                {
                    if (IsDown)
                    {
                        //select mode of sreeen to calculatim
                        int R_x = 1;
                        int R_y = 1;
                        int R_Df = 1;
                        int R_Ds = 1;
                        switch (switcher.Currentinformation.ScreenOrentation)
                        {
                            case ScreenOrentation.LEFT_HORIZONTAL: R_x = 1; R_y = 1; R_Ds = 1; R_Df = 0; break;
                            case ScreenOrentation.RIGHT_HORIZONTAL: R_x = -1; R_y = -1; R_Ds = 1; R_Df = 0; break;
                            case ScreenOrentation.MIDDLE_VERTICAL: R_x = 1; R_y = -1; R_Ds = 0; R_Df = 1; break;
                        }
                        double Lta_a = Math.Sin((corner * Math.PI) / 180) * this.Width * Coefficient_a;
                    }
                }
                else
                {
                    base.Work_Key(key, IsDown, switcher);
                }
            }   
        }
        public override bool Isworkingkey(VKeys key, bool Isdown)
        {
            foreach (var item in Accers)
            {
                if (item.keyValue == key) return true;
            }
            return false;
        }
    }



}
