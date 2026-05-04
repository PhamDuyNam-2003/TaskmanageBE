using BE.Models;
using MongoDB.Driver;

namespace BE.Services.BackgroundServices;

public class TaskReminderWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskReminderWorker> _logger;

    public TaskReminderWorker(IServiceProvider serviceProvider, ILogger<TaskReminderWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker nhắc việc đã khởi động...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                    var tasksCollection = db.GetCollection<TaskItem>("Tasks");
                    var usersCollection = db.GetCollection<User>("Users");

                    var now = DateTime.UtcNow;

                    var filter = Builders<TaskItem>.Filter.And(
                        Builders<TaskItem>.Filter.Eq(t => t.IsReminderEnabled, true),
                        Builders<TaskItem>.Filter.Ne(t => t.Status, WorkStatus.Done),
                        Builders<TaskItem>.Filter.Eq(t => t.IsDeleted, false),
                        Builders<TaskItem>.Filter.Lte(t => t.ReminderTime, now)
                    );

                    var tasksToRemind = await tasksCollection.Find(filter).ToListAsync();

                    foreach (var task in tasksToRemind)
                    {
                        var user = await usersCollection.Find(u => u.Id == task.AssignedTo).FirstOrDefaultAsync();

                        if (user != null && !string.IsNullOrEmpty(user.FcmToken))
                        {
                            _logger.LogInformation($"--- ĐANG GỬI NOTI CHO: {user.Username} - Task: {task.Title} ---");

                            await SendPushNotification(user.FcmToken, task.Title);

                            var update = Builders<TaskItem>.Update.Set(t => t.IsReminderEnabled, false);
                            await tasksCollection.UpdateOneAsync(t => t.Id == task.Id, update);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi Worker: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task SendPushNotification(string fcmToken, string taskTitle)
    {
        await Task.CompletedTask;
    }
}