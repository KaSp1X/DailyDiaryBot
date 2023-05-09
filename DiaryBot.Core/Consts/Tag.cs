namespace DiaryBot.Core
{
    public static class Tag
    {
        public const string Bold = @"[\b]{message}[\b0]";
        public const string Italic = @"[\i]{message}[\i0]";
        public const string Spoiler = @"[\v]{message}[\v0]";
        public const string Underline = @"[\u]{message}[\u0]";
        public const string Strikethrough = @"[\s]{message}[\s0]";
    }
}
