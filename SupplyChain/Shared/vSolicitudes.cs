﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vSolicitudes
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(maximumLength: 15, MinimumLength = 15)]
        public string Producto { get; set; }
        public string DES_PROD { get; set; }
        public int CG_CLI { get; set; } = 0;
        public string DES_CLI { get; set; }
        public int TagId { get; set; } = 0;
        public string Cuit { get; set; } = "";
        public int Cantidad { get; set; }
        public bool TienePresupuesto { get; set; }
    }
}
