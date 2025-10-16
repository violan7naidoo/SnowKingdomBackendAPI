var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SnowKingdomBackendAPI_ApiService>("apiservice");

builder.Build().Run();
