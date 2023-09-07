using Microsoft.EntityFrameworkCore.Query.Internal;
using SupplyChain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class Costos
    {
        public double egresos { get; set; }
        public decimal unidades_equivalentes { get; set; }
        public double coeficiente { get; set; }
    }
}