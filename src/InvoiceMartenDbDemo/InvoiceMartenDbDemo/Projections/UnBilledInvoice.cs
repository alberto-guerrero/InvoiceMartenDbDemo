using Marten;
using Marten.Events.Projections;
using InvoiceMartenDbDemo.Events;

namespace InvoiceMartenDbDemo.Projections;

public class UnBilledInvoice
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientFirstName { get; set; } = string.Empty;
    public string ClientLastName { get; set; } = string.Empty;
    public string ClientAccountNumber { get; set; } = string.Empty;
    public double Balance { get; set; }
}

public class UnBilledInvoiceProjection : EventProjection
{
    public UnBilledInvoiceProjection()
    {
        ProjectAsync<InvoiceCreated>(OnInvoiceCreated);
        ProjectAsync<ClientDemographicSave>(OnClientDemographicsSave);
        ProjectAsync<InvoiceSubmitted>(OnInvoiceSubmitted);
    }

    private static Task OnInvoiceSubmitted(InvoiceSubmitted @event, IDocumentOperations ops)
    {
        ops.Delete<UnBilledInvoice>(@event.InvoiceId);
        return Task.CompletedTask;
    }

    private static async Task OnInvoiceCreated(InvoiceCreated @event, IDocumentOperations operations)
    {
        var clientDemographic = await operations
            .LoadAsync<ClientDemographic>(@event.ClientId);

        var projection = new UnBilledInvoice
        {
            Id = @event.InvoiceId,
            ClientId = @event.ClientId,
            ClientFirstName = clientDemographic!.FirstName,
            ClientLastName = clientDemographic.LastName,
            ClientAccountNumber = clientDemographic.AccountNumber,
            Balance = @event.Balance
        };

        operations.Store(projection);
    }

    /// <summary>
    /// Needed for future demographics updates
    /// </summary>
    /// <param name="event"></param>
    /// <param name="ops"></param>
    /// <returns></returns>
    private static Task OnClientDemographicsSave(ClientDemographicSave @event, IDocumentOperations ops)
    {
        var invoices = ops
                .Query<UnBilledInvoice>()
                .Where(o => o.ClientId == @event.ClientId);

        foreach (var invoice in invoices)
        {
            invoice.ClientFirstName = @event.FirstName;
            invoice.ClientLastName = @event.LastName;
            invoice.ClientAccountNumber = @event.AccountNumber;
            ops.Store(invoice);
        }

        return Task.CompletedTask;
    }
}