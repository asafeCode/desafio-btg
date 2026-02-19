namespace DesafioBtg.Domain.Entities;

public class User : EntityBase
{
    public string NationalId { get; set; } = string.Empty;
    public string AgencyNumber { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal PixLimit { get; set; }
}
