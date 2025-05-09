using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using prism_serial.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using System.Management;
using prism_serial.Common.Events;

namespace prism_serial.ViewModels
{
    /// <summary>
    /// Represents the ViewModel for View1.
    /// </summary>
    public class View1ViewModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="View1ViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="serialPortin">The serial port.</param>
        public View1ViewModel(IEventAggregator eventAggregator, SerialPort serialPortin, Thread dataTransThread)
        {
            this._serialPort = serialPortin;
            _dataTransThread = dataTransThread;
            this._eventAggregator = eventAggregator;

            _serialPort.Encoding = Encoding.UTF8;
            _serialPort.DataReceived += SerialDataReceived;
            ButtonCommand = new DelegateCommand<object>(obj => SearchAvailableCom());
            ClearCommand = new DelegateCommand<object>(obj => ReceivedText = "");
            OpenCloseCommand = new DelegateCommand<object>(OnOpenCloseCommand);
            TransClearCommand = new DelegateCommand<object>(obj => TransText = "");
            TransButtonClickCommand = new DelegateCommand<object>(obj => TransData(_dataTransThread));
            ChangEncoderCommand = new DelegateCommand<object>(OnChangeEncoder);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += (s, e) =>
            {
                SearchAvailableCom();
                GetAllComInfo(ComNameList);
            };
            _timer.IsEnabled = true;
        }

        // Resources
        private SerialPort _serialPort;

        private DispatcherTimer _timer = new DispatcherTimer();
        private DispatcherTimer _timer2 = new DispatcherTimer();
        private readonly IEventAggregator _eventAggregator;
        private View1Model _obj = new View1Model();
        private Thread _dataTransThread;

        // Bindable properties
        /// <summary>
        /// Gets or sets the received text.
        /// </summary>
        public string ReceivedText
        {
            get => (string)_obj.ReceivedText;
            set
            {
                _obj.ReceivedText = value;
                ShowText = ReceivedText;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text to be transmitted.
        /// </summary>
        public string TransText
        {
            get => _obj.TransText;
            set
            {
                _obj.TransText = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text to be displayed.
        /// </summary>
        public string ShowText
        {
            get => _obj.ShowText;
            set
            {
                if (_obj.ShowText != value)
                {
                    _obj.ShowText = value.Replace(@"\n", Environment.NewLine);
                }

                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the baud rates.
        /// </summary>
        public List<int> Baudrate
        {
            get => (List<int>)_obj.Baudrate;
            set
            {
                _obj.Baudrate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the available COM ports.
        /// </summary>
        public ObservableCollection<string> Com
        {
            get => _obj.Com;
            set
            {
                _obj.Com = value;
                RaisePropertyChanged();
            }
        }

        public string ComDetialedInfo
        {
            get => _obj.ComDetialedInfo;
            set
            {
                _obj.ComDetialedInfo = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected baud rate.
        /// </summary>
        public int BaudrateSelect
        {
            get => (int)_obj.BaudrateSelect;
            set
            {
                _obj.BaudrateSelect = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected COM port.
        /// </summary>
        public string ComSelect
        {
            get => _obj.ComSelect;
            set
            {
                _obj.ComSelect = value;
                RaisePropertyChanged();
                MatchComInfo(ComNameList, value);

            }
        }

        public List<string> ComNameList = new();

        /// <summary>
        /// Gets or sets a value indicating whether the COM port and baud rate selection is enabled.
        /// </summary>
        public bool IsComBaudEnable
        {
            get => _obj.IsComBaudEnable;
            set
            {
                _obj.IsComBaudEnable = value;
                RaisePropertyChanged();
            }
        }


        private bool _use_utf_8 = true;

        // Bindable commands
        /// <summary>
        /// Gets or sets the command for the button.
        /// </summary>
        public DelegateCommand<object> ButtonCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for clearing the received text.
        /// </summary>
        public DelegateCommand<object> ClearCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for clearing the transmitted text.
        /// </summary>
        public DelegateCommand<object> TransClearCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for opening or closing the serial port.
        /// </summary>
        public DelegateCommand<object> OpenCloseCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for transmitting the data.
        /// </summary>
        public DelegateCommand<object> TransButtonClickCommand { get; set; }

        public DelegateCommand<object> ChangEncoderCommand { get; set; }

        // Event handlers
        private void SerialDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[_serialPort.BytesToRead];
                _serialPort.Read(buffer, 0, buffer.Length);

                // 根据编码方式处理数据
                string receivedText;
                if (_use_utf_8)
                {
                    receivedText = Encoding.UTF8.GetString(buffer);
                }
                else
                {
                    StringBuilder hex = new StringBuilder(buffer.Length * 2);
                    foreach (byte b in buffer)
                    {
                        hex.AppendFormat("{0:x2} ", b);
                    }

                    receivedText = hex.ToString();
                }
                _eventAggregator.GetEvent<SerialMessage>().Publish(receivedText);
                ReceivedText += receivedText;
            }
            catch
            {
            }
        }

        [Obsolete("This method is inefficient.", true)]
        private void SearchAvailableCom(SerialPort mySerial)
        {
            try
            {
                Com.Clear();
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        var tempString = "COM" + i.ToString();
                        mySerial.PortName = tempString;
                        mySerial.Open();
                        Com.Add(tempString);
                        mySerial.Close();
                    }
                    catch
                    {
                        // ignored
                    }

                    ;
                }
            }
            catch
            {
                // ignored
            }
        }

        private void SearchAvailableCom()
        {
            string selectedPort = ComSelect;
            string[] ports = SerialPort.GetPortNames();
            bool shouldUpdate = !Com.SequenceEqual(ports);

            if (shouldUpdate)
            {
                Com.Clear();
                foreach (string port in ports)
                {
                    Com.Add(port);
                }

                if (ports.Contains(selectedPort))
                {
                    ComSelect = selectedPort;
                }
            }
        }

        private void OnOpenCloseCommand(object? obj)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)obj;
            if (button.Content.ToString() == "打开串口")
            {
                try
                {
                    _serialPort.PortName = ComSelect;
                    _serialPort.BaudRate = BaudrateSelect;
                    _serialPort.Open();
                    button.Content = "关闭串口";
                    IsComBaudEnable = false;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
            else
            {
                try
                {
                    _serialPort.Close();
                    button.Content = "打开串口";
                    IsComBaudEnable = true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
        }

        private void OnChangeEncoder(object? obj)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)obj;
            if (button.Content.ToString() == "Abc")
            {
                button.Content = "Hex";
                //将编码方式改为16进制
                //_serialPort.Encoding = Encoding.Unicode;
                _use_utf_8 = false;
                //把DataReceived转化成16进制
             

            }
            else if (button.Content.ToString() == "Hex")
            {
                button.Content = "Abc";
                //将编码方式改为UTF-8
                //_serialPort.Encoding = Encoding.UTF8;
                _use_utf_8 = true;
            }
        }

        [Obsolete("Use s, e pattern to handle keyboard input.", true)]
        private void OnTxBox_KeyDownCommand(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    _serialPort.Write(TransText);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + ex.Message);
                }
            }
            else if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
            }
        }

        private void TransData(Thread? thread)
        {
            thread = new Thread(() =>
            {
                try
                {
                    _serialPort.WriteTimeout = 2000;
                    _serialPort.Write(TransText);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + ex.Message);
                }
            });
            thread.Start();
            thread.Join();
        }

        public void OnTxBox_KeyDownCommand(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;

            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                int caretIndex = textBox.CaretIndex;
                TransText = textBox.Text;
                int insertionPoint = caretIndex;
                textBox.Text = textBox.Text.Insert(insertionPoint, "\r\n");
                textBox.CaretIndex = insertionPoint + 2;
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                TransText = textBox.Text;
                TransData(_dataTransThread);
            }
        }

        public void OnOpenCloseCommand(object? obj, EventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)obj;
            if (button.Content.ToString() == "打开串口")
            {
                try
                {
                    _serialPort.PortName = ComSelect;
                    _serialPort.BaudRate = BaudrateSelect;
                    _serialPort.Open();
                    button.Content = "关闭串口";
                    IsComBaudEnable = false;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
            else
            {
                try
                {
                    _serialPort.Close();
                    button.Content = "打开串口";
                    IsComBaudEnable = true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
        }

        //匹配串口信息
        private void MatchComInfo(List<string> comNameList, string comName)
        {
            string comInfo = "";
            foreach (string com in comNameList)
            {
                if (com.Contains(comName))
                {
                    comInfo = com;
                    break;
                }
            }

            ComDetialedInfo = comInfo;
        }



        //获得所有串口数据
        private void GetAllComInfo(List<string> comNameList)
        {
            try
            {
                // 使用指定查询创建搜索器
                using (ManagementObjectSearcher searcher =
                       new ManagementObjectSearcher("SELECT Name FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%'"))
                {
                    // 仅获取前几个结果以限制CPU负载
                    foreach (ManagementObject device in searcher.Get().OfType<ManagementObject>().Take(5))
                    {
                        if (device["Name"] != null)
                        {
                            string name = device["Name"].ToString();
                            comNameList.Add(name);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}