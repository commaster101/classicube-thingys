using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Tasks;
using MCGalaxy.Events.PlayerEvents;
using BlockID = System.UInt16;

namespace MCGalaxy
{
    public class Blockmatch : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "flappy"; } }

        public static SchedulerTask task;

        public override void Load(bool startup)
        {
            task = Server.MainScheduler.QueueRepeat(CheckDirection, null, TimeSpan.FromMilliseconds(100));
        }

        void CheckDirection(SchedulerTask task)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
            {
                if (!p.level.Config.MOTD.ToLower().Contains("+flappy")) continue;

                if (p.Extras.GetInt("stage") == 33)
                {
                    Random rnd = new Random();

                    p.Extras["z"] = rnd.Next(23);

                    Thread.Sleep(50);

                }

                int z = p.Extras.GetInt("z");

                p.Extras["stage"] = p.Extras.GetInt("stage") - 1;

                int i = 0;
                while (i < z)
                {
                    if ((p.Pos.BlockY == i) && (p.Pos.BlockZ == (ushort)(p.Extras.GetInt("stage")))) p.Extras["flappyscore"] = 0;
                    p.SendBlockchange(1, (ushort)(i), (ushort)(p.Extras.GetInt("stage")), 25);
                    p.RevertBlock( 1, (ushort)(i), (ushort)((p.Extras.GetInt("stage")) + 3));
                    i++;
                }
                int j = z + 8;
                while (j < 31)
                {
                    if ((p.Pos.BlockY == j) && (p.Pos.BlockZ == (ushort)(p.Extras.GetInt("stage")))) p.Extras["flappyscore"] = 0;
                    p.SendBlockchange(1, (ushort)(j), (ushort)(p.Extras.GetInt("stage")), 25);
                    p.RevertBlock( 1, (ushort)(j), (ushort)((p.Extras.GetInt("stage")) + 3));
                    j++;
                }

                if (p.Extras.GetInt("stage") <= 0)
                {
                    p.Extras["stage"] = 36;
                    p.Extras["flappyscore"] = p.Extras.GetInt("flappyscore") + 1;
                    p.SendCpeMessage(CpeMessageType.Announcement, "-{-" + p.Extras.GetString("flappyscore") + "-}-");
                }

                if(p.level.GetBlock((ushort)p.Pos.BlockX, (ushort)p.Pos.BlockY, (ushort)p.Pos.BlockZ) == 25) p.Extras["flappyscore"] = 0;
            }
        }
        
        public override void Unload(bool shutdown)
        {
            Server.MainScheduler.Cancel(task);
        }
    }
}