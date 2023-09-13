using SupplyChain.Shared.HelpersAtributo;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain
{
	[Table("NoConfor_ListaAcciones")]
	public class NoConformidadesListaAcciones
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Tipoaccion { get; set; } = 0;

		[Required(ErrorMessage = "Debe Indicar una Descripción")]
		[StringLength(50)]
		public string Texto { get; set; } = "";

	}
}

