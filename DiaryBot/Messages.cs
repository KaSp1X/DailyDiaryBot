using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Telegram.Bot.Types;

namespace DiaryBot
{
    public class Messages
    {
        public struct Message
        {
            public int Id { get; set; }
            public string Text { get; set; }

            public Message(int Id, string Text)
            {
                this.Id = Id;
                this.Text = Text;
            }
        }

        private const string _path = "messages.json";

        private static Messages? _instance;

        public static Messages Instance
        {
            get
            {
                if (_instance == null)
                {
                    List<Message> ex = Serializer.Load<List<Message>>(_path) ?? new();
                    _instance = new Messages
                    {
                        _messagesList = ex,
                        PickedMessage = ex[0]
                    };
                }
                return _instance;
            }
        }

        private Messages() { }

        private List<Message> _messagesList;

        public List<Message> MessagesList { get { return _messagesList; } set { _messagesList = value; } }

        public Message? PickedMessage { get; set; }

        public Message? this[int index]
        {
            get
            {
                if (_messagesList.Count <= index || index < 0)
                {
                    return null;
                }
                return _messagesList[index];
            }
        }

        public static void AddLastMessage(int id, string text)
        {

            for (int i = Math.Min(4, Instance._messagesList.Count); i > 0; i--)
            {
                if (Instance._messagesList.Count == i && i < 5)
                    Instance._messagesList.Add(Instance._messagesList[i - 1]);
                else
                    Instance._messagesList[i] = Instance._messagesList[i - 1];
            }
            if (Instance._messagesList.Count == 0)
                Instance._messagesList.Add(new(id, text));
            else
                Instance._messagesList[0] = new(id, text);

            Instance.PickedMessage = Instance._messagesList[0];
            Serializer.Save(_path, Instance._messagesList);
        }

    }
}
