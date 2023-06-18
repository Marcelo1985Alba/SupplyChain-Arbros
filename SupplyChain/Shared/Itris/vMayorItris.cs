﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Itris
{
    public class vMayorItris
    {
        public int IDD { get; set; }
        public double COTIZACION { get; set; }
        public int ID { get; set; }
        public DateTime FEC_ASI { get; set; }
        public DateTime FEC_VAL { get; set; }
        public string TIPO { get; set; }
        public decimal ID_1 { get; set; }
        public string DESCRIPCION { get; set; }
        public string CONCEPTO { get; set; }
        public int? FK_ERP_CEN_COSTO { get; set; }
        public int? FK_ERP_SUCURSALES { get; set; }
        public int? FK_ERP_UNI_NEG { get; set; }
        public decimal IMP_DEB { get; set; }
        public double IMP_DEB_CO { get; set; }
        public decimal IMP_HAB { get; set; }
        public double IMP_HAB_CO { get; set; }
        public decimal SALDO { get; set; }
        public decimal? IMP_DEB_SEC { get; set; }
        public decimal? IMP_HAB_SEC { get; set; }
        public decimal? SALDO_SEC { get; set; }
        public double SALDO_CO { get; set; }
        public string? OBSERVACIONES { get; set; }
        public string RAZ_SOCIAL_EMP { get; set; }
        public string OBS_DET { get; set; }
        public string OBS_COM { get; set; }
        public int FK_ERP_EJERCICIOS { get; set; }
        public int? FK_ERP_TIP_CEN { get; set; }

        public int Anio 
        {
            get
            {
                return FEC_ASI.Year;
            }
        }
        public int Mes
        {
            get
            {
                return FEC_ASI.Month;
            }
        }

        public int ID_ASIEN
        {
            get
            {
                return (int)ID_1;
            }
        }

        public string TIPO_INGRESO
        {
            get
            {
                if (ID_1.ToString().StartsWith("4"))
                {
                    return "INGRESOS";
                }
                else if(ID_1.ToString().StartsWith("5"))
                {
                    return "EGRESOS";
                }
                else
                {
                    return "N/A";
                }
            }
        }
    }
}
