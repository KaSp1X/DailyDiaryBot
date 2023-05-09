namespace DiaryBot.Core
{
    public sealed class PresetsModel : Singleton<PresetsModel>, IModel<Preset>
    {
        public List<Preset> Items { get; init; }
        public Preset? SelectedItem { get; set; }

        private PresetsModel() => Items = Serializer.Load<List<Preset>>(GetPath()) ?? new();

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
