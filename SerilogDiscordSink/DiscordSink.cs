using Serilog.Core;
using Serilog.Events;

namespace SerilogDiscordSink
{
	public class DiscordSink : ILogEventSink
	{
		private readonly DiscordLogQueue _logQueue;

		public DiscordSink(SerilogDiscordOptions options)
		{
			_logQueue = new DiscordLogQueue(options, new DiscordLogMessageFactory());
		}

		public void Emit(LogEvent logEvent)
		{
			_logQueue.Enqueue(logEvent);
		}
	}
}
