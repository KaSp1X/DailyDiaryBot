using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
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

        public async Task SendMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Config.Instance.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else
                    try
                    {
                        var htmlMessage = message.ToHtml();
                        var result = await client.SendTextMessageAsync(Config.Instance.ChatId, htmlMessage, ParseMode.Html, replyToMessageId: Config.Instance.ReplyMessageId);
                        Messages.AddLastMessage(result.MessageId, message);
                    }
                    catch (RequestException ex)
                    {
                        Error.Instance.Message = ex.Message;
                    }
            }
        }

        public async Task EditPickedMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Config.Instance.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else
                    if (Messages.Instance.PickedMessage == null)
                        Error.Instance.Message = "Picked message not found";
                else
                    if (Messages.Instance.PickedMessage?.Text == message)
                        Error.Instance.Message = "You can't update not edited message";
                else
                    try
                    {
                        var htmlMessage = message.ToHtml();
                        var result = await client.EditMessageTextAsync(Config.Instance.ChatId, Messages.Instance.PickedMessage.Value.Id , htmlMessage, ParseMode.Html);
                        Messages.UpdateLastMessage(result.MessageId, message);
                    }
                    catch (RequestException ex) when (ex.Message == "Bad Request: message to edit not found")
                    {
                        Messages.RemoveMessage(Messages.Instance.PickedMessage);
                        Error.Instance.Message = ex.Message;
                    }
                    catch (RequestException ex)
                    {
                        Error.Instance.Message = ex.Message;
                    }
            }
        }
    }
}