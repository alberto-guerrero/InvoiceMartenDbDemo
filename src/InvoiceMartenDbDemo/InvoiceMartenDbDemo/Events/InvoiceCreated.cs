namespace InvoiceMartenDbDemo.Events;

public record InvoiceCreated
{
    public Guid InvoiceId { get; set; }
    public Guid ClientId { get; set; }
    public double Balance { get; set; }
}