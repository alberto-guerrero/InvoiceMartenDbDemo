using Marten;
using InvoiceMartenDbDemo.Events;
using InvoiceMartenDbDemo.Projections;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMartenDbDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectionsController : ControllerBase
    {

        private readonly ILogger<ProjectionsController> _logger;

        public ProjectionsController(ILogger<ProjectionsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/rebuild",Name = "Rebuild Projections")]
        public async Task<IActionResult> RebuildProjections(IDocumentStore store)
        {

            var daemon = await store.BuildProjectionDaemonAsync();
            await daemon.RebuildProjectionAsync<UnBilledInvoice>(CancellationToken.None);
            await daemon.RebuildProjectionAsync<BilledInvoices>(CancellationToken.None);
            return Ok();
        }
    }
}