namespace DiaryBot
{
    public static class HtmlElement
    {
        public const string Bold = "<b>{message}</b>";
        public const string Italic = "<i>{message}</i>";
        public const string Underline = "<u>{message}</u>";
        public const string Strikethrough = "<s>{message}</s>";
        public const string Spoiler = "<span class=\"tg-spoiler\">{message}</span>";

        public static string Insert(string text, string tag) => tag.Replace("{message}", text);

        public static string ToXaml(string text)
        {
            return text.Replace("<b>", "<TextBlock FontWeight=\"Bold\">")
                .Replace("\r\n", "<LineBreak/>")
                .Replace("</b>", "</TextBlock>")
                .Replace("<i>", "<TextBlock FontStyle=\"Italic\">")
                .Replace("</i>", "</TextBlock>")
                .Replace("<u>", "<TextBlock TextDecorations=\"Underline\">")
                .Replace("</u>", "</TextBlock>")
                .Replace("<s>", "<TextBlock TextDecorations=\"Strikethrough\">")
                .Replace("</s>", "</TextBlock>")
                .Replace("<span class=\"tg-spoiler\">", "<TextBlock TextDecorations=\"OverLine\">")
                .Replace("</span>", "</TextBlock>");
        }
    }
}
