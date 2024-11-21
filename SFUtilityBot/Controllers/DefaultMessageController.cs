using Telegram.Bot;
using Telegram.Bot.Types;

namespace SFUtilityBot.Controllers;

public class DefaultMessageController
{
    private readonly ITelegramBotClient _telegramBotClient;

    public DefaultMessageController(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task Handle(Message message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Контроллер {GetType().Name} получил сообщение");
    }
}