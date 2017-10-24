using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Kralizek.AspNetCore.Metrics;
using Amazon.CloudWatch;

namespace SampleWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddMvcDimensions();

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            services.AddAWSService<IAmazonCloudWatch>();

            services.CollectDimensionsFromEC2InstanceMetadata();

            services.PersistMetricsOnCloudWatch(
                new CloudWatchMetric("Elapsed time by Server", MetricValues.ElapsedTime, MetricDimensions.MachineName) { Unit = StandardUnit.Milliseconds },
                new CloudWatchMetric("Elapsed time by Controller/Action", MetricValues.ElapsedTime, AspNetCoreMvcMetricDimensions.ControllerName, AspNetCoreMvcMetricDimensions.ActionName) { Unit = StandardUnit.Milliseconds },
                new CloudWatchMetric("Elapsed time by Action", MetricValues.ElapsedTime, AspNetCoreMvcMetricDimensions.Action) { Unit = StandardUnit.Milliseconds },
                new CloudWatchMetric("Elapsed time by HTTP method", MetricValues.ElapsedTime, MetricDimensions.HttpMethod) { Unit = StandardUnit.Milliseconds },
                new CloudWatchMetric("Elapsed time", MetricValues.ElapsedTime) { Unit = StandardUnit.Milliseconds },
                new CloudWatchMetric("Elapsed time by Controller/Action/HTTP status", MetricValues.ElapsedTime, AspNetCoreMvcMetricDimensions.ControllerName, AspNetCoreMvcMetricDimensions.ActionName, MetricDimensions.HttpResponseStatus) { Unit = StandardUnit.Milliseconds }
            );

            services.Configure<CloudWatchMetricPersisterConfiguration>(cfg => cfg.Namespace = "Sample Kralizek.AspNetCore.Metrics");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMetricsCollector();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
