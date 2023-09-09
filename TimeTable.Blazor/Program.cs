using Radzen;
using TimeTable.Blazor.Interfaces;
using TimeTable.Blazor.Services;
using TimeTable.Domain;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSqlite<TimeTableDbContext>(builder.Configuration.GetConnectionString("TimetableConnection"), optionBuilder =>
            {
                optionBuilder.MigrationsAssembly("TimeTable.Blazor");
            });

            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IImportExportService, ImportExportService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<INotificationService, NotificationDataService>();
            builder.Services.AddScoped<ITimetableService, TimetableService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IApiService, ApiService>();

            builder.Services.Configure<List<ImportTemplate>>(builder.Configuration.GetSection("ImportTemplates"));

            builder.Services.AddHttpClient<IApiService, ApiService>((s, h) =>
            {
                h.BaseAddress = new Uri(builder.Configuration["ApiUrl"]);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    UseDefaultCredentials = true,
                    AllowAutoRedirect = false
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // the order is important
            app.UseAuthorization();

            app.MapControllers();
            app.UseHttpLogging();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}