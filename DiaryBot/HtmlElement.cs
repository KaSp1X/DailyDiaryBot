using System.Text.RegularExpressions;

namespace DiaryBot
{
    public static class HtmlElement
    {
        public const string Bold = @"_\b_{message}_\b0_";
        public const string Italic = @"_\i_{message}_\i0_";
        public const string Spoiler = @"_\v_{message}_\v0_";
        public const string Underline = @"_\u_{message}_\u0_";
        public const string Strikethrough = @"_\s_{message}_\s0_";

        public static string Insert(string text, string tag) => tag.Replace("{message}", text);

        public static string ToHtml(string text)
        {
            text = Regex.Replace(text, @"_\\b_(.*?)_\\b0_", @$"<b>$1</b>");
            text = Regex.Replace(text, @"_\\i_(.*?)_\\i0_", @$"<i>$1</i>");
            text = Regex.Replace(text, @"_\\u_(.*?)_\\u0_", @$"<u>$1</u>");
            text = Regex.Replace(text, @"_\\s_(.*?)_\\s0_", @$"<s>$1</s>");
            text = Regex.Replace(text, @"_\\v_(.*?)_\\v0_", @$"<span class=""tg-spoiler"">$1</span>");
            return text;
        }

        public static string ToXaml(string text)
        {
            text = Regex.Replace(text, @"_\\b_(.*?)_\\b0_", @$"<TextBlock FontWeight=""Bold"">$1</TextBlock>");
            text = Regex.Replace(text, @"_\\i_(.*?)_\\i0_", @$"<TextBlock FontStyle=""Italic"">$1</TextBlock>");
            text = Regex.Replace(text, @"_\\u_(.*?)_\\u0_", @$"<TextBlock TextDecorations=""Underline"">$1</TextBlock>");
            text = Regex.Replace(text, @"_\\s_(.*?)_\\s0_", @$"<TextBlock TextDecorations=""Strikethrough"">$1</TextBlock>");
            text = Regex.Replace(text, @"_\\v_(.*?)_\\v0_", @$"<TextBlock TextDecorations=""OverLine"">$1</TextBlock>");
            text = text.Replace("\r\n", "<LineBreak/>");
            return text;
        }
    }
}
