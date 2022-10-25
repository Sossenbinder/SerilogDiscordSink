using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using SerilogDiscordSink;

var config = new ConfigurationBuilder()
	.AddUserSecrets<Program>()
	.Build();

var serviceProvider = new ServiceCollection()
	.Configure<SerilogDiscordOptions>(config)
	.BuildServiceProvider();

var discordOptions = serviceProvider.GetRequiredService<IOptions<SerilogDiscordOptions>>().Value;
var logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.Discord(new SerilogDiscordOptions()
	{
		Channel = discordOptions.Channel,
		DiscordBotToken = discordOptions.DiscordBotToken,
	})
	.CreateLogger();

logger.Information("Info");
logger.Error("Error");
logger.Warning("Warn");

await Task.Delay(-1);