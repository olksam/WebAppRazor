using Quartz;

using WebApi.Data;

namespace WebApi.HostedServices {
    public class DatabaseCleanupCronJob : IJob {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseCleanupCronJob(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context) {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            var todosCount = dbContext.TodoItems.Count();
            Console.WriteLine($"Total TODOS count: {todosCount}");
        }
    }
}
