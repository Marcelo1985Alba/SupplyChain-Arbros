using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class vCalculoSolicitudes
    {

		public int SolicitudId { get; set; }
		public int PresupuestoId { get; set; }
		public string CodigoFinal { get; set; }
		public string Model { get; set; }
		public string DescripcionLinea { get; set; }
		public string Medida { get; set; }
		public string DescripcionMedida { get; set; }
		public string Orifico { get; set; }
		public string AreaOrificio { get; set; }
		public string SerieEntrada { get; set; }
		public string DescripcionSerieEntrada { get; set; }
		public string TipoEntrada { get; set; }
		public string DescripcionTipoEntrada { get; set; }
		public string SerieSalida { get; set; }
		public string DescripcionSerieSalida { get; set; }
		public string TipoSalida { get; set; }
		public string DescripcionTipoSalida { get; set; }
		public string CodigoDTT { get; set; }
		public string TipoAsiento { get; set; }
		public string DescripcionAsiento { get; set; }
		public string CodigoCBR { get; set; }
		public string Cuerpo_Externo { get; set; }
		public string Bonete_Externo { get; set; }
		public string Resorte_Externo { get; set; }
		public string CodigoDT { get; set; }
		public string Disco_Interno { get; set; }
		public string Tobera_Interno { get; set; }
		public string Capuchon { get; set; }
		public string Palanca { get; set; }
		public string Bonete { get; set; }
		public string Tornillo { get; set; }
		public int TagId { get; set; }
		public string Cantidad { get; set; }
		public string Servicio { get; set; }
		public string IndiceInstrumento { get; set; }
		public string NumeroPlanta { get; set; }
		public string NumeroPlano { get; set; }
		public string DescargaA { get; set; }
		public string LineaEntrada { get; set; }
		public string LineaSalida { get; set; }
		public string Contingencia { get; set; }
		public string NumeroHoja { get; set; }
		public string PresionOperacion { get; set; }
		public string TemperaturaOperacion { get; set; }
		public string PresionDiseño { get; set; }
		public string MaximaTemperaturaDiseño { get; set; }
		public string MinimaTemperaturaDiseño { get; set; }
		public string PresionAtmosfera { get; set; }
		public string DiscoRuptura { get; set; }
		public string Modelo { get; set; }
		public string Norma { get; set; }
		public string ContrapresionFija { get; set; }
		public string ContrapresionVariable { get; set; }
		public string ValvulaSimpleMultiple { get; set; }
		public string SobrepresionAdmisible { get; set; }
		public string PresionApertura { get; set; }
		public string CodigoDiseño { get; set; }
		public string UserId { get; set; }
		public string Nombre { get; set; }
		public int FaseId { get; set; }
		public string TipoFase { get; set; }
		public string NombreFase { get; set; }
		public string Descripcion { get; set; }
		public string CapacidadRequerida_V { get; set; }
		public string Temperatura_de_Descarga_T { get; set; }
		public string MasaMolecular_M { get; set; }
		public string CocienteCaloresEspecificos_k { get; set; }
		public string CoeficienteDeCompresibilidad_z { get; set; }
		public string CapacidadRequerida_W { get; set; }
		public string CapacidadRequerida_Q { get; set; }
		public string DensidadRelativa_G { get; set; }
		public string Viscusidad_Mu { get; set; }
		public string Temperatura_Saturado { get; set; }
		public decimal AreaCalculada { get; set; }
		public string Temperatura_de_Descarga_IN { get; set; }
		public string Temperatura_de_Descarga_IN_Unit { get; set; }
		public string CapacidadRequerida_IN { get; set; }
		public string CapacidadRequerida_IN_Unit { get; set; }
		public string Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string Cuit { get; set; }
}
}
