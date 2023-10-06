using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared
{
    public class ChatMessage : EntityBase<long>
    {
        public new long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public byte[] foto { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Visto { get; set; }
        public virtual ApplicationUser FromUser { get; set; }
        public virtual ApplicationUser ToUser { get; set; }

        [NotMapped]
        public string NameFromUser { get; set; }

        [NotMapped]
        public string NameToUser { get; set; }

    }
}
