using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace DiaryBot
{
    internal class Bot
    {
        public const int MaxTextLength = 4096;

        private static Bot? _instance;

        public static Bot Instance
        {
            get => _instance ??= new Bot();
            set => _instance = null;
        }

        private readonly TelegramBotClient? client;

        public static long? GetToken() => Instance.client?.BotId;

        private Bot()
        {
            if (!string.IsNullOrWhiteSpace(Configs.Instance.SelectedConfig.Token))
                client = new TelegramBotClient(Configs.Instance.SelectedConfig.Token);
        }

        public async Task SendMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Configs.Instance.SelectedConfig.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else
                    try
                    {
                        var htmlMessage = message.ToHtml();
                        var result = await client.SendTextMessageAsync(Configs.Instance.SelectedConfig.ChatId, htmlMessage, ParseMode.Html, replyToMessageId: Configs.Instance.SelectedConfig.ReplyMessageId);
                        Messages.AddLastMessage(result.MessageId, message);
                        Error.Instance.Message = "Success";
                    }
                    catch (RequestException ex)
                    {
                        Error.Instance.Message = ex.Message;
                        throw;
                    }
            }
        }

        public async Task EditPickedMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Configs.Instance.SelectedConfig.ChatId))
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
                        var result = await client.EditMessageTextAsync(Configs.Instance.SelectedConfig.ChatId, Messages.Instance.PickedMessage.Value.Id , htmlMessage, ParseMode.Html);
                        Messages.UpdateLastMessage(result.MessageId, message);
                        Error.Instance.Message = "Success";
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