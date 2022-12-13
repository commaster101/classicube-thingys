using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Tasks;
using MCGalaxy.Blocks;
using MCGalaxy.Commands.Fun;
using BlockID = System.UInt16;

namespace MCGalaxy
{
    public class map : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "ljmap"; } }

        public static SchedulerTask task;

        public override void Load(bool startup)
        {
            task = Server.MainScheduler.QueueRepeat(Checkblocks, null, TimeSpan.FromMilliseconds(500));
        }

        void Checkblocks(SchedulerTask task)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
            {
                if (!p.level.Config.MOTD.ToLower().Contains("+minimap")) continue;
                //checks if the block with id 36 is held change for a custom block id to use as a map 
                int held = p.GetHeldBlock();
                if (held >= 66) held = held - 256;
                if (held != 36) continue;
                ClearChat(p);
                int col = 0;
                int row = 0;
               //array to store block data  player is in the middle bottem left is -,-
                string[,] mapdata = 
                { 
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"}, 
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"},
                                      {"%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█","%0█"} 
                };
                while (col <= 10)
                { 
                    while (row <= 10)
                    {
                        ushort block = 0;

                        int px = p.Pos.BlockX - 5 + row, py = p.Pos.BlockY + 5, pz = p.Pos.BlockZ - 5 + col;

                        int freeY = -1;
                        if (p.level.IsValidPos(px, py, pz))

                            freeY = FindYBelow(p.level, (ushort)px, py, (ushort)pz);
                        int freeya = freeY - 1;

                        string freeyS = freeY.ToString();
                        if (freeY > 0)
                            block = p.level.GetBlock((ushort)px, (ushort)freeya, (ushort)pz);

                        int blocka = Convert.ToInt32(block);

                        // If value is the same, don't bother sending status packets to the client
                        if (p.Extras.GetInt("COMPASS_VALUE2") == px && p.Extras.GetInt("COMPASS_VALUE3") == pz) continue;

                        // Store x and z in extras values so we can retrieve it above
                        p.Extras["COMPASS_VALUE2"] = px;
                        p.Extras["COMPASS_VALUE3"] = pz;
                        //add blocks to the map here
                        //if (block == [id of block eg. stone would be 1])
                        // mapdata[row, col] = "[symbol the block will show as on the block (try to use the same width symbols)]";

                        // blocks that share a symbol can be added like this
                        // if (block == [id of block 1 ]||[id of block 2]||[id of block 3])

                        if (block == 1)
                            mapdata[row, col] = "%7█";

                        if (block == 2)
                            mapdata[row, col] = "%2█";

                        if (block == 3)
                            mapdata[row, col] = "%4█";
                        
                        if (block == 4)
                            mapdata[row, col] = "%7█";
                        
                        if (block == 5)
                            mapdata[row, col] = "%c█";

                        if (block == 8)
                            mapdata[row, col] = "%b▒";
                        
                        if (block == 9)
                            mapdata[row, col] = "%b▒";
                        
                        if (block == 10)
                            mapdata[row, col] = "%c▒";
                        
                        if (block == 11)
                            mapdata[row, col] = "%c▒";
                        
                        if (block == 12)
                            mapdata[row, col] = "%6█";

                        if (block == 13)
                            mapdata[row, col] = "%7█";

                        if (block == 17)
                            mapdata[row, col] = "%c█";

                        if (block == 18)
                            mapdata[row, col] = "%a█";

                        if (block == 19)
                            mapdata[row, col] = "%e█";

                        if (block == 20)
                            mapdata[row, col] = "%f█";

                        if (block == 41)
                            mapdata[row, col] = "%e█";

                        if (block == 42)
                            mapdata[row, col] = "%f█";

                        if (block == 43)
                            mapdata[row, col] = "%7█";
                        
                        if (block == 44)
                            mapdata[row, col] = "%7█";

                        if (block == 45)
                            mapdata[row, col] = "%4█";
                        row++;
                    }
                    row = 0;
                    col++;
                }
                mapdata[5,5] = "%e█";
                if (!p.Supports(CpeExt.MessageTypes)) continue;
                int pcol = 0;
                while (pcol <= 10)
                {
                    p.Message(mapdata[0, pcol] + mapdata[1, pcol] + mapdata[2, pcol] + mapdata[3, pcol] + mapdata[4, pcol] + mapdata[5, pcol] + mapdata[6, pcol] + mapdata[7, pcol] + mapdata[8, pcol] + mapdata[9, pcol] + mapdata[10, pcol]);

                    pcol++;
                }
                p.Message("-----minimap-----");
            }
        }
        static int FindYBelow(Level lvl, ushort x, int y, ushort z)
        {
            for (; y >= 0; y--)
            {
                if (SolidAt(lvl, x, y, z)) continue;
              //if (SolidAt(lvl, x, y + 1, z)) continue;
                if (SolidAt(lvl, x, y - 1, z)) return y;
            }
            return -1;
        }

        static bool SolidAt(Level lvl, ushort x, int y, ushort z)
        {
            if (y - 5 >= lvl.Height) return false;
            BlockID block = lvl.GetBlock(x, (ushort)y, z);
            if (block != Block.Air) return Convert.ToBoolean(lvl.CollideType(block));
            //return CollideType.IsSolid(lvl.CollideType(block));
            return false;
        }

        static void ClearChat(Player p)
        {
            for (int i = 0; i < 30; i++)
            {
                p.Session.SendMessage(CpeMessageType.Normal, "");
            }
        }

        public override void Unload(bool shutdown)
        {
            Server.MainScheduler.Cancel(task);
        }
    }
}
