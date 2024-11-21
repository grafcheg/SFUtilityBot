using SFUtilityBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SFUtilityBot.Controllers;

public class TextMessageController
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IStorage _storage;

    public TextMessageController(ITelegramBotClient telegramBotClient, IStorage storage)
    {
        _telegramBotClient = telegramBotClient;
        _storage = storage;
    }

    public async Task Handle(Message message, CancellationToken cancellationToken)
    {
        switch (message.Text)
        {
            case "/start":
                var buttons = new List<InlineKeyboardButton[]>();
                buttons.Add(new []
                {
                    InlineKeyboardButton.WithCallbackData("Подсчёт количества знаков в сообщении", "1"),
                    InlineKeyboardButton.WithCallbackData("Подсчёт суммы чисел в сообщении", "2")
                });
                
                await _telegramBotClient.SendMessage(message.Chat.Id, "Выберите опцию", cancellationToken: cancellationToken, parseMode: ParseMode.None, replyMarkup: new InlineKeyboardMarkup(buttons));
                break;
            
            default:
                switch (_storage.GetSession(message.Chat.Id).TaskCode)
                {
                    case "1":
                        await _telegramBotClient.SendMessage(message.From.Id, $"Длинна сообщения: {message.Text.Length} знаков", cancellationToken: cancellationToken);
                        break;
                    
                    case "2":
                        var result = message.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.TryParse(s.Trim(), out var n) ? (int?)n : null).Where(x => x != null)
                            .Select(i => i.Value).Sum().ToString();
                        await _telegramBotClient.SendMessage(message.From.Id, $"Сумма чисел в сообщении: {result}", cancellationToken: cancellationToken);
                        break;
                    
                    default:
                        await _telegramBotClient.SendMessage(message.From.Id, $"Ошибка", cancellationToken: cancellationToken);
                        break;
                }
                
                break;
        }
    }
}