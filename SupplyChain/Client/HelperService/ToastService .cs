using System;
using System.Timers;
using SupplyChain.Shared.Enum;

namespace SupplyChain.Client.HelperService;

public class ToastService : IDisposable
{
    private Timer Countdown;

    public void Dispose()
    {
        Countdown?.Dispose();
    }

    public event Action<string, TipoAlerta> OnShow;
    public event Action OnHide;

    public void ShowToast(string message, TipoAlerta level)
    {
        OnShow?.Invoke(message, level);
        StartCountdown();
    }

    private void StartCountdown()
    {
        SetCountdown();

        if (Countdown.Enabled)
        {
            Countdown.Stop();
            Countdown.Start();
        }
        else
        {
            Countdown.Start();
        }
    }

    private void SetCountdown()
    {
        if (Countdown == null)
        {
            Countdown = new Timer(2500);
            Countdown.Elapsed += HideToast;
            Countdown.AutoReset = false;
        }
    }

    private void HideToast(object source, ElapsedEventArgs args)
    {
        OnHide?.Invoke();
    }
}