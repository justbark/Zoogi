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
    [QuerySearch(0x22C5)]
    public class Runebook : Item
    {
        public override int DefaultLabelNumber
        {
            get { return 1041267; }
        }
        public int DefaultRune { get; private set; }
        public List<RunebookEntry> Entries { get; private set; }
        public Range Uses { get; private set; }
        public string Description
        {
            get
            {
                return !ClilocHelper.Contains(Properties, 1042971) ? string.Empty : ClilocHelper.GetParams(Properties, 1042971)[0];
            }
        }
        public string Crafter
        {
            get
            {
                return !ClilocHelper.Contains(Properties, 1050043) ? string.Empty : ClilocHelper.GetParams(Properties, 1050043)[0];
            }
        }
        public bool Exceptional
        {
            get
            {
                return (ClilocHelper.Contains(Properties, 1060636));
            }
        }
        public RuneBookConfig Configuration { get; set; }
        private uint GumpType { get; set; }
        private ushort GumpDelay { get; set; }
        private string Shard { get; set; }

        private GumpButton RenameButton { get; set; }

        public Runebook(Serial serial, RuneBookConfig config)
            : base(serial)
        {
            GumpType = 0x554B87F3;
            GumpDelay = 1000;
            Entries = new List<RunebookEntry>();
            Uses = new Range(0, 0);
            DefaultRune = -1;
            Configuration = config;
            Shard = "FS";
        }

        public Runebook(Serial serial, RuneBookConfig config, string osi)
            : base(serial)
        {
            GumpType = 0x0059;
            GumpDelay = 1000;
            Entries = new List<RunebookEntry>();
            Uses = new Range(0, 0);
            DefaultRune = -1;
            Configuration = config;
            Shard = osi;
        }
        public Runebook(uint value, RuneBookConfig config)
            : this(new Serial(value), config)
        { }
        public Runebook(uint value, RuneBookConfig config, string osi)
            : this(new Serial(value), config, osi)
        { }

        public bool Parse()
        {
            if (!Close() || !Open())
                return false;

            var g = Gump.GetGump(GumpType);

            UpdateLocalizedProperties();

            int scrolloffset = Configuration.ScrollOffset;
            int dropoffset = Configuration.DropOffset;
            int defaultoffset = Configuration.DefaultOffset;
            int recalloffset = Configuration.RecallOffset;
            int gateoffset = Configuration.GateOffset;
            int sacredoffset = Configuration.SacredOffset;
            int jumper = Configuration.Jumper;

            dynamic min = string.Empty;
            dynamic max = string.Empty;

            if (g.HTMLTexts.Count > 0)
                min = g.HTMLTexts[0].Text;
            if (g.HTMLTexts.Count > 1)
                max = g.HTMLTexts[1].Text;

            int imin;
            if (!int.TryParse(min, out imin))
                imin = -1;
            int imax;
            if (!int.TryParse(max, out imax))
                imax = -1;

            Uses = new Range(imin, imax);
            if (Shard == "OSI")
            {
                int eamt = 0;
                for (var i = 0; i < 15; i++)
                {
                    if (g.Buttons.Any(
                            e =>
                                e.PacketValue.Equals(defaultoffset + (i)) && e.Graphic.Released.Equals(2361) &&
                                e.Graphic.Pressed.Equals(2361)))
                        eamt++;
                }

                var namepos = (eamt);

                var defaultbtn = g.Buttons.First(e => e.Graphic.Released.Equals(2361) && e.Graphic.Pressed.Equals(2361));
                if (defaultbtn != null)
                {
                    var value = ((defaultbtn.PacketValue - defaultoffset) / jumper) + 1;
                    DefaultRune = value;
                }

                var rButton = g.Buttons.First(e => e.Graphic.Released.Equals(2472) || e.Graphic.Pressed.Equals(2473));
                RenameButton = rButton;

                for (var i = 0; i < 15; i++)
                {
                    var valid =
                        g.Buttons.Any(
                            e =>
                                e.PacketValue.Equals(defaultoffset + (i)) && e.Graphic.Released.Equals(2361) &&
                                e.Graphic.Pressed.Equals(2361));

                    if (!valid) continue;
                    var recallButton = g.Buttons.First(e => e.PacketValue.Equals(recalloffset + (i)));
                    var gateButton = g.Buttons.First(e => e.PacketValue.Equals(gateoffset + (i)));
                    var sacredButton = g.Buttons.First(e => e.PacketValue.Equals(sacredoffset + (i)));
                    var dropButton = g.Buttons.First(e => e.PacketValue.Equals(dropoffset + (i)));
                    var scrollButton = g.Buttons.First(e => e.PacketValue.Equals(scrolloffset + (i)));
                    var defaultButton = g.Buttons.First(e => e.PacketValue.Equals(defaultoffset + (i)));
                    var name = g.Labels[namepos].Text;

                    Entries.Add(new RunebookEntry(this, recallButton, gateButton, sacredButton, scrollButton,
                            defaultButton, dropButton, name));
                }
            }
            else
            {
                int eamt = 0;
                for (var i = 0; i < 15; i++)
                {
                    if (g.Buttons.Any(e => e.PacketValue.Equals(recalloffset + (i * jumper)) &&
                                           e.Graphic.Released.Equals(2103) &&
                                           e.Graphic.Pressed.Equals(2104)))
                        eamt++;
                }

                var namepos = (eamt * 2);

                var defaultbtn = g.Buttons.First(e => e.Graphic.Released.Equals(2361) && e.Graphic.Pressed.Equals(2361));
                if (defaultbtn != null)
                {
                    var value = ((defaultbtn.PacketValue - defaultoffset) / jumper) + 1;
                    DefaultRune = value;
                }

                var rButton = g.Buttons.First(e => e.Graphic.Released.Equals(2472) || e.Graphic.Pressed.Equals(2473));
                RenameButton = rButton;

                for (var i = 0; i < 15; i++)
                {
                    var valid =
                        g.Buttons.Any(
                            e =>
                                e.PacketValue.Equals(recalloffset + (i * jumper)) && e.Graphic.Released.Equals(2103) &&
                                e.Graphic.Pressed.Equals(2104));

                    if (!valid) continue;
                    var recallButton = g.Buttons.First(e => e.PacketValue.Equals(recalloffset + (i * jumper)));
                    var gateButton = g.Buttons.First(e => e.PacketValue.Equals(gateoffset + (i * jumper)));
                    var sacredButton = g.Buttons.First(e => e.PacketValue.Equals(sacredoffset + (i * jumper)));
                    var dropButton = g.Buttons.First(e => e.PacketValue.Equals(dropoffset + (i * jumper)));
                    var scrollButton = g.Buttons.First(e => e.PacketValue.Equals(scrolloffset + (i * jumper)));
                    var defaultButton = g.Buttons.First(e => e.PacketValue.Equals(defaultoffset + (i * jumper)));

                    var a = g.Labels[i + (i * 1)].Text;
                    var b = g.Labels[i + (i * 1) + 1].Text;
                    var full = a + " " + b;
                    var name = g.Labels[namepos + i].Text;
                    var color = g.Labels[namepos + i].Color;

                    Entries.Add(new RunebookEntry(this, recallButton, gateButton, sacredButton, scrollButton,
                        defaultButton, dropButton, full, name, color));
                }
            }

            return true;

        }
        public bool Rename()
        {
            return Open() && RenameButton != null && RenameButton.Click();
        }
        public bool Recall()
        {
            return Stealth.Client.CastSpellToObj("Recall", Serial.Value);
        }
        public bool Sacred()
        {
            return Stealth.Client.CastSpellToObj("Sacred Journey", Serial.Value);
        }
        public bool Gate()
        {
            return Stealth.Client.CastSpellToObj("Gate Travel", Serial.Value);
        }
        public bool Close()
        {
            Gump g = Gump.GetGump(GumpType);
            if (g != null)
                if (!g.Serial.Equals(0))
                    g.Close();
            Stealth.Client.Wait(1000);
            if (!Gump.WaitForGumpClose(GumpType, GumpDelay))
                return false;
            return true;
        }
        public bool Open()
        {
            if (!Close())
                return false;

            if (!DoubleClick() || !Gump.WaitForGump(GumpType, GumpDelay))
                return false;
            return true;
        }
    }

    public struct RuneBookConfig
    {
        public int ScrollOffset { get; set; }
        public int DropOffset { get; set; }
        public int DefaultOffset { get; set; }
        public int RecallOffset { get; set; }
        public int GateOffset { get; set; }
        public int SacredOffset { get; set; }
        public int Jumper { get; set; }
    }
}
