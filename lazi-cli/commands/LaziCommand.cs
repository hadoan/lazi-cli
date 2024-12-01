using System.Text;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CliWrap;
using CliWrap.Buffered;

[Command(Description = "Lazi command for everyone")]
public class LaziCommand : ICommand
{
    private readonly OpenAICompletion completion;
    public LaziCommand()
    {
        completion = new OpenAICompletion();
    }
    // Order: 0
    [CommandParameter(0, Description = "Question for to ask for your commamnd (e.g list all containers in k8s).")]
    public required string Question { get; set; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var os = Environment.OSVersion;
        var prompt = $"cli command for {Question}, only output one single bash command cli for OS {os.Platform} in plain raw text, no markdown, instruction and explanation";
        var cliCommand = await completion.GetChatMessageContentAsync(prompt);
        console.Output.WriteLine($"Command: {cliCommand}");
        console.Output.WriteLine($"Type 'Y' or press Enter to proceed, 'I' to add more instructions, or any other key to cancel.");
        var confirm = console.ReadKey();

        if (confirm.Key == ConsoleKey.Y || confirm.Key == ConsoleKey.Enter)
        {
            await ProcessRunCommand(console, cliCommand);
        }
        else if (confirm.Key == ConsoleKey.I)
        {
            console.Output.WriteLine($"Your additional instruction: ....");
            var instruction = await console.Input.ReadLineAsync();
            if (instruction != null)
            {
                var cliWithInstruction = await completion.GetChatMessageContentAsync(instruction);
                await ProcessRunCommand(console, cliWithInstruction);
            }
        }
        else
        {
            console.Output.WriteLine("You chose not to continue.");
        }
    }

    private static async Task ProcessRunCommand(IConsole console, string cliCommand)
    {
        console.Output.WriteLine("Running your command ...");
        var commands = cliCommand.Split(" ");
        if (commands != null && commands.Length > 0)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            var cliCmd = Cli.Wrap(commands[0]);
            if (commands.Length > 1)
            {
                var args = commands.Skip(1).ToArray();
                cliCmd = Cli.Wrap(commands[0])
                        .WithArguments(cmdArgs =>
                        {
                            foreach (var arg in args)
                            {
                                console.Output.WriteLine($"Argument: {arg}");
                                cmdArgs.Add(arg);
                            }
                        });

            }
            console.Output.WriteLine($"Command: {cliCmd}");
            await cliCmd
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteBufferedAsync();

            // Access stdout & stderr buffered in-memory as strings
            var stdOut = stdOutBuffer.ToString();
            var stdErr = stdErrBuffer.ToString();

            if (!string.IsNullOrWhiteSpace(stdOut))
                console.Output.WriteLine($"Output: {stdOut}");
            else if (!string.IsNullOrWhiteSpace(stdErr))
            {
                console.Output.WriteLine($"Error: {stdOut}");
            }
        }
    }
}