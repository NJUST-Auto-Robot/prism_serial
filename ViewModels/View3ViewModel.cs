using Prism.Commands;
using Prism.Mvvm;
using prism_serial.Model;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Timers;
using static prism_serial.Model.View3Model;
//using v
namespace prism_serial.ViewModels
{
    public class View3ViewModel : BindableBase
    {
        public View3ViewModel(SerialPort serialPortin)
        {
            _serial = serialPortin;
            _controller = null;
           
            CarCommand = new DelegateCommand(() => {
            CarCommandToSerial(_xboxData.LeftThumbX, _xboxData.LeftThumbY, _xboxData.RightThumbX);
            });
            ClearCommand = new DelegateCommand(() =>
            {
                _carData.ControlX = 0;
                _carData.ControlY = 0;
                _carData.ControlYaw = 0;
            });
            CarStopCommand = new DelegateCommand(() =>
            {
                ControlMode = ControlMode_t.SpeedControlSelf;
                
                CarCommandToSerial(0.0f,0.0f,0.0f);
            });
            StartReadingController();
        }
        SerialPort _serial ;
        private View3Model _obj = new View3Model();
        //public bool IsAPressed
        //{
        //    get => _obj.IsAPressed; set {
        //        if (_obj.IsAPressed != value)
        //        {
        //            _obj.IsAPressed = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}
        private async void StartReadingController()
        {
            // 持续异步读取 Xbox 控制器状态
            while (true)
            {
                if (_controller==null || !_controller.IsConnected)
                {
                    _controller = new Controller(UserIndex.One);
                    await Task.Delay(1000); // 1秒后重试
                }
                while (_controller != null && _controller.IsConnected)
                {
                    // 读取控制器数据
                    ReadController();
                    // 等待一段时间再继续读取
                    await Task.Delay(15); // 100ms 轮询间隔，避免过于频繁
                }
                
            }
        }

        public GamepadState _xboxData
        {
            get => _obj.XboxData; set {SetProperty(ref _obj.XboxData,value); RaisePropertyChanged(); }
        }
        public carState _carData
        {
            get => _obj.carData; set { SetProperty(ref _obj.carData, value); RaisePropertyChanged(); }
        }
        public ControlMode_t ControlMode
        {
            get => _obj.ControlMode; set { _obj.ControlMode=value; RaisePropertyChanged(); }
        }
        //command
        public DelegateCommand CarCommand { get; set; }

        public DelegateCommand CarStopCommand { get; set; }

        //清零设定值
        public DelegateCommand ClearCommand { get; set; }

        public List<string> TextListControl
        {
            get => _obj.TextListControl;
            set => _obj.TextListControl = value;
        }
        public string TextListSelected
        {
            get => _obj.TextListSelected;
            set
            {
                _obj.TextListSelected = value;
                if (value == "车身速度")
                {
                    ControlMode = View3Model.ControlMode_t.SpeedControlSelf;
                }
                else if (value == "大地速度")
                {
                    ControlMode = View3Model.ControlMode_t.SpeedControlGround;
                }
                else if (value == "位置闭环")
                {
                    ControlMode = View3Model.ControlMode_t.LocationControl;
                }
            }
        }
        //private Timer _timer;
        private Controller _controller;
        //private GamepadState _state = new GamepadState();
        public short _deadZone = 500; // 死区值
        private void ReadController()
        {
            var state = _controller.GetState();
            
            _xboxData = new GamepadState
            {
                LeftThumbX = state.Gamepad.LeftThumbX,
                LeftThumbY = state.Gamepad.LeftThumbY,
                RightThumbX = state.Gamepad.RightThumbX,
                RightThumbY = state.Gamepad.RightThumbY,
                LeftTrigger = state.Gamepad.LeftTrigger,
                RightTrigger = state.Gamepad.RightTrigger,
                IsAPressed = (state.Gamepad.Buttons & GamepadButtonFlags.A) != 0,
                IsBPressed = (state.Gamepad.Buttons & GamepadButtonFlags.B) != 0,
                IsXPressed = (state.Gamepad.Buttons & GamepadButtonFlags.X) != 0,
                IsYPressed = (state.Gamepad.Buttons & GamepadButtonFlags.Y) != 0

            };
            //IsAPressed = (state.Gamepad.Buttons & GamepadButtonFlags.A) != 0;
            xboxSendToSerial();
            Console.WriteLine($"LX: {_xboxData.LeftThumbX}, LY: {_xboxData.LeftThumbY}, RX: {_xboxData.RightThumbX}, RY: {_xboxData.RightThumbY}, LT: {_xboxData.LeftTrigger}, RT: {_xboxData.RightTrigger}");
        }
        //对原始数据进行处理
        private int xboxDataHandle(short data)
        {
            //过滤死区并映射到-100到100
            int deadZone = _deadZone;
            int maxValue = 32767;
            int minValue = -32768;

            if (data > deadZone)
            {
                // 映射正数范围
                double ratio = (double)(data - deadZone) / (maxValue - deadZone);
                return (int)Math.Round(ratio * 100);
            }
            else if (data < -deadZone)
            {
                // 映射负数范围
                double ratio = (double)(data + deadZone) / (minValue + deadZone);
                return (int)Math.Round(ratio * -100);
            }
            else
            {
                // 在死区范围内返回0
                return 0;
            }
        }
        private void xboxSendToSerial()
        {
            if (_serial.IsOpen)
            {
                try
                {
                    var thumbx = (short)xboxDataHandle(_xboxData.LeftThumbX);
                    var thumby = (short)xboxDataHandle(_xboxData.LeftThumbY);
                    //通过串口发送数据帧头为0xED
                    //byte[] data = new byte[9];


                    byte[] xBytes = BitConverter.GetBytes(thumbx);
                    byte[] yBytes = BitConverter.GetBytes(thumby);

                    // 使用 List<byte> 动态拼接数组
                    List<byte> dataList = new List<byte>();
                    dataList.Add(0xED); // 添加帧头
                    dataList.AddRange(xBytes); // 添加 xBytes
                    dataList.AddRange(yBytes); // 添加 yBytes
                                                  
                    // 将 List<byte> 转换为 byte[]
                    byte[] data = dataList.ToArray();
                    _serial.Write(data, 0, data.Length);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data to serial port: {ex.Message}");
                }
            }
        }
        private void CarCommandToSerial(float linear_x,float linear_y,float angular_z)
        {
            if (_serial.IsOpen)
            {
                byte[] floatBytes = BitConverter.GetBytes(linear_x);
                byte[] floatBytes2 = BitConverter.GetBytes(linear_y);
                byte[] floatBytes3 = BitConverter.GetBytes( (float)(angular_z*Math.PI));
                List<byte> dataList = new List<byte>();
                byte frameHead=0x00;
                switch (ControlMode)
                {
                    case ControlMode_t.SpeedControlSelf:
                        frameHead = 0xFF;
                        break;

                    case ControlMode_t.SpeedControlGround:
                        frameHead = 0xFD;
                        break;

                    case ControlMode_t.LocationControl:
                        frameHead = 0xFB;
                        break;
                }
                dataList.Add(frameHead); // 添加帧头
                dataList.AddRange(floatBytes); // 添加 xBytes
                dataList.AddRange(floatBytes2); // 添加 yBytes
                dataList.AddRange(floatBytes3); // 添加 yBytes
                byte frameTail = 0x00;
                switch (ControlMode)
                {
                    case ControlMode_t.SpeedControlSelf:
                        frameTail = 0xFE;
                        break;

                    case ControlMode_t.SpeedControlGround:
                        frameTail = 0xFC;
                        break;

                    case ControlMode_t.LocationControl:
                        frameTail = 0xFA;
                        break;
                }
                dataList.Add(frameTail); // 添加帧尾
                byte[] combinedBytes = dataList.ToArray();
                _serial.Write(combinedBytes, 0, 15);
            }
        }
        
    }
}