using System.Threading.Tasks;
using SupplyChain.Shared.Login;

namespace SupplyChain.Client.Auth;

public interface ILoginServiceJWT
{
    static readonly string TOKEN_KEY;
    Task Login(UserToken userToken);
    Task Logout();
    Task ManejarRenovacionToken();
}