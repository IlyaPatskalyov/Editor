using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;
using Bridge.jQuery2;
using Editor.Client.Connectors;
using Editor.Front.DocumentSessions;
using Editor.Model;

namespace Editor.Client
{
    public class EditorPageController
    {
        private readonly Guid clientId = Guid.NewGuid();
        private readonly IConnector connector;

        private readonly Dictionary<Guid, EditorCursor> cursors = new Dictionary<Guid, EditorCursor>();
        private readonly IEditorString model;


        private jQuery textarea;

        public EditorPageController(string documentId)
        {
            jQuery.Document.On("ready", Run);
            model = new EditorString(clientId);
            connector = new WebSocketConnector(documentId, clientId, UpdateState);
        }

        private void Run()
        {
            textarea = jQuery.Select("#EditorTextArea");
            textarea
                .Focus(e => SavePosition())
                .MouseUp(e => SavePosition())
                .KeyUp(e =>
                       {
                           SavePosition();
                           var newValue = textarea.Val().Replace("\r", "");
                           connector.SendOperatinos(model.GenerateOperations(newValue));
                       });

            connector.Start();
        }


        private void SavePosition()
        {
            var position = textarea.Prop<int>("selectionStart");
            connector.SendPosition(position);
        }

        private void UpdateState(DocumentState state)
        {
            if (state.Operations.Length > 0)
            {
                model.ApplyOperations(state.Operations.ToArray());
                textarea.Val(model.ToString());
            }

            RenderCursors(state.Authors);
        }

        private void RenderCursors(Author[] authors)
        {
            var coordinates = EditorCursor.GetCoordinates((HTMLTextAreaElement) textarea["0"], authors);
            var lastPosition = coordinates.Keys.Max();

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

                    EditorCursorCoordinate coordinate;
                    if (!coordinates.TryGetValue(author.Position, out coordinate))
                        coordinate = coordinates[lastPosition];
                    cursor.Change(coordinate);
                }
            }
            var toDelete = cursors.Keys.Except(authors.Select(t => t.ClientId)).ToArray();
            foreach (var id in toDelete)
            {
                cursors[id].Remove();
                cursors.Remove(id);
            }
        }
    }
}