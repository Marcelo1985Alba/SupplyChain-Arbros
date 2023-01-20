using SupplyChain.Shared;
using SupplyChain.Shared.Enum;
using SupplyChain.Shared.HelpersAtributo;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
    /// <summary>
    /// La tabla que muestra, modifica o elimina registros para stock
    /// </summary>
    [Table("Pedidos")]
    public class Pedidos : EntityBase<int?>
    {
        [Key, Column("REGISTRO")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ColumnaGridViewAtributo(Name = "Registro")]
        new public int? Id { get; set; }

        /*AGREGO CAMPOS (EXIGEDESPACHO, EXIGELOTE, EXIGESERIE) NO MAPEADOS A LA BASE DE DATOS 
         * QUE DEBEN SER SETEADOS POR EL PRODUCTO PARA VALIDACIONES.
         * PARA DETERMINAR SI LOS CAMPOS DESPACHO, SERIE Y LOTE
         * DEBEN SER REQUERIDOS. DE NO AGREGAR ESTO NO PERMITE REALIZAR LA GRABACION EN LA BASE DE DATOS POR EF
         * SE CREA UN ATRIBUTO ESPECIAL PARA CADA CAMPO: Ejemplo RequireWhenExigeDespacho
         */
        [NotMapped]
        public bool EXIGEDESPACHO { get; set; } = false;
        [NotMapped]
        public bool EXIGELOTE { get; set; } = false;
        [NotMapped]
        public bool EXIGESERIE { get; set; } = false;
        [NotMapped]
        public decimal? PENDIENTEOC { get; set; } = 0;
        public bool CERRAROC { get; set; } = false;

        /* 
         * AGREGO MODELO RESUMENSTOCK PARA VALIDAR QUE LA CANTIDAD INGRESADA NO SUPERE A LA DEL STOCK: 
         * EJEM: EN DEV A PROVE
         */
        [NotMapped]
        public vResumenStock ResumenStock { get; set; } = new vResumenStock();

        [NotMapped]
        public Cliente Cliente { get; set; } = new Cliente();
        [NotMapped]
        public Proveedor Proveedor { get; set; } = new Proveedor();

        /*
         * Agrego campo CG_DEP_ALT no mapeado para guardar el otro deposito: 
         * Por ejemplo: para la operacion Movim entre dep donde necesito generar dos registros
         * 1) con el deposito de ingres
         * 2) con el deposito de salida
         */
        [NotMapped]
        [RequireDepositoItem]
        public int CG_DEP_ALT { get; set; } = 0;

        [ColumnaGridViewAtributo(Name = "Vale")]
        public int VALE { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Fecha Vale")]
        public DateTime FE_MOV { get; set; }
        [ColumnaGridViewAtributo(Name = "Asiento contable")]
        public int VOUCHER { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Comprobante")]
        public string COMPROB { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Factura")]
        public string FACTURA { get; set; } = "0000-00000000";
        [ColumnaGridViewAtributo(Name = "Letra de la factura")]
        public string LEYENDA { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Remito")]
        public string REMITO { get; set; } = "0000-00000000";
        [ColumnaGridViewAtributo(Name = "Tipo de operación")]
        public int? TIPOO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Pedido")]
        public int? PEDIDO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Orden de compra cliente")]
        public decimal NUMOCI { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Orden de compra proveedor")]
        public int? OCOMPRA { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Orden de fabricación")]
        public int? CG_ORDF { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Observaciones")]
        public string OBSERVACIONES { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Especificaciones")]
        public string OBSERITEM { get; set; } =  "";
        [ColumnaGridViewAtributo(Name = "Observaciones")]
        public string OBS1 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Observaciones")]
        public string OBS2 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Observaciones")]
        public string OBS3 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Observaciones")]
        public string OBS4 { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Aviso")]
        public string AVISO { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Dirección entrega")]
        public string DIRENT { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Tipo insumo")]
        public int? CG_ORDEN { get; set; } = 0;
        [Display(Name = "Código artículo"), Required(ErrorMessage ="ingresar insumo")]
        public string CG_ART { get; set; } = "";
        [Display(Name = "Nombre artículo")]
        public string DES_ART { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Tipo artículo")]
        public string TIPO { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Lote")]
        [RequireWhenExigeLote]
        public string LOTE { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Serie")]
        [RequireWhenExigeSerie]
        public string SERIE { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Despacho")]
        [RequireWhenExigeDespacho]
        public string DESPACHO { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Ubicación")]
        public string UBICACION { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Depósito")]
        [RequireDepositoItem]
        public int CG_DEP { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Cantidad")]
        public decimal? CANTENT { get; set; } = 0;
        [Display(Name = "Cantidad operación")]
        [ControlCant]
        public decimal? STOCK { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Unidad stock")]
        public string UNID { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Factor conversión")]
        public decimal? CG_DEN { get; set; } = 1;
        [ColumnaGridViewAtributo(Name = "Cantidad comercial")]
        public decimal? STOCKA { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Unidad comercial")]
        public string UNIDA { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Cantidad comercial")]
        public decimal? CANTENTA { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Fecha entrega")]
        public DateTime? ENTRREAL { get; set; }
        [ColumnaGridViewAtributo(Name = "Moneda")]
        public string MONEDA { get; set; } = "PESOS";
        [ColumnaGridViewAtributo(Name = "Precio unitario")]
        public decimal? IMPORTE1 { get; set; } = 0;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [ColumnaGridViewAtributo(Name = "Precio total")]
        public decimal? IMPORTE2 { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [ColumnaGridViewAtributo(Name = "Importe del descuento")]
        public decimal? IMPORTE3 { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [ColumnaGridViewAtributo(Name = "Precio total con descuento")]
        public decimal? IMPORTE4 { get; set; }
        [ColumnaGridViewAtributo(Name = "Precio")]
        public decimal? IMPORTE6 { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Porciento descuento")]
        public decimal? DESCUENTO { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Porciento bonificación")]
        public decimal? BONIFIC { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Porciento IVA")]
        public decimal? IVA { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Indice conversión moneda")]
        public decimal? VA_INDIC { get; set; } = 1;
        [ColumnaGridViewAtributo(Name = "Cuenta contable")]
        public decimal? CG_CUENT { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "CUIT")]
        public string CUIT { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Código proveedor")]
        [ForeignKey("Proveedor")]
        public int? CG_PROVE { get; set; } = 0;
        //[ColumnaGridViewAtributo(Name = "Nombre proveedor")]
        //public string DES_PROVE { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Código cliente")]
        public int? CG_CLI { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Nombre cliente")]
        public string DES_CLI { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Dirección")]
        public string DIRECC { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Localidad")]
        public string LOCALIDAD { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Código postal")]
        public string CG_POSTA { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Orden compra cliente")]
        public string ORCO { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Cantidad pedida")]
        public decimal? CANTPED { get; set; } = 0;
        //[ColumnaGridViewAtributo(Name = "Flag")]
        //public int? FLAG { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Usuario")]
        public string USUARIO { get; set; } = "";
        [ColumnaGridViewAtributo(Name = "Fecha registro")]
        public DateTime FE_REG { get; set; }
        
        [ColumnaGridViewAtributo(Name = "Compañía")]
        public int? CG_CIA { get; set; } = 0;
        [ColumnaGridViewAtributo(Name = "Condicion de Entrega")]
        public int CG_COND_ENTREGA { get; set; } = 0;


        [Column("DPP"),ColumnaGridViewAtributo(Name = "Condicion de Entrega")]
        public int CG_CONDICION_PAGO { get; set; } = 0;
        public int CG_TRANS { get; set; } = 0;
        public bool Control1 { get; set; } = false;
        public bool Control2 { get; set; } = false;
        public bool Control3 { get; set; } = false;
        public bool Control4 { get; set; } = false;
        public bool Control5 { get; set; } = false;
        public bool Control6 { get; set; } = false;

        [Column("FLAG")]
        public decimal CONFIRMADO { get; set; }


        [NotMapped]
        public EstadoItem ESTADO { get; set; } = EstadoItem.Agregado;
    }
}
