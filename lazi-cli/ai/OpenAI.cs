
// Import packages
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class OpenAICompletion
{
    // Populate values from your OpenAI deployment
    private readonly string modelId = "gpt-4o";
    private readonly string apiKey = Environment.GetEnvironmentVariable("OPENAPI_KEY")!;
    private readonly IChatCompletionService chatCompletionService;
    private readonly ChatHistory history;
    private readonly OpenAIPromptExecutionSettings openAIPromptExecutionSettings;
    private readonly Kernel kernel;

    public OpenAICompletion()
    {
        var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);
        // Add enterprise components
        builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

        // Build the kernel
        kernel = builder.Build();
        //// Add a plugin (the LightsPlugin class is defined below)
        //kernel.Plugins.AddFromType<LightsPlugin>("Lights");

        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Enable planning
        openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        // Create a history store the conversation
        history = new ChatHistory();
    }

    public async Task<string> GetChatMessageContentAsync(string userInput)
    {
        history.AddUserMessage(userInput);
        // Get the response from the AI
        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);
        var content = result.Content ?? string.Empty;
        history.AddMessage(result.Role, content);
        return content;
    }

}

