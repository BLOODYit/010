using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models.Controls
{
    public class MultiSelectModel
    {        
        public MultiSelectModel()
        {

        }
        public string ID { get; set; }
        public Dictionary<int,string> Data { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool WithCheckBoxes { get; set; } = false;
        public bool WithClearSelected { get; set; } = true;
        public bool WithSearch { get; set; } = false;
        public string KeyField { get; set; } = "ID";
        public string ValueField { get; set; } = "Name";
        public int[] Selected { get; set; } = { };
    }
}