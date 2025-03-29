using ExamationOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline.Helper
{
    public class ExamStatusService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval;

        public ExamStatusService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _checkInterval = TimeSpan.FromSeconds(20);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateExamStatuses();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task UpdateExamStatuses()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ExamOnlineContext>();
                var now = DateTime.Now;

                var examToCheck = dbContext.Exams.Where(e => 
                                                      e.IsDelete == false &&
                                                      ((now <= e.EndDate) || (e.EndDate.Value.AddHours(1) > now)))
                                                .ToList();

                foreach (var exam in examToCheck)
                {
                    if (exam.StartDate > now)
                    {
                        exam.StatusId = 3; // Updcoming
                    }
                    else if (exam.EndDate < now)
                    {
                        exam.StatusId = 1; // Ended
                    }
                    else
                    {
                        exam.StatusId = 2; // On going
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
