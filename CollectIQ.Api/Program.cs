
using CollectIQ.Api.Extensions;
using CollectIQ.Core.Models;
using CollectIQ.Service.Services;
using Microsoft.AspNetCore.Identity;

using NLog;
using NLog.Web;


namespace CollectIQ.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var logger = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();

            LoggerManager loggerManager = new LoggerManager(logger);



            try
            {
                logger.Debug("Application is starting up");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Application encountered a critical error");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }

            // Add services to the container.
            builder.Services.RegisterDependencies();
            builder.Services.ConfigureMapping();
            builder.Services.ConfigureLoggerService(builder.Configuration);
            builder.Services.ConfigureHttpContextAccessor(builder.Configuration);
            builder.Services.ConfigureEmailService(builder.Configuration);
            builder.Services.ConfigureSqlContext(builder.Configuration);
            builder.Services.ConfigureRepositoryManager();

            builder.Services.AddAuthentication();
            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureJWT(builder.Configuration);
            builder.Services.ConfigureCors();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureSwagger();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await CreateRoles(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var rolesManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await rolesManager.RoleExistsAsync(role))
                {
                    await rolesManager.CreateAsync(new Role { Name = role });
                }
            }

            //Create Admin User
            var adminUser = new User
            {
                UserName = "anthony@mrwrite.dev",
                Email = "anthony@mrwrite.dev",
                EmailConfirmed = true,
                FirstName = "Anthony",
                LastName = "Wright"
            };

            var adminPassword = "Admin@123";

            if (await userManager.FindByNameAsync(adminUser.UserName) == null)
            {
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

        }

    }
}
