using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.interfaceUI
{
    class ListfunctionKey
    {
        public static List<FunctionkeyModel> GetListFunc()
        {
            List<FunctionkeyModel> FunctionkeyModels = new List<FunctionkeyModel>();

            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "R",
                Name = "Reload",
                Description = "Reload bullet"
            });
            
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "W",
                Name = "Up",
                Description = "Move forward"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "S",
                Name = "Down",
                Description = "Move back"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "A",
                Name = "Left",
                Description = "Move left"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "D",
                Name = "Right",
                Description = "Move right"
            });
            
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Q",
                Name = "Tilt Left",
                Description = "Tilt body left"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "E",
                Name = "Tilt Right",
                Description = "Tilt body right"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "C",
                Name = "Sit",
                Description = "Sit down"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "X",
                Name = "Lay",
                Description = "Lay body"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "H",
                Name = "Hold",
                Description = "Loot box"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "F",
                Name = "Drive",
                Description = "Get in and drive car"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "F",
                Name = "Exit",
                Description = "Exit car , boat "
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "G",
                Name = "Get in",
                Description = "Get in car "
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "G",
                Name = "Open close ",
                Description = "Open and close door"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "LC",
                Name = "Right fire ",
                Description = "Shot your gun,left click"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "M",
                Name = "Open map",
                Description = "Open and close map"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "RC",
                Name = "Scope on",
                Description = "Turn off and on scope,right click"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Spa",
                Name = "Jump",
                Description = "Jump body"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "T",
                Name = "Cancel",
                Description = "Cancel bom,action"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Z",
                Name = "Receive",
                Description = "Save some people"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "1",
                Name = "Gun first",
                Description = "Get the left gun"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "2",
                Name = "Gun second",
                Description = "Get the right gun"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "3",
                Name = "Gun third",
                Description = "Get the mini gun"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "4",
                Name = "Add",
                Description = "Add blood,drink"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "B",
                Name = "Bomb",
                Description = "Use bomd,frag,smoke"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Tab",
                Name = "Open bag",
                Description = "Open your package"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Esc",
                Name = "Setting",
                Description = "Open setting"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "MC",
                Name = "Mouse eye",
                Description = "Middle click,use eye"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "L",
                Name = "Parachute",
                Description = "Use parachute,unfollow"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "N0",
                Name = "Change sit",
                Description = "Numpad 0,change your sitting"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Up",
                Name = "Booster",
                Description = "Double key up,boost the car"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Do",
                Name = "Brake",
                Description = "Double key down,brake the car"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Ri",
                Name = "Turn right",
                Description = "Turn right car"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Left",
                Name = "Turn left",
                Description = "Turn left car"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Up",
                Name = "Move car forward",
                Description = "Move car forward"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Do",
                Name = "Move car back",
                Description = "Down key,Move car back"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "Ctrl",
                Name = "Look out",
                Description = "Right ctrl,look out in car"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "U",
                Name = "Swim up",
                Description = "Swim up"
            });
            FunctionkeyModels.Add(new FunctionkeyModel()
            {
                Key = "J",
                Name = "Swim down",
                Description = "Swin down"
            });
            return FunctionkeyModels;
        }
    }
}
