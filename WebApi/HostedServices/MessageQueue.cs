using Microsoft.EntityFrameworkCore;

using WebApi.Data;
using WebApi.DTOs;

namespace WebApi.HostedServices {
    public class MessageQueue {
        // TODO: read about ChannelWriter<T> and ChannelReader<T>

        private readonly IServiceProvider _serviceProvider;

        public MessageQueue(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public async Task Enqueue(CreateTransactionRequest request) {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            dbContext.Transactions.Add(new Entities.Transaction {
                Data = request.Data,
                Status = Entities.TransactionStatus.Created
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task<Entities.Transaction?> Dequeue() {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            var transaction = await dbContext.Transactions
                .Where(t => t.Status == Entities.TransactionStatus.Created)
                .OrderBy(t => t.Id)
                .FirstOrDefaultAsync();

            transaction.Status = Entities.TransactionStatus.Processing;
            await dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task Acknowledge(int id) {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            var transaction = await dbContext.Transactions
                .FirstOrDefaultAsync(t => t.Status != Entities.TransactionStatus.Processing && t.Id == id);

            if (transaction is null) {
                return;
            }

            transaction.Status = Entities.TransactionStatus.Processed;
            await dbContext.SaveChangesAsync();
        }

        public async Task Abort(int id) {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            var transaction = await dbContext.Transactions
                .FirstOrDefaultAsync(t => t.Status != Entities.TransactionStatus.Processing && t.Id == id);

            if (transaction is null) {
                return;
            }

            transaction.Status = Entities.TransactionStatus.Aborted;
            await dbContext.SaveChangesAsync();
        }
    }
}
