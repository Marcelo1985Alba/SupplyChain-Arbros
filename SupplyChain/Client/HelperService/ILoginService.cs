using System.Threading.Tasks;
using SupplyChain.Shared.Models;

namespace SupplyChain.Client.HelperService;

public interface ILoginService
{
    public Usuarios UsuarioLogin { get; set; }
    Task Login(Usuarios Usuario);
    Task Logout();
}