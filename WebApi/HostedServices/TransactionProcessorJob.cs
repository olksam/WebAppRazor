using System.Threading.Channels;

using Quartz;

namespace WebApi.HostedServices {


    public class TransactionProcessorJob : IJob {
        private readonly ILogger<TransactionProcessorJob> _logger;

        public TransactionProcessorJob(ILogger<TransactionProcessorJob> logger) {
            _logger = logger;



        }

        public async Task Execute(IJobExecutionContext context) {

        }
    }
}