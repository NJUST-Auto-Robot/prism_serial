using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using prism_serial.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Prism.Events;
using prism_serial.Common.Events;

namespace prism_serial.ViewModels
{
    public class View2ViewModel : BindableBase
    {
        public View2ViewModel(IEventAggregator aggregator)
        {
           
            this._eventAggregator = aggregator;
            TestCommand = new DelegateCommand(() => { isVisual = !isVisual;});
            
            //_serialPort.DataReceived += SerialDataVisual;
            _eventAggregator.GetEvent<SerialMessage>().Subscribe(OnDataVisual);
        }

        private View2Model _obj = new View2Model();
        private readonly IEventAggregator _eventAggregator;


        private View2Model.SerialPoints serialData = new();

        private bool isVisual = false;
      

        //往charpage.html传递数据
        public DelegateCommand TestCommand { get; set; }

      
        public delegate void PostDelegate(string webMessageAsJson);

        //给Web页面传递数据
        public PostDelegate postDelegate;

        private void SerialDataVisual(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if(isVisual) 
            {
                var serialPort = (SerialPort)sender;
                var data = serialPort.ReadExisting();
                try
                {
                    serialData._y.Add(Convert.ToDouble(data));
                    serialData._x.Add(serialData._x.Count);
                    //postDelegate?.Invoke(JsonConvert.SerializeObject(serialData));
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        postDelegate?.Invoke(JsonConvert.SerializeObject(serialData));
                    });
                    //清空数据
                    serialData._x.Clear();
                    serialData._y.Clear();
                }
                catch (Exception)
                {
                    Console.WriteLine("数据转换失败");
                }
            }
        }
        private void OnDataVisual(string data)
        {
            try
            {
                serialData._y.Add(Convert.ToDouble(data));
                serialData._x.Add(serialData._x.Count);
                //postDelegate?.Invoke(JsonConvert.SerializeObject(serialData));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    postDelegate?.Invoke(JsonConvert.SerializeObject(serialData));
                });
                //清空数据
                serialData._x.Clear();
                serialData._y.Clear();
            }
            catch (Exception)
            {
                Console.WriteLine("数据转换失败");
            }
        }
        private void OnTest()
        {
            //Task.Run(() =>//测试模拟后台输出文件
            //{
            //    var sePoints1 = new double[20, 2];
            //    for (int i = 0; i < 20; i++)
            //    {
            //        Thread.Sleep(2000);
            //        sePoints1[i, 0] = i;
            //        sePoints1[i, 1] = i * 1.2;
            //        Console.WriteLine("do Task work,i={0}", i);
            //    }
            //});
            ////Web.ObjectForScripting.
            //var sePoints = new double[20, 2];
            //for (int i = 0; i < 20; i++)
            //{
            //    sePoints[i, 0] = i;
            //    sePoints[i, 1] = i * 1.2;
            //}
            for (int i = 0;i< 20; i++)
            {
                serialData._x.Add(i);
                serialData._y.Add(i * 1.2);
            }
            //serialData._x.Add();
            postDelegate?.Invoke(JsonConvert.SerializeObject(serialData));
        }

        
    }
}