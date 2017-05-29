using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;
using Bridge.jQuery2;
using Editor.Front.DocumentSessions;
using Editor.Model;

namespace Editor.Client
{
    public class EditorPageController
    {
        private readonly string documentId;
        private readonly Guid clientId = Guid.NewGuid();
        private readonly List<Operation> buffer = new List<Operation>();
        private readonly Dictionary<Guid, EditorCursor> cursors = new Dictionary<Guid, EditorCursor>();
        private readonly IEditorString model;

        private int revision;
        private bool lockObject;
        private int position;

        private jQuery textarea;

        public EditorPageController(string documentId)
        {
            this.documentId = documentId;
            jQuery.Document.On("ready", Run);
            model = new EditorString(clientId);
        }

        private void Run()
        {
            textarea = jQuery.Select("#EditorTextArea");
            textarea
                .Focus(e => SavePosition())
                .Blur(e => { position = -1; })
                .MouseUp(e => SavePosition())
                .KeyUp(e =>
                       {
                           SavePosition();
                           Resize();

                           var newValue = textarea.Val().Replace("\r", "");
                           buffer.AddRange(model.GenerateOperations(newValue));
                       });

            Window.SetInterval(Ping, 250);
        }

        private void SavePosition()
        {
            position = textarea.Prop<int>("selectionStart");
        }

        private void Ping()
        {
            if (lockObject)
                return;
            lockObject = true;
            var operationToSend = buffer.ToArray();
            buffer.Clear();
            jQuery
                .Ajax(new AjaxOptions
                      {
                          Url = $"/api/editorapi?documentId={documentId}&clientId={clientId}&position={position}&revision={revision}",
                          ContentType = "application/json",
                          Type = "POST",
                          Data = JSON.Stringify(operationToSend)
                      })
                .Done(delegate(object data, string str, jqXHR jqXHR)
                      {
#pragma warning disable 618
                          var state = JSON.Parse<DocumentState>(jqXHR.ResponseText);
#pragma warning restore 618
                          revision = state.Revision;
                          if (state.Operations.Length > 0)
                          {
                              model.ApplyOperations(state.Operations);
                              textarea.Val(model.ToString());
                          }

                          RenderCursors(state.Authors);
                          Resize();
                      })
                .Fail(() => buffer.AddRange(operationToSend))
                .Always(() => { lockObject = false; });
        }

        private void RenderCursors(Author[] authors)
        {
            var inputValue = model.ToString();
            foreach (var author in authors)
            {
                author.ClientId = Guid.Parse(author.ClientId.ToString());
                if (author.ClientId != clientId)
                {
                    EditorCursor cursor;
                    if (!cursors.TryGetValue(author.ClientId, out cursor))
                    {
                        cursor = new EditorCursor(author.ClientId);
                        cursors.Add(author.ClientId, cursor);
                    }
                    cursor.Change(inputValue, author.Position);
                }
            }

            var toDelete = cursors.Keys.Except(authors.Select(t => t.ClientId)).ToArray();
            foreach (var id in toDelete)
            {
                cursors[id].Remove();
                cursors.Remove(id);
            }
        }

        private void Resize()
        {
            var oldScrollTop = jQuery.Document.ScrollTop();
            var oldScrollLeft = jQuery.Document.ScrollLeft();

            textarea.Height(5);
            textarea.Height(Math.Max(300, textarea.Prop<int>("scrollHeight")));
            textarea.Width(5);
            textarea.Width(Math.Max(800, textarea.Prop<int>("scrollWidth")));

            jQuery.Document.ScrollTop(oldScrollTop);
            jQuery.Document.ScrollLeft(oldScrollLeft);
        }
    }
}