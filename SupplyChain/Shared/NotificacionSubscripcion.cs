namespace SupplyChain.Shared;

public class NotificacionSubscripcion : EntityBase<int>
{
    public string UserId { get; set; }
    public string Url { get; set; }
    public string P256dh { get; set; }
    public string Auth { get; set; }
}