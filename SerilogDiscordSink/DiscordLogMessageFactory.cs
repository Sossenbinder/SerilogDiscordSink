using System;
using Discord;
using Serilog.Events;

namespace SerilogDiscordSink
{
	public class DiscordLogMessageFactory
	{
		public Embed CreateMessage(LogEvent logEvent)
		{
			var logLevelIcon = GetIconForLogLevel(logEvent.Level);
			var embed = new EmbedBuilder
			{
				Title = $"{logLevelIcon} {logEvent.Level} {logLevelIcon}",
				Description = "Descr"
			};

			return embed.Build();
		}

		private static string GetIconForLogLevel(LogEventLevel logLevel)
		{
			switch (logLevel)
			{
				case LogEventLevel.Verbose:
					return "📢";
				case LogEventLevel.Debug:
					return "🐜";
				case LogEventLevel.Information:
					return "ℹ";
				case LogEventLevel.Warning:
					return "⚠";
				case LogEventLevel.Error:
					return "❌";
				case LogEventLevel.Fatal:
					return "☠️";
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}
}
