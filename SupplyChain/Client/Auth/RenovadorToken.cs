using System;
using System.Timers;

namespace SupplyChain.Client.Auth;

public class RenovadorToken : IDisposable
{
    private readonly ILoginServiceJWT loginServiceJWT;
    private Timer timer;

    public RenovadorToken(ILoginServiceJWT loginServiceJWT)
    {
        this.loginServiceJWT = loginServiceJWT;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    public void Iniciar()
    {
        timer = new Timer();
        timer.Interval = 1000 * 60 * 40; //40 minutos
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        loginServiceJWT.ManejarRenovacionToken();
    }
}