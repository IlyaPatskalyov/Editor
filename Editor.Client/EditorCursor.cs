using System;
using Bridge.jQuery2;

namespace Editor.Client
{
    public class EditorCursor
    {
        private static readonly string[] colors = {"#808080", "#FF0000", "#800000", "#008000", "#000080", "#800080", "#1A5276", "#873600"};
        private readonly jQuery element;

        public EditorCursor(Guid clientId)
        {
            element = jQuery.Select("<div class='editor__cursor'></div>")
                            .Css("background-color", colors[Math.Abs(clientId.GetHashCode()) % colors.Length]);

            jQuery.Select("#EditorTextArea")
                  .Parent()
                  .Prepend(element);
        }

        public void Change(string inputValue, int position)
        {
            int x = 0, y = 0;
            for (var i = 0; i < inputValue.Length; i++)
            {
                if (i == position)
                    break;
                if (inputValue[i] == '\n')
                {
                    y++;
                    x = -1;
                }
                x++;
            }

            element
                .Css("left", 16 + 11 * x)
                .Css("top", 18 + 28 * y);
        }

        public void Remove()
        {
            element.Remove();
        }
    }
}