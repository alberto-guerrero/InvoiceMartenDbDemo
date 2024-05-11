using Marten;
using InvoiceMartenDbDemo.Events;
using InvoiceMartenDbDemo.Projections;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMartenDbDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {

        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/invoices",Name = "CreateInvoice")]
        public async Task<IActionResult> CreateInvoice(InvoiceCreated @event,IDocumentSession documentSession)
        {
            documentSession.Events.Append(@event.InvoiceId, @event);
            await documentSession.SaveChangesAsync();
            
            return Accepted();
        }

        [HttpGet("/invoices/unbilled",Name = "UnBilledInvoicesGetAll")]
        public IEnumerable<UnBilledInvoice> UnBilledInvoicesGetAll(IQuerySession querySession)
        {
            return querySession.Query<UnBilledInvoice>().ToList();
        }

        [HttpGet("/invoices/billed", Name = "BilledInvoicesGetAll")]
        public IEnumerable<BilledInvoices> BilledInvoicesGetAll(IQuerySession querySession)
        {
            return querySession.Query<BilledInvoices>().ToList();
        }

        [HttpPost("/payment", Name = "CreatePayment")]
        public async Task<IActionResult> CreatePayment(PaymentApplied @event, IDocumentSession documentSession)
        {
            documentSession.Events.Append(@event.InvoiceId, @event);
            await documentSession.SaveChangesAsync();

            return Accepted();
        }

        [HttpPost("/invoice/submit", Name = "SubmitInvoice")]
        public async Task<IActionResult> SubmitInvoice(InvoiceSubmitted @event, IDocumentSession documentSession)
        {
            documentSession.Events.Append(@event.InvoiceId, @event);
            await documentSession.SaveChangesAsync();

            return Accepted();
        }

        [HttpGet("/client/ledger", Name = "Client Ledger")]
        public IEnumerable<ClientLedger> ClientLedgerGetAll(IQuerySession querySession)
        {
            return querySession.Query<ClientLedger>().ToList();
        }
    }
}