using System.Collections.Generic;

namespace DiaryBot
{
    class Presets
    {
        public record Preset(string Name, string Text);

        private const string _path = "presets.json";

        private static Presets? instance;

        public static Presets Instance
        {
            get
            {
                if (instance == null)
                {
                    List<Preset> ex = Serializer.Load<List<Preset>>(_path) ?? new();
                    instance = new()
                    {
                        PresetsList = ex,
                        SelectedPreset = new(string.Empty, string.Empty)
                    };
                }
                return instance;
            }
        }

        private Presets() { }

        public List<Preset> PresetsList { get; set; }

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
