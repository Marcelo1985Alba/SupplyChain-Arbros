using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    [Table("Proporcion2")]
    public class Proporcion : EntityBase<int>
    {

        public int REGISTRO { get; set; } = 0;
        //public int ID { get; set; } = 0;
        public string PROPORC { get; set; } = "";
    }
}
