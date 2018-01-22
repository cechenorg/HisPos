﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Feature
    {
        public string Icon { get; set; }
        public string Title { get; set; }

        public string[] Functions;

        public Feature(string icon, string title, string[] functions)
        {
            Icon = icon;
            Title = title;
            Functions = functions;
        }
    }
}
