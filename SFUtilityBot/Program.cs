using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFUtilityBot.Controllers;
using SFUtilityBot.Services;
using Telegram.Bot;

namespace SFUtilityBot;

static class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = Encoding.Unicode;
        
        var host = new HostBuilder().ConfigureServices((hostContext, services) => ConfigureServices(services)).UseConsoleLifetime().Build();
        
        Console.WriteLine("Staring service");
        await host.RunAsync();
        Console.WriteLine("Service stopped");
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<TextMessageController>();
        services.AddTransient<InlineKeyboardController>();
        services.AddTransient<DefaultMessageController>();

        services.AddSingleton<IStorage, MemoryStorage>();
        
        services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("7636522547:AAHtK7d0QMJCOOSBsCVl6C7sr_TmAAKogR0"));
        services.AddHostedService<Bot>();
    }
}