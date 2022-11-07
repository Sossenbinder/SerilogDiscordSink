using System;
using System.Threading.Tasks;
using Serilog.Events;

namespace SerilogDiscordSink
{
	public class SerilogDiscordOptions
	{
		public ulong Channel { get; set; }

		public string DiscordBotToken { get; set; }

		public Func<Exception, LogEvent, Task> OnError { get; set; }
	}
}
