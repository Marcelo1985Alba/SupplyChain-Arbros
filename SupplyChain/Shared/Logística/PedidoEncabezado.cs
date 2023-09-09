using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplyChain.Shared.HelpersAtributo;
using SupplyChain.Shared.Models;

namespace SupplyChain.Shared;

public class PedidoEncabezado
{
    [Key] public int REGISTRO { get; set; }

    public DateTime FE_MOV { get; set; } = DateTime.Now;
    public int VALE { get; set; }

    [Required(ErrorMessage = "Ingresar Tipo de Operación de Stock")]
    public int TIPOO { get; set; } = 0;

    public int CG_CLI { get; set; } = 0;
    public string DES_CLI { get; set; } = string.Empty;

    [ColumnaGridViewAtributo(Name = "Condicion de Entrega")]
    public int CG_COND_ENTREGA { get; set; } = 0;

    [ColumnaGridViewAtributo(Name = "Condicion de Pago")]
    public int CG_CONDICION_PAGO { get; set; } = 0;

    public int CG_TRANS { get; set; } = 0;
    public decimal BONIFIC { get; set; } = 0;

    [ColumnaGridViewAtributo(Name = "Indice conversión moneda")]
    public decimal? VA_INDIC { get; set; } = 1;

    public string DIRENT { get; set; } = string.Empty;
    public string MONEDA { get; set; } = string.Empty;
    public int PEDIDO { get; set; } = 0;

    /// <summary>
    ///     ORDEN DE COMPRA DE PROVEEDOR
    /// </summary>
    public int? OCOMPRA { get; set; } = 0;

    /// <summary>
    ///     ORDEN DE COMPRA DE CLIENTE
    /// </summary>
    public string ORCO { get; set; } = string.Empty;

    public int CG_ORDF { get; set; } = 0;

    [RequireRemito] public string REMITO { get; set; } = "0000-00000000";

    //public int CG_DEP { get; set; } = 0;
    public int CG_DEP_ALT { get; set; } = 0;
    public int VOUCHER { get; set; } = 0;
    public decimal TOTAL { get; set; } = 0;

    public string BULTOS { get; set; } = string.Empty;
    public string MONTO { get; set; } = string.Empty;

    [ValidateComplexType] public List<Pedidos> Items { get; set; } = new();

    [NotMapped] public ModeloOrdenFabricacionEncabezado ModeloOrdenFabricacionEncabezado { get; set; } = new();
}