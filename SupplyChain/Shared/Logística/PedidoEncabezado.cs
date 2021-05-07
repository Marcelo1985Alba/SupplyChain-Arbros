﻿using SupplyChain.Shared.HelpersAtributo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SupplyChain.Shared
{
    public class PedidoEncabezado
    {
        [Key]
        public int REGISTRO { get; set; }
        public DateTime FE_MOV { get; set; } = DateTime.Now;
        public int VALE { get; set; }
        public int TIPOO { get; set; }
        public int PEDIDO { get; set; } = 0;
        public int? OCOMPRA { get; set; } = 0;
        public int CG_ORDF { get; set; } = 0;
        [RequireRemito]
        public string REMITO { get; set; } = "0000-00000000";
        //public int CG_DEP { get; set; } = 0;
        public int CG_DEP_ALT { get; set; } = 0;
        public int VOUCHER { get; set; } = 0;
        [ValidateComplexType]
        public List<Pedidos> Items { get; set; }

    }
}
