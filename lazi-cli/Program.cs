using CliFx;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello Lazi people! Ask ChatGPT about your command and we execute it for you.");

await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
