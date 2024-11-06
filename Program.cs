using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
  private static TelegramBotClient botClient;
  private static string chatIdInput;
  static async Task Main(string[] args)
  {
    // Укажите здесь ваш токен, полученный от BotFather
    string token = "7798866614:AAEoJkGZYrswU1enokRFstGj8kVkhXWsk1M";
    botClient = new TelegramBotClient(token);


    // Начинаем получение обновлений
    var receiverOptions = new ReceiverOptions
    {
      AllowedUpdates = Array.Empty<UpdateType>() // Получаем все типы обновлений
    };

    await botClient.SetMyCommands(new[] {
      new BotCommand {Command = "start", Description = "Запустить бота"},
      new BotCommand {Command = "message", Description = "Получить сообщение"},
      new BotCommand { Command = "contact", Description = "Связаться с поддержкой"}
    });
    botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions);

    Console.WriteLine("Бот запущен. Отправьте сообщение боту, чтобы узнать ваш ChatId.");
    Console.ReadLine();


    if (!long.TryParse(chatIdInput, out long chatId))
    {
      Console.WriteLine("Неверный формат ChatId.");
      return;
    }

    Console.WriteLine("Бот готов отправлять сообщения. Введите текст для отправки:");

    while (true)
    {
      string messageText = Console.ReadLine();

      if (string.IsNullOrEmpty(messageText))
      {
        Console.WriteLine("Пустое сообщение, попробуйте еще раз.");
        continue;
      }

      await SendMessage(chatId, messageText);
    }
  }

  private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
  {
    if (update.Type == UpdateType.Message && update.Message != null)
    {
      var message = update.Message;
      long chatId = message.Chat.Id;
      Console.WriteLine($"Получено сообщение от ChatId: {chatId}");

      chatIdInput = chatId.ToString();
      var userName = message.From?.Username?.ToString();

      if (message.Entities != null && message.Entities[0].Type == MessageEntityType.BotCommand)
      {
        string command = message.Text.Split(' ')[0]; // Команда идет первой до пробела
        Console.WriteLine($"Получена команда: {command}");

        // Выполняем действия в зависимости от команды
        switch (command)
        {
          case "/start":
            await botClient.SendMessage(message.Chat.Id, "Добро пожаловать! Это команда /start.");
            break;
          case "/help":
            await botClient.SendMessage(message.Chat.Id, "Здесь вы можете получить помощь. Это команда /help.");
            break;
          default:
            await botClient.SendMessage(message.Chat.Id, $"Неизвестная команда: {command}");
            break;
        }
      }
      Console.WriteLine($"Получено сообщение от пользователя: {userName}");

      // return;
    }
  }

  private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
  {
    Console.WriteLine($"Ошибка: {exception.Message}");
    return Task.CompletedTask;
  }

  private static async Task SendMessage(long chatId, string message)
  {
    try
    {
      await botClient.SendMessage(
          chatId: chatId,
          text: message,
          parseMode: ParseMode.Markdown
      );
      Console.WriteLine($"Сообщение отправлено: {message}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
    }
  }
}
