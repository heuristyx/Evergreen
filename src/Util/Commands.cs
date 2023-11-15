using System;
using System.Collections.Generic;
using System.Linq;

namespace Evergreen;

public static class Commands
{
  private static List<Command> commands = new List<Command>();

  /// <summary>
  /// Registers a command for use in the console.
  /// </summary>
  public static void RegisterCommand(string name, string description, int argumentCount, Action<string[]> action)
  {
    if (commands.Any(c => c.Name == name && c.ArgumentCount == argumentCount))
      Evergreen.Log.LogError($"Error registering command: command {name} with {argumentCount} arguments already exists.");

    commands.Add(
      new Command(name, description, argumentCount, action)
    );
  }

  internal static void SendCommand(string input)
  {
    if (TextDrawing.inputField.wasCanceled)
    {
      TextDrawing.inputField.DeactivateInputField();
      return;
    }
    TextDrawing.inputField.text = "";

    string[] args = input.Split(' ');
    string commandName = args[0];
    string[] commandArgs = args.Skip(1).ToArray();

    if (!commands.Any(c => c.Name == commandName))
    {
      TextDrawing.DrawToConsole($"Unknown command: {commandName}");
      return;
    }
    else if (!commands.Any(c => c.Name == commandName && c.ArgumentCount == commandArgs.Length))
    {
      TextDrawing.DrawToConsole($"Command {commandName} does not take {commandArgs.Length} arguments.");
      return;
    }

    var command = commands.Where(c => c.Name == commandName && c.ArgumentCount == commandArgs.Length).First();
    command.Action(commandArgs);
  }

  internal class Command
  {
    public string Name { get; }
    public string Description { get; }
    public int ArgumentCount { get; }
    public Action<string[]> Action { get; }

    public Command(string name, string description, int argumentCount, Action<string[]> action)
    {
      Name = name;
      Description = description;
      ArgumentCount = argumentCount;
      Action = action;
    }
  }
}