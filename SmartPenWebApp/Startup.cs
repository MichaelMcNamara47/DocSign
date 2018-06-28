using System;
using System.Collections.Generic;
using System.IO;//File upload, path/directory
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders; // File upload
using SmartSignWebApp.Services;

using Neosmartpen.Net.Protocol.v1;
using SmartSignWebApp.PenConnector;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SmartPenWebApp
{
    public class Startup
    {
        private readonly string EndpointUri = Environment.GetEnvironmentVariable("ENDPOINTURI");
        private readonly string PrimaryKey = Environment.GetEnvironmentVariable("PRIMARYKEY");
        private readonly string DatabaseName = Environment.GetEnvironmentVariable("DATABASEID");
        private readonly string CollectionName = Environment.GetEnvironmentVariable("COLLECTIONID");


        private DocumentClient client;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ASP.net core requires dependancy injection?

            /*
             Service for file upload (using IFileProvider
             */
            client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
            this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
            this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName });

            services.AddSingleton(client);
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddTransient<IMailService, SGMailService>();
            services.AddSingleton<PenCommV1Callbacks, PenConnector>();
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // This shows developer exception page while in development environment.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            /* Takes request url's and tries to find an index.html eg change :8888/ to :8888/index.html
            So, the order matters because static files needs a file to look for
            
            app.UseDefaultFiles();
            MVC has it's own defaults which replace this
            */

            // Serve static html files stored in wwwroot
            app.UseStaticFiles();

            /*
             Enable MVC: Listen to requests, try to map them to a controller.
             Configure the route
             */
            app.UseMvc(cfg =>
            {
                cfg.MapRoute(
                    "Default",
                    "/{controller}/{action}/{id?}",
                    new { controller = "App", Action = "Index" });
            });


        }
    }
}
