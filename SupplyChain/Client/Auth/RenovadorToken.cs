using SupplyChain.Client.HelperService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SupplyChain.Client.Auth
{
    public class RenovadorToken : IDisposable
    {

        Timer timer;
        private readonly ILoginServiceJWT loginService;
        public RenovadorToken(ILoginServiceJWT loginService)
        {
            this.loginService = loginService;
        }

        public void Iniciar()
        {
            timer = new Timer();
            timer.Interval = 9000; // 5 segundos
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            loginService.ManejarRenovacionToken();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
