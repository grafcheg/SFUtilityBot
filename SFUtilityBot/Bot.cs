using Microsoft.Extensions.Hosting;
using SFUtilityBot.Controllers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SFUtilityBot;

public class Bot : BackgroundService
{
    private ITelegramBotClient _telegramBotClient;

    private readonly TextMessageController _textMessageController;
    private readonly InlineKeyboardController _inlineKeyboardController;
    private readonly DefaultMessageController _defaultMessageController;

    public Bot(ITelegramBotClient telegramBotClient, TextMessageController textMessageController, InlineKeyboardController inlineKeyboardController, DefaultMessageController defaultMessageController)
    {
        _telegramBotClient = telegramBotClient;
        _textMessageController = textMessageController;
        _inlineKeyboardController = inlineKeyboardController;
        _defaultMessageController = defaultMessageController;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, new ReceiverOptions() { AllowedUpdates = {} }, cancellationToken: stoppingToken);
        Console.WriteLine("Бот запущен");
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        
        Console.WriteLine(errorMessage);
        Console.WriteLine("Ждём 10 секунд до повторной попытки");
        Thread.Sleep(10000);
        
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
            return;
        }

        if (update.Type == UpdateType.Message)
        {
            switch (update.Message!.Type)
            {
                case MessageType.Text:
                    await _textMessageController.Handle(update.Message!, cancellationToken);
                    return;
                
                default:
                    await _defaultMessageController.Handle(update.Message, cancellationToken);
                    return;
            }
        }
    }
}