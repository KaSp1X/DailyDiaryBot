﻿using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace DiaryBot
{
    public sealed class Bot : Singleton<Bot>
    {
        public const int MaxTextLength = 4096;

        private readonly TelegramBotClient? client;

        public static long? GetToken() => Instance.client?.BotId;

        private Bot()
        {
            if (!string.IsNullOrWhiteSpace(Configs.Instance.SelectedItem?.Token))
                client = new TelegramBotClient(Configs.Instance.SelectedItem.Token);
        }

        public async Task SendMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Configs.Instance.SelectedItem?.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else
                {
                    try
                    {
                        var htmlMessage = message.ToHtml();
                        var result = await client.SendTextMessageAsync(Configs.Instance.SelectedItem.ChatId, 
                            htmlMessage, ParseMode.Html, 
                            replyToMessageId: Configs.Instance.SelectedItem.ReplyMessageId);
                        Messages.Instance.Add(new(result.MessageId, message));
                        Error.Instance.Message = "Success";
                    }
                    catch (RequestException ex)
                    {
                        Error.Instance.Message = ex.Message;
                    }
                }
            }
        }

        public async Task EditPickedMessage(string message)
        {
            if (client != null)
            {
                if (string.IsNullOrWhiteSpace(Configs.Instance.SelectedItem?.ChatId))
                    Error.Instance.Message = "Bad Request: chat not found";
                else if (Messages.Instance.SelectedItem == null)
                    Error.Instance.Message = "Picked message not found";
                else if (Messages.Instance.SelectedItem?.Text == message)
                    Error.Instance.Message = "You can't update not edited message";
                else
                {
                    try
                    {
                        var htmlMessage = message.ToHtml();
                        var result = await client.EditMessageTextAsync(Configs.Instance.SelectedItem.ChatId, Messages.Instance.SelectedItem.Id, htmlMessage, ParseMode.Html);
                        Messages.Instance.Update(new(result.MessageId, message));
                        Error.Instance.Message = "Success";
                    }
                    catch (RequestException ex) when (ex.Message == "Bad Request: message to edit not found")
                    {
                        Messages.Instance.Remove();
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
}