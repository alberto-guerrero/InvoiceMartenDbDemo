using InvoiceMartenDbDemo.Events;

namespace InvoiceMartenDbDemo.Projections;

public class ClientDemographic
{
    public Guid ClientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }
    public string AccountNumber { get; set; } = string.Empty;

    public void Apply(ClientDemographicSave @event)
    {
        ClientId = @event.ClientId;
        FirstName = @event.FirstName;
        LastName = @event.LastName;
        AccountNumber = @event.AccountNumber;
        DateOfBirth = @event.DateOfBirth;
    }
}