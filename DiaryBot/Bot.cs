using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DiaryBot
{
    internal class Bot
    {
        private static Bot? _instance;

        public static Bot Instance
        {
            get => _instance ??= new Bot();
        }

        private readonly TelegramBotClient? client;

        private Bot()
        {
            if (!string.IsNullOrWhiteSpace(Config.Instance.Token))
                client = new TelegramBotClient(Config.Instance.Token);
        }

        public Telegram.Bot.Types.Message LastMessage { get; set; }

        public async Task<Telegram.Bot.Types.Message> SendMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Config.Instance.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else
                    try
                    {
                        return await client.SendTextMessageAsync(Config.Instance.ChatId, message, ParseMode.Html, replyToMessageId: Config.Instance.ReplyMessageId);
                    }
                    catch (RequestException ex)
                    {
                        Error.Instance.Message = ex.Message;
                    }
            }
            return null;
        }

        public async Task<Telegram.Bot.Types.Message> EditLastMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Config.Instance.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else
                    if (LastMessage == null)
                        Error.Instance.Message = "Last message not found";
                else
                    if (LastMessage.Text == message)
                        Error.Instance.Message = "You can't update not edited message";
                else
                    try
                    {
                        return await client.EditMessageTextAsync(Config.Instance.ChatId, LastMessage.MessageId, message, ParseMode.Html);
                    }
                    catch (RequestException ex)
                    {
                        Error.Instance.Message = ex.Message;
                    }
            }
            return null;
        }
    }
}
