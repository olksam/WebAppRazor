using Microsoft.EntityFrameworkCore;

using WebApi.Data;

namespace WebApi.HostedServices {
    public class ResetTransactionStatusBackgroundService : BackgroundService {
        private readonly IServiceProvider _serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while(!stoppingToken.IsCancellationRequested) {
                var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

                var pendingTransactions = await dbContext.Transactions
                    .Where(e => e.Status == Entities.TransactionStatus.Processing)
                    .ToListAsync();

                foreach (var transaction in pendingTransactions) {
                    transaction.Status = Entities.TransactionStatus.Created;
                }

                await dbContext.SaveChangesAsync();

                await Task.Delay(3000);
            }
        }
    }
}
