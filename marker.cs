
using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Tasks;

namespace MCGalaxy
{
    public class Compass : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "marker"; } }

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
               
                int x = p.Pos.BlockX, z = p.Pos.BlockZ;
                //edit the 63's to change the point the mark points to
                double marka = Math.Atan2(z - 63, x - 63);
                double markb = Math.Round(marka * (180 / Math.PI)) - 90;
                if (markb < 0) markb += 360;

                int mark = Convert.ToInt32(markb);
       

                if (!p.Supports(CpeExt.MessageTypes)) continue;
                //get yaw
                int yaw = Orientation.PackedToDegrees(p.Rot.RotY);
                
                int cyaw = yaw;

                // If value is the same, don't bother sending status packets to the client
                if (p.Extras.GetInt("COMPASS_VALUE") == yaw && p.Extras.GetInt("COMPASS_VALUE2") == x && p.Extras.GetInt("COMPASS_VALUE3") == z) continue;           

                // Store yaw in extras values so we can retrieve it above
                p.Extras["COMPASS_VALUE"] = yaw;
                p.Extras["COMPASS_VALUE2"] = x;
                p.Extras["COMPASS_VALUE3"] = z;
                // add text above the compass in the blank space

                // marker
                    p.SendCpeMessage(CpeMessageType.Status1, "%b---------");

                if (yaw >= (mark + 17) && yaw < (mark + 22))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b%e|%b--------");

                if (yaw >= (mark + 12) && yaw < (mark + 17))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b-%e|%b-------");

                if (yaw >= (mark + 7) && yaw < (mark + 12))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b--%e|%b------");

                if (yaw >= (mark + 2) && yaw < (mark + 7))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b---%e|%b-----");

                if (yaw >= (mark - 2) && yaw < (mark + 2)) 
                    p.SendCpeMessage(CpeMessageType.Status1, "%b----%e|%b----");

                if (yaw >= (mark - 7) && yaw < (mark - 2))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b-----%e|%b---");

                if (yaw >= (mark - 12) && yaw < (mark - 7))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b------%e|%b--");

                if (yaw >= (mark - 17) && yaw < (mark - 12))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b-------%e|%b-");

                if (yaw >= (mark - 22) && yaw < (mark - 17))
                    p.SendCpeMessage(CpeMessageType.Status1, "%b--------%e|%b");



                //North
                if (yaw >= 12 && yaw < 17)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b--N");

                if (yaw >= 7 && yaw < 12)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b-N-");

                if (yaw >= 2 && yaw < 7)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%bN--");

                if (yaw >= 358 | yaw < 2)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b----%fN%b----");

                if (yaw >= 353 && yaw < 358)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b--N%f" + cyaw + "%b---");

                if (yaw >= 348 && yaw < 353)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b-N-%f" + cyaw + "%b---");

                if (yaw >= 343 && yaw < 348)
                    p.SendCpeMessage(CpeMessageType.Status2, "%bN--%f" + cyaw + "%b---");


                //East
                if (yaw >= 102 && yaw < 107)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b--E");

                if (yaw >= 97 && yaw < 102)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b-E-");

                if (yaw >= 92 && yaw < 97)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%bE--");

                if (yaw >= 88 && yaw < 92)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b----%fE%b----");

                if (yaw >= 83 && yaw < 88)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b--E%f" + cyaw + "%b---");

                if (yaw >= 78 && yaw < 83)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b-E-%f" + cyaw + "%b---");

                if (yaw >= 73 && yaw < 78)
                    p.SendCpeMessage(CpeMessageType.Status2, "%bE--%f" + cyaw + "%b---");

                //south
                if (yaw >= 192 && yaw < 197)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b--S");

                if (yaw >= 187 && yaw < 192)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b-S-");

                if (yaw >= 182 && yaw < 187)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%bS--");

                if (yaw >= 178 && yaw < 182)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b----%fS%b----");

                if (yaw >= 173 && yaw < 178)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b--S%f" + cyaw + "%b---");

                if (yaw >= 168 && yaw < 173)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b-S-%f" + cyaw + "%b---");

                if (yaw >= 163 && yaw < 168)
                    p.SendCpeMessage(CpeMessageType.Status2, "%bS--%f" + cyaw + "%b---");

                //West
                if (yaw >= 282 && yaw < 287)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b--W");

                if (yaw >= 277 && yaw < 282)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b-W-");

                if (yaw >= 272 && yaw < 277)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%bW--");

                if (yaw >= 268 && yaw < 272)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b----%fW%b----");

                if (yaw >= 263 && yaw < 268)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b--W%f" + cyaw + "%b---");

                if (yaw >= 258 && yaw < 263)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b-W-%f" + cyaw + "%b---");

                if (yaw >= 253 && yaw < 258)
                    p.SendCpeMessage(CpeMessageType.Status2, "%bW--%f" + cyaw + "%b---");

                //other degrees

                if (yaw >= 17 && yaw < 73)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b---");

                if (yaw >= 107 && yaw < 163)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b---");

                if (yaw >= 197 && yaw < 253)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b---");

                if (yaw >= 287 && yaw < 343)
                    p.SendCpeMessage(CpeMessageType.Status2, "%b---%f" + cyaw + "%b---");
            }
        }

        public override void Unload(bool shutdown)
        {
            Server.MainScheduler.Cancel(task);
        }
    }
}
