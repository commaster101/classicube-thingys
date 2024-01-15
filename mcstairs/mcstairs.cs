using System;
using System.Collections.Generic;

using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy
{
    public sealed class mcstairs : Plugin
    {
        public override string name { get { return "mcstairs"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.1"; } }
        
        static string author = "LJplays+";
        public override string creator { get { return author; } }


        //This is a server-side block IDs for all the blocks added

        static BlockID StairN = 325;
        static BlockID StairW = 326;
        static BlockID StairE = 327;
        static BlockID StairS = 328;
        static BlockID StairBase = 44;

        public override void Load(bool startup)
        {
            //The map we want to add a physics block to might already be loaded when the plugin starts, thus we have to add it right away in that case.
            Level[] levels = LevelInfo.Loaded.Items;
            foreach (Level lvl in levels)
            {
                if (!lvl.Config.MOTD.ToLower().Contains("-stairs"))
                {
                    lvl.PhysicsHandlers[StairN] = DoStair;
                    lvl.PhysicsHandlers[StairS] = DoStair;
                    lvl.PhysicsHandlers[StairE] = DoStair;
                    lvl.PhysicsHandlers[StairW] = DoStair;
                }
            }
            //Otherwise, we will look for when it loads using an event
            OnBlockHandlersUpdatedEvent.Register(OnBlockHandlersUpdated, Priority.Low);
        }
        public override void Unload(bool shutdown)
        {
            OnBlockHandlersUpdatedEvent.Unregister(OnBlockHandlersUpdated);
        }

        static void OnBlockHandlersUpdated(Level lvl, BlockID block)
        {
            if (lvl.Config.MOTD.ToLower().Contains("-stairs")) return; 

            if (block == StairN) lvl.PhysicsHandlers[StairN] = DoStair;
            if (block == StairS) lvl.PhysicsHandlers[StairS] = DoStair;
            if (block == StairE) lvl.PhysicsHandlers[StairE] = DoStair;
            if (block == StairW) lvl.PhysicsHandlers[StairW] = DoStair;
        }

        static void DoStair(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            int index;
            int indexdown;
            BlockID blockdown = lvl.GetBlock(x, (ushort)(y - 1), z, out indexdown);
            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (lvl.GetBlock(x, (ushort)(y - 1), z) == StairBase) return;
            else if(lvl.GetBlock(x, (ushort)(y - 1), z) == 0)
            {
                lvl.AddUpdate(indexdown, StairBase, true);
            }
            else if (lvl.GetBlock(x, (ushort)(y + 1), z) == 0)
            {
                if (lvl.GetBlock(x, (ushort)(y - 1), z) == StairBase) return;
                lvl.AddUpdate(index, StairBase, true);
                lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                lvl.AddUpdate(index, block, true);
            }
            else
            {
                lvl.AddUpdate(index, 0, true);
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
    }
}
