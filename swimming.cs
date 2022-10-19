//this plugin requires the custom models plugin - CustomModels: https://github.com/NotAwesome2/MCGalaxy-CustomModels/releases/tag/v1.4.2
//(you will also have to copy https://github.com/NotAwesome2/MCGalaxy-CustomModels/blob/master/Newtonsoft.Json.dll into the folder mcgalaxy lives)
//
//after you load the plugin put this model file https://www.dropbox.com/s/8b9u9xzglpfe39g/crawling.bbmodel?dl=0
//in the new models folder in the plugin folder
//
//after you have put the model in run these cmds
// /custommodel config crawling eyeY 4
// /custommodel config crawling calchumananims false
// /custommodel config crawling bobbing false
// /custommodel config crawling collisionbounds 8.6 12 8.6
// /custommodel config crawling pickingbounds 0 0 0 8 12 32
//
// you will now star crawling if you are in a 1 block tall space and start swimming if you press left-shift in water
using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Tasks;
using MCGalaxy.Blocks;
using MCGalaxy.Commands.Fun;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy
{
    public class crawl : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "swimming"; } }

        public static SchedulerTask task;

        public override void Load(bool startup)
        {
            task = Server.MainScheduler.QueueRepeat(Use, null, TimeSpan.FromMilliseconds(200));

            Command.Register(new Cmdswim());
        }
 
        static void Use(SchedulerTask task)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
            {
                int x = p.Pos.BlockX, y = p.Pos.BlockY, z = p.Pos.BlockZ;
                bool inblock = false;             
                inblock = Findblock(p.level, (ushort)x, (ushort)y, (ushort)z);
                if (!p.Supports(CpeExt.TextHotkey)) continue;
                p.Send(Packet.TextHotKey("swim", "/swim◙", 42, 0, true));
                //checks if your a human model so it will work with other plugins and doesnt turn a chicken into a person
                if (inblock == true && ((p.IsLikelyInsideBlock()) || p.Extras.GetBoolean("wantsswim") == true) && (p.Model == "humanoid"))
                {
                    p.UpdateModel("crawling");
                }
                if (inblock == false && (p.Model == "crawling"))
                {
                    p.UpdateModel("humanoid");
                    p.Extras["wantsswim"] = false;
                }

                //if your skin is a slim model the name changes becuase models are hard
                if (inblock == false && (p.Model == "crawling(alex)"))
                {
                    p.UpdateModel("humanoid");
                    p.Extras["wantsswim"] = false;
                }
                else p.Extras["wantsswim"] = false; continue;
            }
        }
      
        //checks if the block at one more then your Ycoord is soild and your current coord is not/checks if your in water
        static bool Findblock(Level lvl, ushort x, int y, ushort z)
        {  
                BlockID block = lvl.GetBlock(x, (ushort)y, z);
                BlockID block2 = lvl.GetBlock(x, (ushort)(y - 1), z);
            if (CollideType.IsSolid(lvl.CollideType(block))) return true;
            //add list of blocks you can swim in here (can't use block def becuase lava isn't swimthrough)
            if (block2 == 9 || block2 == 8) return true;
            else return false;   
        }

        public sealed class Cmdswim : Command2
        {
            public override string name { get { return "swim"; } }
            public override string type { get { return CommandTypes.Games; } }
            public override bool museumUsable { get { return false; } }
            public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
            public override bool SuperUseable { get { return false; } }

            public override void Use(Player p, string message, CommandData data)
            {
                p.Extras["wantsswim"] = true;
            }

            public override void Help(Player p)
            {
                p.Message("%T/swim %H-lets you swim in water.");
            }
        }

        public override void Unload(bool shutdown)
        {
            Server.MainScheduler.Cancel(task);

            Command.Unregister(Command.Find("swim"));
        }
    }
}
