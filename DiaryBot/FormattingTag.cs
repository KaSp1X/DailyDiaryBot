using System.Text.RegularExpressions;

namespace DiaryBot
{
    public static class FormattingTag
    {
        public const string Bold = @"[\b]{message}[\b0]";
        public const string Italic = @"[\i]{message}[\i0]";
        public const string Spoiler = @"[\v]{message}[\v0]";
        public const string Underline = @"[\u]{message}[\u0]";
        public const string Strikethrough = @"[\s]{message}[\s0]";

        public static string Insert(string text, string tag) => tag.Replace("{message}", text);

        public static string ToHtml(string text)
        {
            text = Regex.Replace(text, @"\[\\b\](.*?)\[\\b0\]", @$"<b>$1</b>");
            text = Regex.Replace(text, @"\[\\i\](.*?)\[\\i0\]", @$"<i>$1</i>");
            text = Regex.Replace(text, @"\[\\u\](.*?)\[\\u0\]", @$"<u>$1</u>");
            text = Regex.Replace(text, @"\[\\s\](.*?)\[\\s0\]", @$"<s>$1</s>");
            text = Regex.Replace(text, @"\[\\v\](.*?)\[\\v0\]", @$"<span class=""tg-spoiler"">$1</span>");
            return text;
        }

        public static string ToXaml(string text)
        {
            text = Regex.Replace(text, @"\[\\b\](.*?)\[\\b0\]", @$"<TextBlock FontWeight=""Bold"">$1</TextBlock>");
            text = Regex.Replace(text, @"\[\\i\](.*?)\[\\i0\]", @$"<TextBlock FontStyle=""Italic"">$1</TextBlock>");
            text = Regex.Replace(text, @"\[\\u\](.*?)\[\\u0\]", @$"<TextBlock TextDecorations=""Underline"">$1</TextBlock>");
            text = Regex.Replace(text, @"\[\\s\](.*?)\[\\s0\]", @$"<TextBlock TextDecorations=""Strikethrough"">$1</TextBlock>");
            text = Regex.Replace(text, @"\[\\v\](.*?)\[\\v0\]", @$"<TextBlock TextDecorations=""OverLine"">$1</TextBlock>");
            text = text.Replace("\r\n", "<LineBreak/>");
            return text;
        }
    }
}
