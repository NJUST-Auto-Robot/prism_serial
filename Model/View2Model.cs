using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace prism_serial.Model
{
    public class View2Model
    {
        public View2Model()
        {
           

        }

       


    
        //发给html的数据
        public class SerialPoints
        {
            public List<double> _x { set; get; } = new();
            public List<double> _y { set; get; } = new();
        }

    }
}