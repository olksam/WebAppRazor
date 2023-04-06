using WebApi.Data;

namespace WebApi.HostedServices {
    public class DatabaseCleanupJob : IHostedService {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseCleanupJob(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        private bool _run;

        private async Task Run() {
            while (_run) {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

                var todosCount = dbContext.TodoItems.Count();
                Console.WriteLine($"Total TODOS count: {todosCount}");
                await Task.Delay(3000);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            _run = true;
            Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _run = false;
            return Task.CompletedTask;
        }
    }
}
