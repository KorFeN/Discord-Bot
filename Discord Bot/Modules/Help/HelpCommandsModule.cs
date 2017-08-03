using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules.Help
{
    public class HelpCommandsModule : ModuleBase
    {
        static CommandService commandService;

        internal static Task Start(CommandService commands)
        {
            commandService = commands;
            return Task.CompletedTask;
        }

        [Command("help")]
        [Alias("commands")]
        [Summary("Shows all Commands")]
        public async Task Help()
        {
            var helpString = "Available Commands:\n" + string.Join("\n", commandService.Commands.Select(GetHelpForCommand));

            await ReplyAsync(helpString);
        }

        static string GetHelpForCommand(CommandInfo command)
        {
            string names = string.Join(", ", command.Aliases.Select(p => "/" + p));

            string parameters = string.Join(", ", command.Parameters.Select(p => p.Name));
            string commandline = $"{names}: {command.Summary}";
            string parameterDetails = string.Join("\n", command.Parameters.Select(GetHelpForParameter));

            if (command.Parameters.Count == 0)
            {
                return $"  {commandline}";
            }
            else
            {
                return $"  {commandline}\n{parameterDetails}";
            }
        }

        static string GetHelpForParameter(ParameterInfo parameter)
        {
            string name = parameter.IsOptional ? $"({parameter.Name})" : $"[{parameter.Name}]";

            return $"    {name}: {parameter.Summary}";
        }
    }
}
