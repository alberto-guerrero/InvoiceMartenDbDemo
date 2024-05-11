using Marten;
using InvoiceMartenDbDemo.Events;
using InvoiceMartenDbDemo.Projections;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMartenDbDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        [HttpPost(Name = "CreateClientDemographics")]
        public async Task<IActionResult> Create(ClientDemographicSave ev,IDocumentSession documentSession)
        {
            documentSession.Events.Append(ev.ClientId, ev);
            await documentSession.SaveChangesAsync();
            
            return Accepted();
        }

        [HttpGet("{id}", Name = "GetClientDemographics")]
        public async Task<ClientDemographic?> Get(Guid id, IQuerySession querySession)
        {
            return await querySession.Events.AggregateStreamAsync<ClientDemographic>(id);
        }

        [HttpGet(Name = "GetAllClientDemographics")]
        public IEnumerable<ClientDemographic> GetAll(IQuerySession querySession)
        {
            return querySession.Query<ClientDemographic>().ToList();
        }
    }
}