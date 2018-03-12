using System;
using Gravity.Configuration;
using Gravity.Diagnostics.NLog;
using Gravity.Manager.ApplicationService;
using Gravity.Manager.Configuration;
using Gravity.Manager.Data;
using Gravity.Manager.Data.EF;
using Gravity.Manager.Data.EF.Repositories;
using Gravity.Manager.Domain;
using Gravity.Manager.Domain.Audits;
using Gravity.Manager.Domain.Organizations;
using Gravity.Manager.Service;
using Gravity.Manager.Service.Ldap;
using Gravity.Manager.Web.Application;
using Gravity.Runtime.Serialization;
using Gravity.Service;
using Gravity.Storage;
using Gravity.Storage.AmazonS3;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ILogger = Gravity.Diagnostics.ILogger;

namespace Gravity.Manager.Web
{
    internal static class DependencyBuilder
    {
        public static void Build(IServiceCollection services, IConfiguration configuration)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            // Built-in dependency container can not give us target type (Autofac has this, though in a non-trivial way).
            services.AddTransient(typeof(ILogger), _ => new NLogLogger(LogManager.GetLogger("default")));

            services.AddSingleton<ISettingsProvider, AppConfigSettingsProvider>();
            
            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<ISerializer, JsonNetSerializer>();

            // S3 client is thread safe, singleton is fine.
            services.Configure<S3Options>(configuration.GetSection(SettingKeys.S3ConnectionOptions));
            services.AddSingleton<IFileStorage, S3FileStorage>();
            services.AddSingleton<IOutputStorage, FileOutputStorage>();

            // Configure LdapConnectionOptions from settings
            services.Configure<LdapConnectionOptions>(configuration.GetSection(SettingKeys.LdapConnectionOptions));
            services.AddSingleton<IExternalAuthenticationProvider, LdapAuthenticationProvider>();
            services.AddSingleton<IUserStateWrapper, UserStateWrapper>();
            services.AddSingleton<IOperatingUserProvider, ApplicationOperatingUserProvider>();
            services.AddSingleton<ISigninManager, SigninManager>();
            
            // Add relational data services.
            services.AddDbContext<GravityManagerDbContext>((svc, opts) => 
                opts.UseMySql(svc.GetService<IConfiguration>().GetConnectionString(SettingKeys.GravityDbConnectionString)));
            
            services.AddScoped<IDiscoveryUnitOfWork, DiscoveryUnitOfWork>();
            services.AddScoped<IDiscoveryAppService, DiscoveryAppService>();
            
            services.AddScoped<IAuditUnitOfWork, AuditUnitOfWork>();
            services.AddScoped<IAuditAppService, AuditAppService>();

            // To register GravityManagerDbContext as DbContext
            services.AddScoped<DbContext, GravityManagerDbContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IMemberUnitOfWork, MemberUnitOfWork>();
            services.AddScoped<IMemberAppService, MemberAppService>();
        }
    }
}