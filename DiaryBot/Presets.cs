using System.Collections.Generic;

namespace DiaryBot
{
    public sealed class Presets : Singleton<Presets>, IRecordable<Preset>
    {
        public List<Preset> Items { get; init; }
        public Preset? SelectedItem { get; set; }

        private Presets() => Items = Serializer.Load<List<Preset>>(GetPath()) ?? new();

        public string GetPath() => "presets.json";

        public void Add(Preset newItem)
        {
            Items.Add(newItem);
            Serializer.Save(GetPath(), Items);
        }

        public void Update(Preset updatedItem)
        {
            int index = Items.IndexOf(SelectedItem);
            if (index != -1)
            {
                Items[index] = updatedItem;
                Serializer.Save(GetPath(), Items);
            }
        }

        public void Remove()
        {
            Items.Remove(SelectedItem);
            Serializer.Save(GetPath(), Items);
        }
    }
}
