using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

[Command(Description = "Calculates the logarithm of a value.")]
public class LogCommand : ICommand
{
    // Order: 0
    [CommandParameter(0, Description = "Value whose logarithm is to be found.")]
    public required double Value { get; init; }

    // Name: --base
    // Short name: -b
    [CommandOption("base", 'b', Description = "Logarithm base.")]
    public double Base { get; init; } = 10;

    public ValueTask ExecuteAsync(IConsole console)
    {
        var result = Math.Log(Value, Base);
        console.Output.WriteLine(result);

        // If the execution is not meant to be asynchronous,
        // return an empty task at the end of the method.
        return default;
    }
}