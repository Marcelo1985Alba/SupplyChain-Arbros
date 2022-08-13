using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Shared.Login
{
    public class UserInfo
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public int Cg_Cli { get; set; } = 0;

    }
}