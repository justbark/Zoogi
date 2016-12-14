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
using System.Threading;


namespace zoogiHunter
{
    class Program
    {
//==========================================================================================
//main
//==========================================================================================
        static void Main(string[] args)
        {
            string fileName = "C:\\Users\\Justin\\Desktop\\antrail";
            string windRailFile = "C:\\Users\\Justin\\Desktop\\windrail";
            List<Tuple<int, int, int>> rail = readRail(fileName);
            List<Tuple<int, int, int>> windRail = readRail(windRailFile);

            PlayerMobile Player = PlayerMobile.GetPlayer();
            var RUOconfig = new RuneBookConfig()
            {
                ScrollOffset = 2,
                DropOffset = 3,
                DefaultOffset = 4,
                RecallOffset = 5,
                GateOffset = 6,
                SacredOffset = 7,
                Jumper = 6
            };

            Item runebookserial = GetTargetItem();
            Runebook rb = new Runebook(runebookserial.Serial.Value, RUOconfig);

          
            Console.WriteLine("recalling to ant location");
            recallTo(Player, rb, "antHole");
            Stealth.Client.Wait(1000);
            Console.WriteLine("going into anthole");
            //===============================================================================
            //ant hole
            //===============================================================================
            int locX = Player.Location.X;
            int locY = Player.Location.Y;

            clickHole(Player, locX, locY);
            for(int i = 0; i < rail.Count(); i++)
            {
                Console.WriteLine("moving to: ");
                Console.WriteLine("\t" + rail.ElementAt(i).Item1 + "," + rail.ElementAt(i).Item2 + "," + rail.ElementAt(i).Item3);

                if (rail.ElementAt(i).Item1 == 0 &&
                    rail.ElementAt(i).Item2 == 0 &&
                    rail.ElementAt(i).Item3 == 0)
                {
                    int _X = Player.Location.X;
                    int _Y = Player.Location.Y;
                    Console.WriteLine("clicking hole 1...");
                    clickHoleTile(Player, _X, _Y, 5913, 1893, 1073886108);
                    Console.WriteLine("success.");
                    continue;
                }
                if (rail.ElementAt(i).Item1 == 1 &&
                    rail.ElementAt(i).Item2 == 1 &&
                    rail.ElementAt(i).Item3 == 1)
                {
                    int _X = Player.Location.X;
                    int _Y = Player.Location.Y;
                    Console.WriteLine("clicking hole 2...");
                    clickHoleTile(Player, _X, _Y, 5876, 1892, 1073886069);
                    Console.WriteLine("success.");
                    continue;
                }

                if (Player.Hits != Player.MaxHits)
                {
                    healSelf(Player);
                }
                Console.WriteLine("railNum: " + i + " out of " + rail.Count());
                Console.WriteLine((rail.Count - i) + " rail spots to go.");

                advancePosition(Player, rail.ElementAt(i));

            }
            //Console.ReadLine();
            //===========================================================================
            //find the matriarch
            //============================================================================
            SDK.Initialize();

            List<Mobile> npcs = Scanner.FindMobiles();
            foreach (Mobile m in npcs)
            {
                if (m.Serial.Value == 1640)
                {
                    ushort _locX = (ushort)m.Location.X;
                    ushort _locY = (ushort)m.Location.Y;
                    sbyte _locZ = (sbyte)m.Location.Z;
                    Console.WriteLine("Npc found. Moving to location...");
                    Player.Movement.MoveXYZ(_locX, _locY, _locZ, 1, 1, true);
                    Stealth.Client.Wait(1000);
                    acceptQuesticle(Player, m);
                    
                    break;
                 }

             }
            //============================================================================
            //at wind passage. Need to kill 7 ants
            //============================================================================
            Console.WriteLine("recalling to wind");
            recallTo(Player, rb, "windTram");
            Console.WriteLine("begining rail...");
            List<Ants> ants = new List<Ants>();
            Scanner.Range = 3;
            Scanner.VerticalRange = (uint)Player.Location.Z;
            for (int i = 0; i < windRail.Count(); i++)
            {
                if (Player.Hits != Player.MaxHits)
                {
                    healSelf(Player);
                }
                advancePosition(Player, windRail.ElementAt(i));
                SDK.Initialize();

                ants = Scanner.Find<Ants>(0x0, false).OrderBy(x => x.Distance).ToList();
                foreach (Ants m in ants)
                {
                   Player.Cast("Wither");
                   Thread.Sleep(1000);
                   if (Player.Hits != Player.MaxHits)
                   {
                      healSelf(Player);
                   }
                }
                if(ants.Count == 0)
                {
                    continue;
                }
            }
        }
//=========================================================================================
//end of main
//=========================================================================================
        static void acceptQuesticle(PlayerMobile P, Mobile npc)
        {
            Console.WriteLine("Accepting quest");
            ContextOptions.Initialize();
            npc.ContextMenu.Parse();
            Stealth.Client.Wait(1500);
            npc.ContextMenu.Click("Talk", true);


            //foreach(Gump gumps in Gump.ActiveGumps)
            //{
            //    Console.WriteLine("gump type: " + gumps.GumpType);
            //    Console.WriteLine("serial: " + gumps.Serial.Value);
            //    Console.WriteLine("buttons: " + gumps.Buttons.Count);
            //}

            Gump questGump;
            Gump.WaitForGump(2460962336, 5000, out questGump);
            if (questGump == null)
                Console.WriteLine("questgump is null");
            Stealth.Client.Wait(1000);
            questGump.Buttons.First().Click();
            Gump secondQuestGump;
            Gump.WaitForGump(2685952746, 5000, out secondQuestGump);

            if (secondQuestGump == null)
                Console.WriteLine("second quest gump is null");
            Stealth.Client.Wait(1000);
            secondQuestGump.Buttons.First().Click();
          
            Gump questAcceptedGump;
            Gump.WaitForGump(1353954171, 5000, out questAcceptedGump);

            if (questAcceptedGump == null)
                Console.WriteLine("questAcceptedGump is null");
            Stealth.Client.Wait(1000);
            questAcceptedGump.Close();
            //Console.ReadLine();

        }

        private static void healSelf(PlayerMobile p)
        {
            Console.WriteLine("player damaged, Healing self.");
            p.Cast("heal");
            p.Targeting.AutoTargetSelf();
        }

        private static void advancePosition(PlayerMobile P, Tuple<int,int,int> location)
        {
            P.Movement.MoveXYZ((ushort)location.Item1, (ushort)location.Item2, (sbyte)location.Item3, 0, 0, true);
           
        }

        public static List<Tuple<int, int, int>> readRail(string fileName)
        {
            List<Tuple<int, int, int>> file_contents = new List<Tuple<int, int, int>>();
            using (var streamReader = File.OpenText(fileName))
            {
                var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    List<string> rawcoords = line.Split(',').ToList();
                    int x = Convert.ToInt32(rawcoords[0]);
                    int y = Convert.ToInt32(rawcoords[1]);
                    int z = Convert.ToInt32(rawcoords[2]);

                    Tuple<int, int, int> xyz = new Tuple<int, int, int>(x, y, z);
                    file_contents.Add(xyz);
                }
            }
            return file_contents;
        }

        static void ErrorExit(string errorText)
        {
            Console.WriteLine(errorText);
            Console.WriteLine("press any key to exit");
            Console.ReadKey(false);
            Environment.Exit(0);
        }

        public static Item GetTargetItem()
        {
            Stealth.Client.ClientRequestObjectTarget();
            while (Stealth.Client.ClientTargetResponsePresent() == false) ;
            return new Item(new Serial(Stealth.Client.ClientTargetResponse().ID));
        }

        public static void recallTo(PlayerMobile character, Runebook book, String location)
        {
            book.Parse();
            Stealth.Client.Wait(100);
            foreach (RunebookEntry rune in book.Entries)
            {
                if (rune.Name == location && book.Entries.IndexOf(rune) != book.DefaultRune)
                {
                    rune.SetDefault();
                    break;
                }
            }
            Stealth.Client.Wait(100);
            book.Recall();//recall to location
            book.Close();
            
        }

        public static void clickHole(PlayerMobile player, int locX, int locY)
        {
            SDK.Initialize();

            List<Hole> holes = Scanner.Find<Hole>(1725, 815);
            Console.WriteLine("holes found: " + holes.Count);

            while (player.Location.X == locX && player.Location.Y == locY)
            {
                foreach (Hole entranceHole in holes)
                {
                    Console.WriteLine(entranceHole.GetType());
                    Console.WriteLine(entranceHole.Serial.Value);
                    Console.WriteLine(entranceHole.Tooltip);
                    if (entranceHole.Serial.Value == 1073886186)
                    {
                        entranceHole.DoubleClick();
                        break;
                    }
                }
            }
        }
        public static void clickHoleTile(PlayerMobile p, int LocX, int LocY, int holeX, int holeY, uint id)
        {
            SDK.Initialize();

            List<HoleTile> holeTiles = Scanner.Find<HoleTile>(holeX, holeY);
            Console.WriteLine("holes found: " + holeTiles.Count);

            while (p.Location.X == LocX && p.Location.Y == LocY)
            {
                foreach (HoleTile secondaryHole in holeTiles)
                {
                    Console.WriteLine(secondaryHole.GetType());
                    Console.WriteLine(secondaryHole.Serial.Value);
                    Console.WriteLine(secondaryHole.Tooltip);
                    if (secondaryHole.Serial.Value == id)
                    {
                        secondaryHole.DoubleClick();
                        break;
                    }
                }
            }
        }
    }

    [QuerySearch(new ushort[] { 6006 })]
    public class Hole : Item
    {
        public Hole(Serial serial)
            : base(serial)
        {

        }
    }
    [QuerySearch(new ushort[] { 1173 })]
    public class HoleTile : Item
    {
        public HoleTile(Serial serial)
            : base(serial)
        {

        }
    }
    [QuerySearch(new ushort[] { 805, 806, 807 })]
    public class Ants : Item
    {
        public Ants(Serial serial)
            : base(serial)
        {

        }
    }
}
