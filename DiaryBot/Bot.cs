using System.Threading.Tasks;
using Telegram.Bot;

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
            if (Static.Config != null && !string.IsNullOrWhiteSpace(Static.Config.token))
                client = new TelegramBotClient(Static.Config.token);
            else
                client = null;
        }

        public async Task<Telegram.Bot.Types.Message?> SendMessage(string message)
        {
            if (client == null ||
                Static.Config == null ||
                string.IsNullOrWhiteSpace(Static.Config.chatId) ||
                string.IsNullOrWhiteSpace(message))
                return null;
            return await client.SendTextMessageAsync(Static.Config.chatId, message);
        }
    }
}
