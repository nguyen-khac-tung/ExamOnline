using ExamationOnline.Helper;
using ExamationOnline.Models;
using ExamationOnline.Repository;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller}/{action}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection service, ConfigurationManager configuration)
        {
            service.AddDbContext<ExamOnlineContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            service.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(30);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });

            service.AddTransient<IUserRepository, UserRepository>();
            service.AddTransient<IQuestionRepository, QuestionRepository>();
            service.AddTransient<IExamRepository, ExamRepository>();
            service.AddTransient<IClassRepository, ClassRepository>();
            service.AddTransient<ISubjectRepository, SubjectRepository>();
            service.AddTransient<IStudentExamRepository, StudentExamRepository>();

            service.AddHostedService<ExamStatusService>();

            service.AddControllersWithViews();
        }
    }
}
