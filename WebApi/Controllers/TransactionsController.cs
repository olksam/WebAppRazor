using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Quartz;

using WebApi.DTOs;
using WebApi.HostedServices;

namespace WebApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase {
        private readonly IScheduler _scheduler;

        public TransactionsController(IScheduler scheduler) {
            _scheduler = scheduler;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTransaction(CreateTransactionRequest request) {
            var job = JobBuilder.Create<TransactionProcessorJob>()
                .WithIdentity($"TransactionProcessorJob_{Guid.NewGuid()}")
                .Build();

            await _scheduler.AddJob(job, false);

            return Ok();
        }
    }
}
