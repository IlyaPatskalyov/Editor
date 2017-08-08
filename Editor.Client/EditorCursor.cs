using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using Editor.Front.DocumentSessions;

namespace Editor.Client
{
    public class EditorCursor
    {
        private static readonly string[] colors =
            {"#f44336", "#e91e63", "#9c27b0", "#673ab7", "#3f51b5", "#009688", "#4caf50", "#ff9800", "#ff5722", "#607d8b"};

        private readonly jQuery element;
        private readonly jQuery textarea;
        private readonly Action scrollHandler;
        private EditorCursorCoordinate coordinate;


        public EditorCursor(Guid clientId)
        {
            element = jQuery.Select("<div class='editor__cursor'></div>")
                            .Css("background-color", colors[Math.Abs(clientId.GetHashCode()) % colors.Length]);
            textarea = jQuery.Select("#EditorTextArea");

            scrollHandler = () => { Render(); };
            textarea
                .Scroll(scrollHandler)
                .Parent()
                .Prepend(element);
        }

        public void Change(EditorCursorCoordinate coordinate)
        {
            this.coordinate = coordinate;
            Render();
        }

        public void Render()
        {
            var left = coordinate.Left - textarea.ScrollLeft();
            var top = coordinate.Top - textarea.ScrollTop();

            if (0 <= left && left <= textarea.Width() &&
                0 <= top && top <= textarea.Height())
            {
                element
                    .Show()
                    .Css("left", coordinate.Left - textarea.ScrollLeft())
                    .Css("top", coordinate.Top - textarea.ScrollTop())
                    .Css("height", coordinate.Height);
            }
            else
                element.Hide();
        }


        public void Remove()
        {
            element.Remove();
            textarea.Unbind("scroll", scrollHandler);
        }

        private static readonly string[] propeties =
        {
            "boxSizing",
            "width",
            "height",
            "overflowX",
            "overflowY",

            "borderTopWidth",
            "borderRightWidth",
            "borderBottomWidth",
            "borderLeftWidth",

            "paddingTop",
            "paddingRight",
            "paddingBottom",
            "paddingLeft",

            "fontStyle",
            "fontVariant",
            "fontWeight",
            "fontStretch",
            "fontSize",
            "lineHeight",
            "fontFamily",

            "textAlign",
            "textTransform",
            "textIndent",
            "textDecoration",

            "letterSpacing",
            "wordSpacing"
        };

        private static HTMLElement div;

        public static Dictionary<int, EditorCursorCoordinate> GetCoordinates(HTMLTextAreaElement element, Author[] authors)
        {
            var value = element.Value + "_";
            div?.Remove();

            div = Document.CreateElement("div");
            Document.Body.AppendChild(div);

            var style = div.Style;
            var computed = Window.GetComputedStyle(element);
            style.WhiteSpace = WhiteSpace.PreWrap;
            style.WordWrap = "break-word";
            style.Position = Position.Absolute;
            //style.Visibility = Visibility.Hidden;

            foreach (var prop in propeties)
                style[prop] = computed[prop];


            var positions = authors.Select(a => a.Position)
                                   .Where(p => p < value.Length)
                                   .Concat(new[] {value.Length, value.Length + 1})
                                   .OrderBy()
                                   .Distinct()
                                   .ToArray();

            div.TextContent = value.Substring(0, positions[0]);

            var result = new Dictionary<int, EditorCursorCoordinate>();
            var j = 0;
            for (var i = 0; i < positions.Length - 1; i++)
            {
                var span = Document.CreateElement("span");
                span.Style.BackgroundColor = colors[j++ % colors.Length];
                span.TextContent = value.Substring(positions[i], positions[i + 1]);

                div.AppendChild(span);
                result[positions[i]] = new EditorCursorCoordinate
                                       {
                                           Top = span.OffsetTop + SafeIntParse(computed.BorderTopWidth),
                                           Left = span.OffsetLeft + SafeIntParse(computed.BorderLeftWidth),
                                           Height = SafeIntParse(computed.LineHeight)
                                       };
            }

            div.Remove();
            div = null;
            return result;
        }

        private static int SafeIntParse<T>(Union<string, T> value)
        {
            return SafeIntParse((string) value);
        }

        private static int SafeIntParse(string value)
        {
            return (int) double.Parse(Regex.Replace(value, "[^0-9.]", ""));
        }
    }
}