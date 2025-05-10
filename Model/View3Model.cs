
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PropertyChanged;
namespace prism_serial.Model
{
    public class View3Model
    {
        public View3Model()
        {
            carData.ControlXString= carData.ControlX.ToString("F2");
            carData.ControlYString = carData.ControlY.ToString("F2");
            carData.ControlYawString = carData.ControlYaw.ToString("F2");
            TextListSelected = "车身速度";
            ControlMode = ControlMode_t.SpeedControlSelf;
        }
        public bool IsAPressed { get; set; }
        public GamepadState XboxData = new GamepadState();
        public carState carData = new carState();
        public class GamepadState
        {
            public short LeftThumbX { get; set; }
            public short LeftThumbY { get; set; }
            public short RightThumbX { get; set; }
            public short RightThumbY { get; set; }
            public byte LeftTrigger { get; set; }
            public byte RightTrigger { get; set; }
            public bool IsAPressed { get; set; }
            public bool IsBPressed { get; set; }
            public bool IsXPressed { get; set; }
            public bool IsYPressed { get; set; }

            public bool IsLBPressed { get; set; }
            public bool IsRBPressed { get; set; }
        }
        [AddINotifyPropertyChangedInterface]
        public class carState
        {
            
            public string ControlXString { get; set; }
            public string ControlYString { get; set; }
            public string ControlYawString { get; set; }
            
            public float ControlX { get; set; }
            public float ControlY { get; set; }
            public float ControlYaw { get; set; }
            // Fody 自动调用的方法
            void OnControlXChanged() => ControlXString = ControlX.ToString("F2");
            void OnControlYChanged() => ControlYString = ControlY.ToString("F2");
            void OnControlYawChanged() => ControlYawString = ControlYaw.ToString("F2");
            
        }
        public enum ControlMode_t
        {
            LocationControl,
            SpeedControlSelf,
            SpeedControlGround,
        }
        public ControlMode_t ControlMode { get; set; }
        public string TextListSelected { get; set; }
        public List<string> TextListControl { get; set; } = new() { "车身速度", "大地速度", "位置闭环" };
    }
}