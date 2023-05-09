using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace DiaryBot.Core
{
    public sealed class Bot : Singleton<Bot>
    {
        public const int MaxTextLength = 4096;

        private readonly TelegramBotClient? client;

        public static long? GetToken() => Instance.client?.BotId;

        private Bot()
        {
            if (!string.IsNullOrWhiteSpace(ConfigsModel.Instance.SelectedItem?.Token))
                client = new TelegramBotClient(ConfigsModel.Instance.SelectedItem.Token);
        }

        public async Task SendMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(ConfigsModel.Instance.SelectedItem?.ChatId))
                    StatusModel.Instance.Message = "Bad Request: chat not found";
                else
                {
                    try
                    {
                        var htmlMessage = message.ToHtml();
                        var result = await client.SendTextMessageAsync(chatId: ConfigsModel.Instance.SelectedItem.ChatId, 
                            text: htmlMessage, parseMode: ParseMode.Html, 
                            replyToMessageId: ConfigsModel.Instance.SelectedItem.ReplyMessageId);
                        MessagesModel.Instance.Add(new(result.MessageId, message));
                        StatusModel.Instance.Message = "Success";
                    }
                    catch (RequestException ex)
                    {
                        StatusModel.Instance.Message = ex.Message;
                    }
                }
            }
        }

        public async Task EditSelectedMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(ConfigsModel.Instance.SelectedItem?.ChatId))
                    StatusModel.Instance.Message = "Bad Request: chat not found";
                else if (MessagesModel.Instance.SelectedItem == null)
                    StatusModel.Instance.Message = "Picked message not found";
                else if (MessagesModel.Instance.SelectedItem?.Text == message)
                    StatusModel.Instance.Message = "You can't update not edited message";
                else
                {
                    try
                    {
                        var htmlMessage = message.ToHtml();
                        var result = await client.EditMessageTextAsync(ConfigsModel.Instance.SelectedItem.ChatId, MessagesModel.Instance.SelectedItem.Id, htmlMessage, ParseMode.Html);
                        MessagesModel.Instance.Update(new(result.MessageId, message));
                        StatusModel.Instance.Message = "Success";
                    }
                    catch (RequestException ex) when (ex.Message == "Bad Request: message to edit not found")
                    {
                        MessagesModel.Instance.Remove();
                        StatusModel.Instance.Message = ex.Message;
                    }
                    catch (RequestException ex)
                    {
                        StatusModel.Instance.Message = ex.Message;
                    }
                }
            }
        }
    }
}