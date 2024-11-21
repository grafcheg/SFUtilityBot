using SFUtilityBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SFUtilityBot.Controllers;

public class InlineKeyboardController
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IStorage _storage;

    public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage storage)
    {
        _telegramBotClient = telegramBotClient;
        _storage = storage;
    }

    public async Task Handle(CallbackQuery? callbackQuery, CancellationToken cancellationToken)
    {
        // Console.WriteLine($"Контроллер {GetType().Name} обнаружил нажатие на кнопку");
        
        if (callbackQuery?.Data == null)
            return;

        _storage.GetSession(callbackQuery.From.Id).TaskCode = callbackQuery.Data;

        string taskText = callbackQuery.Data switch
        {
            "1" => "Подсчёт количества знаков в сообщении. Отправьте сообщение с текстом в котором нужно посчитать количество знаков",
            "2" => "Подсчёт суммы чисел в сообщении. Отправьте сообщение с числами через пробел",
            _ => String.Empty
        };
        
        await _telegramBotClient.SendMessage(callbackQuery.From.Id, taskText, cancellationToken: cancellationToken);
    }
}