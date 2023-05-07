using System.Collections.Generic;

namespace DiaryBot
{
    public sealed class Configs : Singleton<Configs>, IRecordable<Config>
    {
        public List<Config> Items { get; init; }
        public Config? SelectedItem { get; set; }

        private Configs()
        {
            Items = Serializer.Load<List<Config>>(GetPath()) ?? new();
            SelectedItem = Items.Count > 0 ? Items[^1] : null;
            if (string.IsNullOrWhiteSpace(SelectedItem?.Token))
                Error.Instance.Message = "Create a new configuration profile to proceed futher";
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