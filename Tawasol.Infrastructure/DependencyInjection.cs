using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;
using Tawasol.Infrastructure.Identity;
using Tawasol.Infrastructure.Persistence;
using Tawasol.Infrastructure.Persistence.Repositories;
using Tawasol.Infrastructure.Services;

namespace Tawasol.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // services.AddDbContext<AppDbContext>(options =>
        //     options.UseSqlServer(connectionString,
        //         b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql( 
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
        
        // Use AddIdentity to register all required services, including SignInManager
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ICaseItemRepository, CaseItemRepository>();
        services.AddScoped<IInKindDonationRepository, InKindDonationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IFileService, LocalFileService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IFcmService, FcmService>();
        services.AddScoped<ICaseUpdateService, CaseUpdateService>();

        services.AddSignalR();

        return services;
    }
}
