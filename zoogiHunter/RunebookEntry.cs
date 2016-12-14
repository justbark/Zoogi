using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ScriptSDK;
using StealthAPI;
using ScriptSDK.Mobiles;
using ScriptSDK.Items;
using ScriptSDK.Gumps;
using ScriptSDK.Configuration;
using ScriptSDK.Attributes;
using ScriptSDK.ContextMenus;
using ScriptSDK.Data;
using ScriptSDK.Engines;
using ScriptSDK.Targets;
using ScriptSDK.Utils;

namespace zoogiHunter
{
    public class RunebookEntry
    {
        private readonly Runebook _owner;
        private readonly GumpButton _recall;
        private readonly GumpButton _gate;
        private readonly GumpButton _sacred;
        private readonly GumpButton _scroll;
        private readonly GumpButton _default;
        private readonly GumpButton _drop;
        public string Name { get; private set; }
        public string Location { get; private set; }
        public Map Map { get; private set; }
        public Point3D Position { get; private set; }

        internal RunebookEntry(Runebook owner, GumpButton recall, GumpButton gate, GumpButton sacred, GumpButton scroll, GumpButton def, GumpButton drop, string location, string name, int color)
        {
            _owner = owner;
            _recall = recall;
            _gate = gate;
            _sacred = sacred;
            _scroll = scroll;
            _default = def;
            _drop = drop;
            Name = name;
            Location = location;
            Map = Map.Internal;
            switch (color)
            {
                case 1102:
                    Map = Map.Malas;
                    break;
                case 81:
                    Map = Map.Felucca;
                    break;
                case 10:
                    Map = Map.Trammel;
                    break;
                case 1154:
                    Map = Map.Tokuno;
                    break;
                case 0:
                    Map = Map.TerMur;
                    break;
            }
            Position = Geometry.CoordsToPoint(Location, Map);
        }
        internal RunebookEntry(Runebook owner, GumpButton recall, GumpButton gate, GumpButton sacred, GumpButton scroll, GumpButton def, GumpButton drop, string name)
        {
            _owner = owner;
            _recall = recall;
            _gate = gate;
            _sacred = sacred;
            _scroll = scroll;
            _default = def;
            _drop = drop;
            Name = name;
        }

        public bool SetDefault()
        {
            return _owner.Open() && _default.Click();
        }

        public bool Recall()
        {
            return _owner.Open() && _recall.Click();
        }

        public bool Sacred()
        {
            return _owner.Open() && _sacred.Click();
        }

        public bool Gate()
        {
            return _owner.Open() && _gate.Click();
        }

        public bool UseScroll()
        {
            return _owner.Open() && _scroll.Click();
        }
        public bool DropRune()
        {
            return _owner.Open() && _drop.Click();
        }

    }
}
