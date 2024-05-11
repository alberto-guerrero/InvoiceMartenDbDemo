using Marten;
using Marten.Events.Projections;
using InvoiceMartenDbDemo.Events;

namespace InvoiceMartenDbDemo.Projections;

public class ClientLedger
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }
    public string AccountNumber { get; set; } = string.Empty;

    public List<Invoice> Invoices { get; set; }= new();

    public record Invoice
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
        public DateTime? BilledDate { get; set; }
        public List<Payment> Payments { get; set; } = new();
    }

    public record Payment(DateTime PaymentDate, double Amount);
}

public class ClientLedgerProjection : EventProjection
{
    public ClientLedgerProjection()
    {
        ProjectAsync<InvoiceCreated>(OnInvoiceCreated);
        ProjectAsync<InvoiceSubmitted>(OnInvoiceSubmitted);
        ProjectAsync<ClientDemographicSave>(OnClientDemographicsSave);
        ProjectAsync<PaymentApplied>(OnPaymentApplied);
    }

    private async Task OnInvoiceCreated(InvoiceCreated @event, IDocumentOperations operations)
    {
        var demographics = await operations
            .LoadAsync<ClientDemographic>(@event.ClientId);

        var ledger = await operations
            .LoadAsync<ClientLedger>(@event.ClientId);

        if (ledger == null)
        {
            ledger = new ClientLedger
            {
                Id = @event.InvoiceId,
                ClientId = @event.ClientId,
                FirstName = demographics!.FirstName,
                LastName = demographics.LastName,
                DateOfBirth = demographics.DateOfBirth,
                AccountNumber = demographics.AccountNumber,
                Invoices = new List<ClientLedger.Invoice>
                {
                    new()
                    {
                        Id = @event.InvoiceId,
                        Amount = @event.Balance,
                        Balance = @event.Balance
                    }
                }
            };
        }
        else
        {
            ledger.Invoices.Add(new()
            {
                Id = @event.InvoiceId,
                Amount = @event.Balance,
                Balance = @event.Balance
            });
        }
        
        operations.Store(ledger);
    }

    private Task OnPaymentApplied(PaymentApplied @event, IDocumentOperations operations)
    {
        // Find ledger
        var ledger = operations
            .Query<ClientLedger>()
            .First(o => o.Invoices.Any(c => c.Id == @event.InvoiceId));

        // Apply payment
        var invoice = ledger.Invoices.First(c => c.Id == @event.InvoiceId);
        invoice.Payments.Add(new(DateTime.Now, @event.Amount));
        invoice.Balance -= @event.Amount;

        // Save projection
        operations.Store(ledger);

        return Task.CompletedTask;
    }

    private Task OnInvoiceSubmitted(InvoiceSubmitted @event, IDocumentOperations operations)
    {
        // Find ledger
        var ledger = operations
            .Query<ClientLedger>()
            .First(o => o.Invoices.Any(c => c.Id == @event.InvoiceId));

        // Apply Invoice
        var invoice = ledger.Invoices.First(c => c.Id == @event.InvoiceId);
        invoice.BilledDate = @event.PostedDate;

        // Save projection
        operations.Store(ledger);

        return Task.CompletedTask;
    }

    private Task OnClientDemographicsSave(ClientDemographicSave @event, IDocumentOperations operations)
    {
        var ledgers = operations
            .Query<ClientLedger>()
            .Where(o => o.ClientId == @event.ClientId);

        foreach (var ledger in ledgers)
        {
            ledger.FirstName = @event.FirstName;
            ledger.LastName = @event.LastName;
            ledger.AccountNumber = @event.AccountNumber;
            operations.Store(ledger);
        }

        return Task.CompletedTask;
    }

    
}