namespace DiaryBot.Core
{
    public sealed class MessagesModel : Singleton<MessagesModel>, IModel<Message>
    {
        private const int _messageLimit = 4;

        public List<Message> Items { get; init; }
        public Message? SelectedItem { get; set; }

        private MessagesModel() 
        { 
            Items = Serializer.Load<List<Message>>(GetPath()) ?? new();
            SelectedItem = this[0];
        }

        public string GetPath() => "messages.json";

        public Message? this[int index]
        {
            get
            {
                if (index >= Items.Count || index < 0)
                {
                    return null;
                }
                return Items[index];
            }
        }

        public void Add(Message newItem)
        {
            Items.Insert(0, newItem);
            if (Items.Count > _messageLimit)
                Items.RemoveAt(_messageLimit);
            SelectedItem = Items[0];
            Serializer.Save(GetPath(), Items);
        }

        public void Update(Message updatedItem)
        {
            Items.Remove(SelectedItem);
            Items.Insert(0, updatedItem);
            SelectedItem = updatedItem;
            Serializer.Save(GetPath(), Items);
        }

        public void Remove()
        {
            Items.Remove(SelectedItem);
            SelectedItem = this[0];
            Serializer.Save(GetPath(), Items);
        }
    }
}
