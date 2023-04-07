using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace DiaryBot
{
    internal class Bot
    {
        private static Bot? instance;

        public static Bot Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Bot();
                }
                return instance;
            }
        }

        private readonly TelegramBotClient? client;

        private Bot()
        {
            if (!string.IsNullOrWhiteSpace(Config.Instance.Token))
                client = new TelegramBotClient(Config.Instance.Token);
        }

        public Message LastMessage { get; set; }

        public async Task<Message?> SendMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Config.Instance.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else
                    try
                    {
                        return await client.SendTextMessageAsync(Config.Instance.ChatId, message);
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
