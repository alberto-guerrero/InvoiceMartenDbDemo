namespace InvoiceMartenDbDemo.Events;

public record InvoiceSubmitted
{
    public Guid InvoiceId { get; set; }
    public DateTime PostedDate { get; set; }
}