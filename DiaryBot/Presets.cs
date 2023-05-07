using System.Collections.Generic;

namespace DiaryBot
{
    public sealed class Presets : Singleton<Presets>
    {
        public record Preset(string Name, string Text);

        private const string _path = "presets.json";

        private Presets() 
        {
            PresetsList = Serializer.Load<List<Preset>>(_path) ?? new();
            SelectedPreset = new(string.Empty, string.Empty);
        }

        public List<Preset> PresetsList { get; init; }

        public Preset SelectedPreset { get; set; }

        public static void AddPreset(Preset newPreset)
        {
            Instance.PresetsList.Add(newPreset);
            Serializer.Save(_path, Instance.PresetsList);
        }

        public static void UpdatePreset(Preset selectedPreset, Preset updatedPreset)
        {
            int index = Instance.PresetsList.IndexOf(selectedPreset);
            if (index != -1)
            {
                Instance.PresetsList[index] = updatedPreset;
                Serializer.Save(_path, Instance.PresetsList);
            }
        }

        public static void RemovePreset(Preset selectedPreset)
        {
            int index = Instance.PresetsList.IndexOf(selectedPreset);
            if (index != -1)
            {
                Instance.PresetsList.RemoveAt(index);
                Serializer.Save(_path, Instance.PresetsList);
            }
        }
    }
}
