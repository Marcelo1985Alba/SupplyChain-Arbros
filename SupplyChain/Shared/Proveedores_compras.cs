using SupplyChain.Shared;
using SupplyChain.Shared.HelpersAtributo;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SupplyChain.Shared.Models
{
    [Table("Proveedores_compras")]
    public class Proveedores_compras 
	{
		[Key]
		public int NROCLTE { get; set; }
		public string DES_PROVE { get; set; }
	}
}
