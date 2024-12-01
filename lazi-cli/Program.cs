using CliFx;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();