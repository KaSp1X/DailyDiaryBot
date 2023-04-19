using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace DiaryBot
{
    public static class FormattingTag
    {
        public const string Bold = @"[\b]{message}[\b0]";
        public const string Italic = @"[\i]{message}[\i0]";
        public const string Spoiler = @"[\v]{message}[\v0]";
        public const string Underline = @"[\u]{message}[\u0]";
        public const string Strikethrough = @"[\s]{message}[\s0]";

        public static (bool, string) Insert(this string fullText, string selectedText, int selectedStart, int selectedEnd, string tag)
        {
            // check if selectedText is already in needed tags
            bool hasTagOnLeft = false;
            bool hasTagOnRigth = false;

            for (int i = selectedStart - 1; i >= 4; i--)
            {
                if (fullText[..i].EndsWith(tag[..4]))
                {
                    hasTagOnLeft = true;
                    break;
                }
                else if (fullText[..i].EndsWith(tag[^5..]))
                    break;
            }

            for (int i = selectedEnd + 1; i < fullText.Length - 5; i++)
            {
                if (fullText[i..].StartsWith(tag[^5..]))
                {
                    hasTagOnRigth = true;
                    break;
                }
                else if (fullText[i..].StartsWith(tag[..4]))
                    break;
            }

            // if we detect only tag on one side, we will slide it to the start or the end of the selectedText
            // if there are detected tags from both sides, we return the original selectedText, there is no need to modify it
            if (hasTagOnLeft && hasTagOnRigth)
                return (false, selectedText);

            // tags are working bad with multilines, so we spliting selectedText and processing each line separately
            string[] textLines = selectedText.Split("\r\n");
            for (int i = 0; i < textLines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(textLines[i]))
                {
                    // if we detect both tags inside a selected line, we remove them as we are expanding a tag zone
                    if (textLines[i].Contains(tag[..4]) && textLines[i].Contains(tag[^5..]))
                        textLines[i] = tag.Replace("{message}", textLines[i].Replace(tag[..4], "").Replace(tag[^5..], ""));

                    // if we detect only a start tag inside a selected line, we remove it and add an end tag to the end of a selected line
                    else if (textLines[i].Contains(tag[..4]))
                        textLines[i] = tag[..^5].Replace("{message}", textLines[i].Replace(tag[..4], ""));

                    // if we detect only a end tag inside a selected line, we remove it and add an start tag to the start of a selected line
                    else if (textLines[i].Contains(tag[^5..]))
                        textLines[i] = tag[4..].Replace("{message}", textLines[i].Replace(tag[^5..], ""));
                    
                    // if there are no tags inside, we just add new tags
                    else
                        textLines[i] = tag.Replace("{message}", textLines[i]);
                }
            }
            // joining all lines and returning success of inserting
            return (true, string.Join("\r\n", textLines));
        }

        public static string ToHtml(this string text)
        {
            // we assume there are no same tags inside each other(e.g. [\b]some [\b]bold[\b0] text[\b0]),
            // cause we are processing it with Insert() method
            // although it doesn't save from selftyped tags
            // if there are, it will leave them as plain text
            text = Regex.Replace(text, @"\[\\b\](.*?)\[\\b0\]", @$"<b>$1</b>");
            text = Regex.Replace(text, @"\[\\i\](.*?)\[\\i0\]", @$"<i>$1</i>");
            text = Regex.Replace(text, @"\[\\u\](.*?)\[\\u0\]", @$"<u>$1</u>");
            text = Regex.Replace(text, @"\[\\s\](.*?)\[\\s0\]", @$"<s>$1</s>");
            text = Regex.Replace(text, @"\[\\v\](.*?)\[\\v0\]", @$"<span class=""tg-spoiler"">$1</span>");
            return text;
        }

        public static string ToXaml(this string text)
        {
            // we assume there are no same tags inside each other(e.g. [\b]some [\b]bold[\b0] text[\b0]),
            // cause we are processing it with Insert() method
            // although it doesn't save from selftyped tags
            // if there are, it will leave them as plain text
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
