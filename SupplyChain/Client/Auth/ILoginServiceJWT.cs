using SupplyChain.Shared.Login;
using System.Threading.Tasks;

namespace SupplyChain.Client.Auth
{
    public interface ILoginServiceJWT
    {
        static readonly string TOKEN_KEY;
        Task Login(UserToken userToken);
        Task Logout();
        Task ManejarRenovacionToken();
    }
}