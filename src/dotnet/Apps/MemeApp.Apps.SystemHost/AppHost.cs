var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MemeApp_Apps_ServerHost>("memeapp-apps-serverhost");

builder.Build().Run();
