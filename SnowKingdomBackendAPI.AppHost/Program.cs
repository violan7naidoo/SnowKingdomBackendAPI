var builder = DistributedApplication.CreateBuilder(args);

// Define the Game Engine
var gameEngine = builder.AddProject<Projects.SnowKingdomBackendAPI_ApiService>("apiservice");

// Define the RGS
var rgsService = builder.AddProject<Projects.SnowKingdomBackendAPI_RGS>("rgsservice")
                        .WithReference(gameEngine);

// Define the Frontend with the CORRECT relative path
builder.AddNpmApp("frontend", "../../../FrontEnd", "dev") // <-- THIS IS THE FIX
       .WithReference(rgsService)
       .WithHttpEndpoint(env: "PORT");

builder.Build().Run();