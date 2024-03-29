﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vESTADOS_COMPRAS : EntityBase<int>
    {

        [NotMapped]public int Id { get; set; }
        public int? NUMERO { get; set; }
        public string? TILDE { get; set; }
        public string? ESTADOS_COMPRA { get; set; }
        public DateTime? FE_VENC { get; set; }
        public DateTime? FE_PREV { get; set; }
        public DateTime? FE_EMIT { get; set; }
        public DateTime? FE_CIERRE { get; set; }
        public int? NROCLTE { get; set; }
        public string? CG_MAT { get; set; }
        public string? DES_MAT { get; set; }
        public decimal? AUTORIZADO { get; set; }
        public decimal? SOLICITADO { get; set; }
        public string? UNID { get; set; }
        public decimal? PRECIO { get; set; }
        public decimal? BON { get; set; }
        public decimal? PRECIONETO { get; set; }
        public decimal? PRECIOTOT { get; set; }
        public string? MONEDA { get; set; }
        public string? DES_PROVE { get; set; }
        public string? ESPECIFICA { get; set; }
        public string? USUARIO { get; set; }
        public string? REMITO { get; set; }

        public string? PAGOS { get; set; }
        public string? FACTURA { get; set; } 
        public string? LETRA_FACTURA { get; set; }
        public double? TOT_DOL { get; set; }
        public decimal? CANT_USO_RECIBIDA {  get; set; }
        public decimal? CANT_COMERCIAL_RECIBIDA {  get; set; }
        public decimal? PENDIENTE_RECIBIDO {  get; set; }

    }
}
