using System.Fabric;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AssignmentOptimizationService.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json.Converters;

namespace AssignmentOptimizationService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class AssignmentOptimizationFabricService : StatelessService
    {
        public AssignmentOptimizationFabricService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.Services.AddScoped<IAssignmentOptimization, AssignmentOptimizationEngine>();

                        builder.WebHost
                            .UseKestrel(opt =>
                            {
                                int port = serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port;
                                opt.Listen(IPAddress.IPv6Any, port, listenOptions =>
                                {
                                    listenOptions.UseHttps(GetCertificateFromStore()!);
                                });
                            })
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url);
                        
                        // Add services to the container.
                        builder.Services.AddControllers().AddNewtonsoftJson(jsonOptions =>
                        {
                            jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
                        });
                        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();
                        builder.Services.AddSwaggerGenNewtonsoftSupport();

                        var app = builder.Build();
                        
                        // Configure the HTTP request pipeline.
                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }
                        app.UseHttpsRedirection();
                        app.UseAuthorization();
                        app.MapControllers();

                        return app;
                    }))
            };
        }

        /// <summary>
        /// Finds the ASP .NET Core HTTPS development certificate in development environment. Update this method to use the appropriate certificate for production environment.
        /// </summary>
        /// <returns>Returns the ASP .NET Core HTTPS development certificate</returns>
        private static X509Certificate2? GetCertificateFromStore()
        {
            string? aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.Equals(aspNetCoreEnvironment, "Development", StringComparison.OrdinalIgnoreCase))
            {
                const string aspNetHttpsOid = "1.3.6.1.4.1.311.84.1.1";
                const string CNName = "CN=localhost";
                using (X509Store store = new(StoreName.My, StoreLocation.LocalMachine))
                {
                    store.Open(OpenFlags.ReadOnly);
                    var certCollection = store.Certificates;
                    var currentCerts = certCollection.Find(X509FindType.FindByExtension, aspNetHttpsOid, true);
                    currentCerts = currentCerts.Find(X509FindType.FindByIssuerDistinguishedName, CNName, true);
                    return currentCerts.Count == 0 ? null : currentCerts[0];
                }
            }
            else
            {
                throw new NotImplementedException("GetCertificateFromStore should be updated to retrieve the certificate for non Development environment");
            }
        }
    }
}
