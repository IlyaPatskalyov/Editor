using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;
using Editor.Front.DocumentSessions;

namespace Editor.Client.Connectors
{
    public abstract class ConnectorBase : IConnector
    {
        private readonly Action<DocumentState> onUpdateState;
        private readonly List<string[]> packs = new List<string[]>();
        private int position;
        private int revision;
        private int eventsInterval;
        private int y = 0;
        private int x = 0;

        public ConnectorBase(Action<DocumentState> onUpdateState)
        {
            this.onUpdateState = onUpdateState;
        }

        public virtual void Start()
        {
            eventsInterval = Window.SetInterval(Ping, 250);
        }

        public virtual void Stop()
        {
            Global.ClearInterval(eventsInterval);
        }

        protected abstract void Ping();

        protected DocumenChange BuildChanges()
        {
            var operations = Get().Take(10000).ToArray();
            return new DocumenChange
                   {
                       Operations = operations,
                       Position = position,
                       Revision = revision
                   };
        }

        protected void CancelChanges(DocumenChange change)
        {
        }

        protected void ProcessNewState(DocumentState state)
        {
            revision = state.Revision;
            onUpdateState(state);
        }

        public void SendPosition(int pos)
        {
            position = pos;
        }

        public void SendOperatinos(string[] operations)
        {
            packs.Add(operations);
        }

        public IEnumerable<string> Get()
        {
            while (y < packs.Count)
            {
                var pack = packs[y];
                while (x < pack.Length)
                {
                    yield return pack[x++];
                }
                x = 0;
                y++;
                if (y == packs.Count - 1)
                    break;
            }
        }
    }
}