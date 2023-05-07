using System;
using System.Collections.Generic;

namespace DiaryBot
{
    public sealed class Messages : Singleton<Messages>
    {
        private const string _path = "messages.json";

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

        private Messages() 
        { 
            MessagesList = Serializer.Load<List<Message>>(_path) ?? new();
            PickedMessage = MessagesList[0];
        }

        public List<Message> MessagesList { get; init; }

        public Message? PickedMessage { get; set; }

        public Message? this[int index]
        {
            get
            {
                if (index >= MessagesList.Count || index < 0)
                {
                    return null;
                }
                return MessagesList[index];
            }
        }

        public static void AddLastMessage(int id, string text)
        {

            for (int i = Math.Min(3, Instance.MessagesList.Count); i > 0; i--)
            {
                if (Instance.MessagesList.Count == i && i < 4)
                    Instance.MessagesList.Add(Instance.MessagesList[i - 1]);
                else
                    Instance.MessagesList[i] = Instance.MessagesList[i - 1];
            }
            if (Instance.MessagesList.Count == 0)
                Instance.MessagesList.Add(new(id, text));
            else
                Instance.MessagesList[0] = new(id, text);

            Instance.PickedMessage = Instance[0];
            Serializer.Save(_path, Instance.MessagesList);
        }

        public static void UpdateLastMessage(int id, string text)
        {
            int i = 0;
            for (; i < Instance.MessagesList.Count; i++)
            {
                if (Instance[i].Id == Instance.PickedMessage?.Id)
                {
                    break;
                }
            }

            for (int j = 0; j < i; j++)
            {
                (Instance.MessagesList[j + 1], Instance.MessagesList[j]) = (Instance.MessagesList[j], Instance.MessagesList[j + 1]);
            }

            Instance.PickedMessage = Instance.MessagesList[0] = new(id, text);
            Serializer.Save(_path, Instance.MessagesList);
        }

        public static void RemoveMessage(Message? pickedMessage)
        {
            int i = 0;
            for (; i < Instance.MessagesList.Count; i++)
            {
                if (Instance[i].Id == pickedMessage?.Id)
                {
                    Instance.MessagesList.Remove(pickedMessage ?? new());
                }
            }

            Instance.PickedMessage = Instance[0];
            Serializer.Save(_path, Instance.MessagesList);
        }
    }
}
