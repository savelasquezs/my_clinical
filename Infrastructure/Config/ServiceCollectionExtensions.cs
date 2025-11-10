using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Clinica_Herramientas_2.Infrastructure.Adapters.Output.Persistence;
using Clinica_Herramientas_2.Application.Adapters.Input;

namespace Clinica_Herramientas_2.Infrastructure.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClinicaServices(this IServiceCollection services, string connectionString)
        {
            // Registrar DbContext como scoped
            services.AddDbContext<ClinicaDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Registrar PortsFactory como scoped
            services.AddScoped<PortsFactory>(sp =>
            {
                var dbContext = sp.GetRequiredService<ClinicaDbContext>();
                return new PortsFactory(dbContext);
            });

            // Registrar ConfigFactory como scoped
            services.AddScoped<ConfigFactory>(sp =>
            {
                var portsFactory = sp.GetRequiredService<PortsFactory>();
                return new ConfigFactory(portsFactory);
            });

            // Registrar cada Config como scoped
            services.AddScoped<AdminConfig>(sp =>
            {
                var configFactory = sp.GetRequiredService<ConfigFactory>();
                return configFactory.AdminConfig;
            });

            services.AddScoped<DoctorConfig>(sp =>
            {
                var configFactory = sp.GetRequiredService<ConfigFactory>();
                return configFactory.DoctorConfig;
            });

            services.AddScoped<NurseConfig>(sp =>
            {
                var configFactory = sp.GetRequiredService<ConfigFactory>();
                return configFactory.NurseConfig;
            });

            services.AddScoped<RRHHConfig>(sp =>
            {
                var configFactory = sp.GetRequiredService<ConfigFactory>();
                return configFactory.RRHHConfig;
            });

            services.AddScoped<SupportConfig>(sp =>
            {
                var configFactory = sp.GetRequiredService<ConfigFactory>();
                return configFactory.SupportConfig;
            });

            services.AddScoped<AuthConfig>(sp =>
            {
                var configFactory = sp.GetRequiredService<ConfigFactory>();
                return configFactory.AuthConfig;
            });

            // Registrar cada Inputs como scoped
            services.AddScoped<AdminInputs>(sp =>
            {
                var adminConfig = sp.GetRequiredService<AdminConfig>();
                return adminConfig.AdminInputs;
            });

            services.AddScoped<DoctorInputs>(sp =>
            {
                var doctorConfig = sp.GetRequiredService<DoctorConfig>();
                return doctorConfig.DoctorInputs;
            });

            services.AddScoped<NurseInputs>(sp =>
            {
                var nurseConfig = sp.GetRequiredService<NurseConfig>();
                return nurseConfig.NurseInputs;
            });

            services.AddScoped<RRHHInputs>(sp =>
            {
                var rrhhConfig = sp.GetRequiredService<RRHHConfig>();
                return rrhhConfig.RRHHInputs;
            });

            services.AddScoped<SupportInputs>(sp =>
            {
                var supportConfig = sp.GetRequiredService<SupportConfig>();
                return supportConfig.SupportInputs;
            });

            return services;
        }
    }
}

