﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLaunch._Debug.DATDB.Platforms.NOINTRO.Models
{
    public class NoIntroCollection
    {
        // Properties
        public List<NoIntroObject> Data { get; set; }

        // Constructors
        public NoIntroCollection()
        {
            Data = new List<NoIntroObject>();

            Data = ImportNoIntroData.Go();
        }
    }
}
