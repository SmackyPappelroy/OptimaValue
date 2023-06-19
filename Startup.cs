// Startup.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MQTTnet.AspNetCore;
using OptimaValue.API.Models;
using MQTTnet.Server;
using MQTTnet.Client;
using System;
using System.Linq;
using Mqtt;
using System.Threading.Tasks;

namespace OptimaValue;
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();

        //AddMqtt(services);

        services.AddControllers();


        // Add Swagger services
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlcAPI", Version = "v1" });
        });

        services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
        });
    }

    private async Task AddMqtt(IServiceCollection services)
    {
        services.AddSingleton<MqttService>();
        // Start MqttService
        var mqttService = services.BuildServiceProvider().GetRequiredService<MqttService>();
        await mqttService.InitializeMqttAsync("localhost", "mainclient");
    }


    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();


        // Add CORS middleware
        app.UseCors("AllowAllOrigins");

        // Add Swagger middleware
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlcAPI v1");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }



}
