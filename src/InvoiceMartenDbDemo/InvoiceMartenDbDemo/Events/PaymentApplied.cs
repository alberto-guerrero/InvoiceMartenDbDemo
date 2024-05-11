namespace InvoiceMartenDbDemo.Events;

public record PaymentApplied
{
    public Guid InvoiceId { get; set; }
    public double Amount { get; set; }
}