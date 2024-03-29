﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.CDM
{
    public class CargaMaq
    {
            [Key]
            public Guid REGUID { get; set; }
            public int ORDEN { get; set; }
            public int CG_ORDEN { get; set; }
            public int CG_ESTADOCARGA { get; set; }
            public int CG_ORDF { get; set; }
            public int CG_ORDFORIG { get; set; }
            public int CG_ORDFASOC { get; set; }
            public string CG_CELDA { get; set; }
            public string DES_CELDA { get; set; }
            public string CG_PROD { get; set; }
            public string DES_PROD { get; set; }
            public string PROCESO { get; set; }
            public int CG_CLI { get; set; }
            public string DES_CLI { get; set; }
            public DateTime? FECHA_PREVISTA_FABRICACION { get; set; }
            public string HORA { get; set; }
            public decimal HORASFAB { get; set; }
            public int ULTIMAORDENASOCIADA { get; set; }
            public decimal MINUTOSFAB { get; set; }
            public DateTime? FE_ENTREGA { get; set; }
            public decimal DIASFAB { get; set; }
            public decimal CANT { get; set; }
            public decimal CANTFAB { get; set; }
            public decimal AVANCE { get; set; }
            public string BACKGROUND { get; set; }
            public string CLASE { get; set; }
            public bool INSUMOS_ENTREGADOS_A_PLANTA { get; set; }
            public int FILA { get; set; }
            public int COLUMNA { get; set; }
            public int COLUMNSPAN { get; set; }
            public int FLAG_DEPENDENCIAS { get; set; }
            public bool EXIGEOA { get; set; }
            public int PEDIDO { get; set; }
            public DateTime FE_CURSO { get; set; }
            public DateTime? FE_FIRME { get; set; }
            public bool BORDE { get; set; }
            public int ORDEN_CELDA { get; set; }
            public int SELECCIONADA { get; set; }
        }
    
}
