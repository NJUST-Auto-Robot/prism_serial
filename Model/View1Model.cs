using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace prism_serial.Model
{
    public class View1Model
    {
        public View1Model()
        {
            ReceivedText = string.Empty;
            TransText = string.Empty;
            ShowText = string.Empty;
            Com = new ObservableCollection<string>();
            //log=new ObservableCollection<string>();
            //log = new ObservableCollection<MessageItem>();
            BaudrateSelect = 115200;
            IsComBaudEnable = true;
            IsvisualData = false;
        }
        public string ComDetialedInfo
        { get; set; }
        public string ReceivedText
        { get; set; }

        public string TransText
        { get; set; }

        public string ShowText
        { get; set; }

        public ObservableCollection<string> Com { get; set; }
        public List<int> Baudrate = new() { 9600, 115200, 1152000, 2000000 };

        public bool IsComBaudEnable { get; set; }
        public bool IsvisualData { get; set; }
        public int BaudrateSelect { get; set; }
        public string ComSelect { get; set; }
    }
}