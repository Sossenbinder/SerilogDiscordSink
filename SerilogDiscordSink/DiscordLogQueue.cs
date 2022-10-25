using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Serilog.Events;

namespace SerilogDiscordSink
{
	public class DiscordLogQueue : IDisposable, IAsyncDisposable
	{
		private readonly SerilogDiscordOptions _options;
		private readonly Channel<LogEvent> _logEventChannel;
		private readonly Lazy<Task<DiscordSocketClient>> _discordClient;
		private readonly CancellationTokenSource _cts;
		
		public DiscordLogQueue(SerilogDiscordOptions options)
		{
			_options = options;
			_cts = new CancellationTokenSource();
			_logEventChannel = Channel.CreateUnbounded<LogEvent>();

			_discordClient = new Lazy<Task<DiscordSocketClient>>(CreateClient);

			_ = RunLogProcessorLoop();
		}

		public void Enqueue(LogEvent logEvent)
		{
			while (true)
			{
				if (_logEventChannel.Writer.TryWrite(logEvent))
				{
					return;
				}
			}
		}

		private async Task RunLogProcessorLoop()
		{
			var discordClient = await _discordClient.Value;
			var channel = (IMessageChannel) discordClient.GetChannel(_options.Channel);

			var reader = _logEventChannel.Reader;
			var cancellationToken = _cts.Token;

			while (!cancellationToken.IsCancellationRequested && await reader.WaitToReadAsync(cancellationToken))
			{
				try
				{
					var item = await reader.ReadAsync(cancellationToken);

					await channel.SendMessageAsync(item.MessageTemplate.Text);
				}
				catch (Exception exc)
				{

				}
			}


		}

		public void Dispose()
		{
			DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(15));
		}

		public async ValueTask DisposeAsync()
		{
			if (!_discordClient.IsValueCreated)
			{
				return;
			}

			_cts.Cancel();
			await (await _discordClient.Value).DisposeAsync();
		}

		private async Task<DiscordSocketClient> CreateClient()
		{
			var readyTcs = new TaskCompletionSource<object>();

			var discordClient = new DiscordSocketClient();
			discordClient.Ready += () =>
			{
				readyTcs.SetResult(null);
				return Task.CompletedTask;
			};

			await discordClient.LoginAsync(TokenType.Bot, _options.DiscordBotToken);
			await discordClient.StartAsync();

			await readyTcs.Task;

			return discordClient;
		}
	}
}
