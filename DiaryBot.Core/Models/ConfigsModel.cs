namespace DiaryBot.Core
{
    public sealed class ConfigsModel : Singleton<ConfigsModel>, IModel<Config>
    {
        public List<Config> Items { get; init; }
        public Config? SelectedItem { get; set; }

        private ConfigsModel()
        {
            Items = Serializer.Load<List<Config>>(GetPath()) ?? new();
            SelectedItem = Items.Count > 0 ? Items[^1] : null;
            if (string.IsNullOrWhiteSpace(SelectedItem?.Token))
                StatusModel.Instance.Message = "Create a new configuration profile to proceed futher";
        }

        public string GetPath() => "config.json";

        public void Add(Config newItem)
        {
            Items.Add(newItem);
            SelectedItem = Items[^1];
            Serializer.Save(GetPath(), Items);
        }

        public void Update(Config updatedItem)
        {
            int index = Items.IndexOf(SelectedItem);
            if (index != -1)
            {
                Items[index] = SelectedItem = updatedItem;
                Serializer.Save(GetPath(), Items);
            }
        }

        public void Remove()
        {
            Items.Remove(SelectedItem);
            SelectedItem = Items.Count > 0 ? Items[^1] : null;
            Serializer.Save(GetPath(), Items);
        }
    }
}