namespace InvoiceMartenDbDemo.Events;

public record ClientDemographicSave
{
    public Guid ClientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
}