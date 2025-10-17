var builder = DistributedApplication.CreateBuilder(args);

// Define the Game Engine (your original ApiService)
var gameEngine = builder.AddProject<Projects.SnowKingdomBackendAPI_ApiService>("apiservice");

// Define the new RGS service and tell it how to find the Game Engine
var rgsService = builder.AddProject<Projects.SnowKingdomBackendAPI_RGS>("rgsservice")
                        .WithReference(gameEngine);

// Define the Frontend using AddNodeApp with the arguments passed as an array
builder.AddNodeApp("frontend", "npm", "../SnowKingdom6x4-FrontEnd", new string[] { "run", "dev" }) // <-- CORRECTED SYNTAX
       .WithReference(rgsService)
       .WithHttpsEndpoint(env: "PORT");

builder.Build().Run();