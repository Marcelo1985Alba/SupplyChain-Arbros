using SupplyChain.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain
{
    [Table("TipoArea")]
	public class TipoArea : EntityBase<int>
	{
		[Key, Column("CG_TIPOAREA")]
		new public int Id { get; set; } = 0;
		public string DES_TIPOAREA { get; set; } = "";
		[NotMapped]
		public bool GUARDADO { get; set; }
		[NotMapped]
		public bool ESNUEVO { get; set; }
	}
}
