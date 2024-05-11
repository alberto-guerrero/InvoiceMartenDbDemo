using Marten;
using Marten.Events.Projections;
using InvoiceMartenDbDemo.Events;

namespace InvoiceMartenDbDemo.Projections;

public class BilledInvoices
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientFirstName { get; set; } = string.Empty;
    public string ClientLastName { get; set; } = string.Empty;
    public string ClientAccountNumber { get; set; } = string.Empty;
    public double Balance { get; set; }
    public DateTime BilledDate { get; set; }
}


public class BilledInvoiceProjection : EventProjection
{
    public BilledInvoiceProjection()
    {
        ProjectAsync<InvoiceSubmitted>(OnInvoiceSubmitted);
        ProjectAsync<ClientDemographicSave>(OnClientDemographicsSave);
        ProjectAsync<PaymentApplied>(OnPaymentApplied);
    }

    private static async Task OnInvoiceSubmitted(InvoiceSubmitted @event, IDocumentOperations operations)
    {
        var stream = await operations.Events.FetchStreamAsync(@event.InvoiceId);
        var createdInvoice = stream.Select(o => o.Data).OfType<InvoiceCreated>().First();
        var clientDemographic = await operations.LoadAsync<ClientDemographic>(createdInvoice.ClientId);

        var projection = new BilledInvoices
        {
            Id = @event.InvoiceId,
            ClientId = createdInvoice.ClientId,
            ClientFirstName = clientDemographic!.FirstName,
            ClientLastName = clientDemographic.LastName,
            ClientAccountNumber = clientDemographic.AccountNumber,
            Balance = createdInvoice.Balance,
            BilledDate = @event.PostedDate
        };

        operations.Store(projection);
    }

    private static async Task OnPaymentApplied(PaymentApplied @event, IDocumentOperations operations)
    {
        var projection = await operations.LoadAsync<BilledInvoices>(@event.InvoiceId);
        if (projection != null)
        {
            projection.Balance -= @event.Amount;
            operations.Store(projection);
        }
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
            .Query<BilledInvoices>()
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