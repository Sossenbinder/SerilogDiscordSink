using Serilog;
using Serilog.Configuration;

namespace SerilogDiscordSink
{
	public static class LoggerSinkConfigurationExtensions
	{
		public static LoggerConfiguration Discord(this LoggerSinkConfiguration loggerSinkConfiguration, SerilogDiscordOptions options)
		{
			return loggerSinkConfiguration.Sink(new DiscordSink(options));
		}
	}
}
