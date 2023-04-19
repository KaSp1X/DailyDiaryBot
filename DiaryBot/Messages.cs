using System;
using System.Collections.Generic;

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
                    _instance = new();
                    _instance._messagesList = ex;
                    _instance.PickedMessage = _instance[0];
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
                if (index >= _messagesList.Count || index < 0)
                {
                    return null;
                }
                return _messagesList[index];
            }
        }

        public static void AddLastMessage(int id, string text)
        {

            for (int i = Math.Min(3, Instance._messagesList.Count); i > 0; i--)
            {
                if (Instance._messagesList.Count == i && i < 4)
                    Instance._messagesList.Add(Instance._messagesList[i - 1]);
                else
                    Instance._messagesList[i] = Instance._messagesList[i - 1];
            }
            if (Instance._messagesList.Count == 0)
                Instance._messagesList.Add(new(id, text));
            else
                Instance._messagesList[0] = new(id, text);

            Instance.PickedMessage = Instance[0];
            Serializer.Save(_path, Instance._messagesList);
        }

        public static void UpdateLastMessage(int id, string text)
        {
            int i = 0;
            for (; i < Instance._messagesList.Count; i++)
            {
                if (Instance[i]?.Id == Instance.PickedMessage?.Id)
                {
                    break;
                }
            }

            for (int j = 0; j < i; j++)
            {
                (Instance._messagesList[j + 1], Instance._messagesList[j]) = (Instance._messagesList[j], Instance._messagesList[j + 1]);
            }

            Instance.PickedMessage = Instance._messagesList[0] = new(id, text);
            Serializer.Save(_path, Instance._messagesList);
        }

        public static void RemoveMessage(Message? pickedMessage)
        {
            int i = 0;
            for (; i < Instance._messagesList.Count; i++)
            {
                if (Instance[i]?.Id == pickedMessage?.Id)
                {
                    Instance.MessagesList.Remove(pickedMessage ?? new());
                }
            }

            Instance.PickedMessage = Instance[0];
            Serializer.Save(_path, Instance._messagesList);
        }
    }
}
