﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vTransportes : EntityBase<int>
    {
        public string DESCRIPCION { get; set; }
        public string CALLE { get; set; }
    }
}