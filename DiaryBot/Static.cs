namespace DiaryBot
{
    public static class Static
    {
        public const string ConfigPath = "config.json";
        private static Config? _config = null;

        public static Config Config
        {
            get
            {
                if (_config == null)
                {
                    _config = Serializer.Load<Config>(ConfigPath);
                    if (_config == null)
                    {
                        _config = new Config();
                        Serializer.Save(ConfigPath,_config);
                    }
                }
                return _config;
            }
        }
    }
}
