using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;
using Tawasol.Infrastructure.Identity;
using Tawasol.Infrastructure.Persistence;
using Tawasol.Infrastructure.Persistence.Repositories;
using Tawasol.Infrastructure.Services;

namespace Tawasol.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Register Identity for the infrastructure-specific ApplicationUser
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = false; // Emails are not used for login
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Register domain repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICaseRepository, CaseRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ICaseItemRepository, CaseItemRepository>();
            services.AddScoped<IInKindDonationRepository, InKindDonationRepository>();
            
            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register application services
            services.AddScoped<IFileService, LocalFileService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IFcmService, FcmService>();
            services.AddScoped<ICaseUpdateService, CaseUpdateService>();

            services.AddSignalR();

            return services;
        }
    }
}
