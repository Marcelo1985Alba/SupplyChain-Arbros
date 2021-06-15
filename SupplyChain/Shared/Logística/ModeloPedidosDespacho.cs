using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{

    public class ModeloPedidosDespacho
    {
        [Key]
        public string DESPACHO { get; set; }
    }

    public class ModeloPedidosLote
    {
        [Key]
        public string LOTE { get; set; }
    }
}
