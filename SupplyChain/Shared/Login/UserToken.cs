using System;

namespace SupplyChain.Shared.Login
{
    public class UserToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public byte[] Foto { get; set; }
    }
}
