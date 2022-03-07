using SupplyChain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
    public class PedCli : EntityBase
    {
        private string _color;
        [Key]
        public int REGISTRO { get; set; } = 0;
        public DateTime FE_PED { get; set; }
        public int PEDIDO { get; set; } = 0;
        public decimal CANTPED { get; set; }
        public int NUMOCI { get; set; } = 0;
        public int CG_CLI { get; set; } = 0;
        public string DES_CLI { get; set; } = "";
        public string ORCO { get; set; } = "";
        public string CG_ART { get; set; } = "";
        public string DES_ART { get; set; } = "";
        public string OBSERITEM { get; set; } = "";
        public string DIRENT { get; set; } = "";
        public string CG_ESTADO { get; set; } = "";
        public int CG_ESTADPEDCLI { get; set; } = 0;
        public string ESTADO_LOGISTICA { get; set; } = "";
        public string LOTE { get; set; } = "";
        public string CAMPOCOM1 { get; set; } = "";
        public string CAMPOCOM6 { get; set; } = "";
        public string CAMPOCOM3 { get; set; } = "";
        public string CAMPOCOM4 { get; set; } = "";
        public string CAMPOCOM5 { get; set; } = "";
        public string CAMPOCOM2 { get; set; } = "";
        public string REMITO { get; set; } = "";
        public DateTime ENTRPREV { get; set; }

        //[Column("FLAG")]
        [NotMapped]
        public bool CONFIRMADO { get; set; }
        [NotMapped]
        public string COLOR
        {
            get { return _color; }
            set
            {
                _color = ESTADO_LOGISTICA switch
                {
                    "Remitir" => "red",
                    "Inspeccion" => "yellow",
                    "Ret.Planta" => "greenyellow",
                    "Ret.CABA" => "greenyellow",
                    "Entregar" => "pink",
                    "Facturar" => "maroon",
                    "Pago" => "blue",
                    _ => "black",
                };
            }
        }
    }
}
