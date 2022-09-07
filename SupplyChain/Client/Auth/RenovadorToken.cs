using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace SupplyChain.Client.Auth
{
    public class RenovadorToken : IDisposable
    {
        Timer timer;
        private readonly ILoginServiceJWT loginServiceJWT;

        public RenovadorToken(ILoginServiceJWT loginServiceJWT)
        {
            this.loginServiceJWT = loginServiceJWT;
        }

        public void Iniciar()
        {
            timer = new Timer();
            timer.Interval = 1000 * 60 * 40;//40 minutos
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            loginServiceJWT.ManejarRenovacionToken();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
