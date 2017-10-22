using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Kralizek.AspNetCore.Metrics;
using Amazon.CloudWatch;
using Microsoft.Extensions.Configuration;

namespace MinimalCloudWatchSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            services.AddAWSService<IAmazonCloudWatch>();

            services.PersistMetricsOnCloudWatch(
                new CloudWatchMetric("Elapsed time by Server", MetricValues.ElapsedTime, MetricDimensions.MachineName) { Unit = StandardUnit.Milliseconds },
                new CloudWatchMetric("Elapsed time by HTTP method", MetricValues.ElapsedTime, MetricDimensions.HttpMethod) { Unit = StandardUnit.Milliseconds },
                new CloudWatchMetric("Elapsed time", MetricValues.ElapsedTime) { Unit = StandardUnit.Milliseconds }
            );

            services.Configure<CloudWatchMetricPersisterConfiguration>(cfg => cfg.Namespace = "Minimal sample Kralizek.AspNetCore.Metrics");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMetricsCollector();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
