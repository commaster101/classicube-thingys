using System;
using System.Collections.Generic;

using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy {
	
	public sealed class illLogic : Plugin {
		public override string name { get { return "illLogic"; } }
		public override string MCGalaxy_Version { get { return "1.9.4.1"; } }
        //This name is used to determine who to send debug text to
        static string author = "LJplays+";
		public override string creator { get { return author; } }
        
        //The MOTD to add custom physics to a Level.
        static string physicsMOTDName = "+logic";

        //the MOTD to make it blocks can't be made or deleted only moved
        static string infMOTDName = "-inflogic";

        //This is a server-side block IDs for all the blocks added

        static BlockID obsidian = 49;
        static BlockID bedrock = 7;
        static BlockID buttonBlock = 360;

        //texture like reneforce cable or somthing
        static BlockID wireBlock = 359;

        //moves up and down mabye arrows?
        static BlockID flipperBlock = 362;

        //texture should show that it pushs
        static BlockID repellerBlock = 366;

        //some kind of styled arrow mabye like a claw
        static BlockID placerDBlock = 361;
        static BlockID placerUBlock = 365;
        static BlockID placerNBlock = 373;
        static BlockID placerSBlock = 374;
        static BlockID placerEBlock = 375;
        static BlockID placerWBlock = 376;

        //like a lamp lol
        static BlockID lightOnBlock = 363;
        static BlockID lightOffBlock = 364;

        //a basic arrow 
        static BlockID weyNBlock = 367;
        static BlockID weySBlock = 368;
        static BlockID weyEBlock = 369;
        static BlockID weyWBlock = 370;
        static BlockID weyUBlock = 371;
        static BlockID weyDBlock = 372;

        //You can find the server-side block ID in a custom command with:
        //Vec3S32 pos = p.Pos.FeetBlockCoords;
        //p.Message("Server-side BlockID at this location is {0}", p.level.GetBlock((ushort)pos.X, (ushort)pos.Y, (ushort)pos.Z));

        public override void Load(bool startup) {
            
            //The map we want to add a physics block to might already be loaded when the plugin starts, thus we have to add it right away in that case.
			Level[] levels = LevelInfo.Loaded.Items;
			foreach (Level lvl in levels) {
                if (lvl.Config.MOTD.ToLower().Contains(physicsMOTDName)) {
                    lvl.PhysicsHandlers[wireBlock] = DoWire;
                    lvl.PhysicsHandlers[flipperBlock] = DoFlipper;

                    lvl.PhysicsHandlers[repellerBlock] = DoRepeller;

                    lvl.PhysicsHandlers[placerDBlock] = DoPlacer;
                    lvl.PhysicsHandlers[placerUBlock] = DoPlacer;
                    lvl.PhysicsHandlers[placerNBlock] = DoPlacer;
                    lvl.PhysicsHandlers[placerSBlock] = DoPlacer;
                    lvl.PhysicsHandlers[placerEBlock] = DoPlacer;
                    lvl.PhysicsHandlers[placerWBlock] = DoPlacer;

                    lvl.PhysicsHandlers[lightOnBlock] = Dolight;
                    lvl.PhysicsHandlers[lightOffBlock] = Dolight;

                    lvl.PhysicsHandlers[weyNBlock] = Dowey;
                    lvl.PhysicsHandlers[weySBlock] = Dowey;
                    lvl.PhysicsHandlers[weyEBlock] = Dowey;
                    lvl.PhysicsHandlers[weyWBlock] = Dowey;
                    lvl.PhysicsHandlers[weyUBlock] = Dowey;
                    lvl.PhysicsHandlers[weyDBlock] = Dowey;

                }
			}
            
            //Otherwise, we will look for when it loads using an event
            OnBlockHandlersUpdatedEvent.Register(OnBlockHandlersUpdated, Priority.Low);
            OnPlayerClickEvent.Register(toggles, Priority.Low);
        }
		public override void Unload(bool shutdown) {
            OnBlockHandlersUpdatedEvent.Unregister(OnBlockHandlersUpdated);
            OnPlayerClickEvent.Unregister(toggles);
        }
        
		static void OnBlockHandlersUpdated(Level lvl, BlockID block) {
            if (!lvl.Config.MOTD.ToLower().Contains(physicsMOTDName)) { return; }
            if (block == wireBlock) lvl.PhysicsHandlers[wireBlock] = DoWire;

            if (block == flipperBlock) lvl.PhysicsHandlers[flipperBlock] = DoFlipper;

            if (block == repellerBlock) lvl.PhysicsHandlers[repellerBlock] = DoRepeller;

            if (block == placerDBlock) lvl.PhysicsHandlers[placerDBlock] = DoPlacer;
            if (block == placerUBlock) lvl.PhysicsHandlers[placerUBlock] = DoPlacer;
            if (block == placerNBlock) lvl.PhysicsHandlers[placerNBlock] = DoPlacer;
            if (block == placerSBlock) lvl.PhysicsHandlers[placerSBlock] = DoPlacer;
            if (block == placerEBlock) lvl.PhysicsHandlers[placerEBlock] = DoPlacer;
            if (block == placerWBlock) lvl.PhysicsHandlers[placerWBlock] = DoPlacer;

            if (block == lightOnBlock) lvl.PhysicsHandlers[lightOnBlock] = Dolight;
            if (block == lightOffBlock) lvl.PhysicsHandlers[lightOffBlock] = Dolight;

            if (block == weyNBlock) lvl.PhysicsHandlers[weyNBlock] = Dowey;
            if (block == weySBlock) lvl.PhysicsHandlers[weySBlock] = Dowey;
            if (block == weyEBlock) lvl.PhysicsHandlers[weyEBlock] = Dowey;
            if (block == weyWBlock) lvl.PhysicsHandlers[weyWBlock] = Dowey;
            if (block == weyUBlock) lvl.PhysicsHandlers[weyUBlock] = Dowey;
            if (block == weyDBlock) lvl.PhysicsHandlers[weyDBlock] = Dowey;
        }

        void toggles(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            BlockID block = p.level.GetBlock(x, y, z);

            if (block != buttonBlock) return;
            if (button != MouseButton.Right) return;
            if (action == MouseAction.Pressed) return;
            Dotoggle(p, x, y, z);
        }

        static void DoFlipper(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            int index = C.Index;

            int f = 0;
            if (lvl.GetBlock(x, (ushort)(y + 1), z) != 0) f++;
            if (lvl.GetBlock(x, (ushort)(y - 1), z) != 0) f--;

            if (f == 0){
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
                return;
            }

            if(lvl.GetBlock(x, (ushort)(y + f + f), z) != 0) return;
            if (lvl.GetBlock(x, (ushort)(y + f), z) == flipperBlock) return;
            
            lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs)); 
            lvl.AddUpdate(lvl.IntOffset(index, 0, +f, 0), C.Block);

            BlockID push = lvl.GetBlock(x, (ushort)(y + f), z);
            lvl.AddUpdate(lvl.IntOffset(index, 0, +f +f, 0), push);

            //push players
            if (f == 1)
            {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player p in players)
                {
                    if (p.Pos.BlockY == (C.Y + 2) || p.Pos.BlockY == (C.Y + 3))
                    {
                        if((p.Pos.BlockZ == C.Z) && p.Pos.BlockX == C.X)
                        {
                            //this looks kind wierd but we are tping a the player up
                            Position pos = Position.FromFeet(p.Pos.X, p.Pos.Y, p.Pos.Z);
                            p.SendPosition(pos, p.Rot);
                        }
                    }
                }
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }

        static void DoRepeller(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            int index = C.Index;

            List<int> unpushable = new List<int>();
            unpushable.Add(obsidian);
            unpushable.Add(bedrock);
            unpushable.Add(flipperBlock);
            unpushable.Add(buttonBlock);
            unpushable.Add(0);

            //don't use lvl.IntOffset cus threading and stuff mucks up physics
            if (!unpushable.Contains(lvl.GetBlock(x, (ushort)(y + 1), z)) && (lvl.GetBlock(x, (ushort)(y + 2), z) == 0))
            {
                BlockID push = lvl.GetBlock(x, (ushort)(y + 1), z);

                lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                lvl.AddUpdate(index, 0, true);

                lvl.GetBlock(x, (ushort)(y + 2), z, out index);
                lvl.AddUpdate(index, push, true);
            }
            if (!unpushable.Contains(lvl.GetBlock(x, (ushort)(y - 1), z)) && (lvl.GetBlock(x, (ushort)(y - 2), z) == 0))
            {
                BlockID push = lvl.GetBlock(x, (ushort)(y - 1), z);

                lvl.GetBlock(x, (ushort)(y - 1), z, out index);
                lvl.AddUpdate(index, 0, true);

                lvl.GetBlock(x, (ushort)(y - 2), z, out index);
                lvl.AddUpdate(index, push, true);
            }

            if (!unpushable.Contains(lvl.GetBlock((ushort)(x + 1), y, z)) && (lvl.GetBlock((ushort)(x + 2), y, z) == 0))
            {
                BlockID push = lvl.GetBlock((ushort)(x + 1), y, z);

                lvl.GetBlock((ushort)(x + 1), y, z, out index);
                lvl.AddUpdate(index, 0, true);

                lvl.GetBlock((ushort)(x + 2), y, z, out index);
                lvl.AddUpdate(index, push, true);
            }
            if (!unpushable.Contains(lvl.GetBlock((ushort)(x - 1), y, z)) && (lvl.GetBlock((ushort)(x - 2), y, z) == 0))
            {
                BlockID push = lvl.GetBlock((ushort)(x - 1), y, z);

                lvl.GetBlock((ushort)(x - 1), y, z, out index);
                lvl.AddUpdate(index, 0, true);

                lvl.GetBlock((ushort)(x - 2), y, z, out index);
                lvl.AddUpdate(index, push, true);
            }

            if (!unpushable.Contains(lvl.GetBlock(x, y, (ushort)(z + 1))) && (lvl.GetBlock(x, y, (ushort)(z + 2)) == 0))
            {
                BlockID push = lvl.GetBlock(x, y, (ushort)(z + 1));

                lvl.GetBlock(x, y, (ushort)(z + 1), out index);
                lvl.AddUpdate(index, 0, true);

                lvl.GetBlock(x, y, (ushort)(z + 2), out index);
                lvl.AddUpdate(index, push, true);
            }
            if (!unpushable.Contains(lvl.GetBlock(x, y, (ushort)(z - 1))) && (lvl.GetBlock(x, y, (ushort)(z - 2)) == 0))
            {
                BlockID push = lvl.GetBlock(x, y, (ushort)(z - 1));

                lvl.GetBlock(x, y, (ushort)(z - 1), out index);
                lvl.AddUpdate(index, 0, true);

                lvl.GetBlock(x, y, (ushort)(z - 2), out index);
                lvl.AddUpdate(index, push, true);
            }

            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }

        static void Dowey(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            ushort yCur = y;

            int index;

            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (block == weyEBlock)//+x
            {
                lvl.GetBlock((ushort)(x + 1), y, z, out index);
                lvl.AddCheck(index);
            }
            else if (block == weyWBlock)//-x
            {
                lvl.GetBlock((ushort)(x - 1), y, z, out index);
                lvl.AddCheck(index);
            }
            else if (block == weySBlock)//+z
            {
                lvl.GetBlock(x, y, (ushort)(z + 1), out index);
                lvl.AddCheck(index);
            }
            else if (block == weyNBlock)//-z
            { 
                lvl.GetBlock(x, y, (ushort)(z - 1), out index);
                lvl.AddCheck(index);
            }
            else if (block == weyUBlock)//+y
            {
                lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                lvl.AddCheck(index);
            }
            else if (block == weyDBlock)//-y
            {
                lvl.GetBlock(x, (ushort)(y - 1), z, out index);
                lvl.AddCheck(index);
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }

        static void Dolight(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            ushort yCur = y;
            
            int index;

            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (block == lightOnBlock)
            {
                block = lightOffBlock;
                lvl.AddUpdate(index, block, true);
            }
            else if(block == lightOffBlock) 
            {
                block = lightOnBlock;
                lvl.AddUpdate(index, block, true);

                lvl.GetBlock((ushort)(x + 1), y, z, out index);
                lvl.AddCheck(index);

                lvl.GetBlock((ushort)(x - 1), y, z, out index);
                lvl.AddCheck(index);

                lvl.GetBlock(x, y, (ushort)(z + 1), out index);
                lvl.AddCheck(index);

                lvl.GetBlock(x, y, (ushort)(z - 1), out index);
                lvl.AddCheck(index);

                if (lvl.GetBlock(x, (ushort)(y + 1), z) != placerDBlock && (lvl.GetBlock(x, (ushort)(y + 1), z) != placerUBlock))
                {
                    lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                    lvl.AddCheck(index);
                }
                if (lvl.GetBlock(x, (ushort)(y - 1), z) != placerDBlock && (lvl.GetBlock(x, (ushort)(y - 1), z) != placerUBlock))
                {
                    lvl.GetBlock(x, (ushort)(y - 1), z, out index);
                    lvl.AddCheck(index);
                }
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }

        //this code is really ugly sorry
        static void DoPlacer(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            BlockID block = lvl.GetBlock(x, y, z);
            if (block == placerUBlock) {
                BlockID below = lvl.GetBlock(x, (ushort)(y - 1), z);
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && lvl.GetBlock(x, (ushort)(y + 1), z) != 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && below == 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (below == placerUBlock) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                int index;
                lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                lvl.AddUpdate(index, below, true);

                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName))
                {
                    lvl.GetBlock(x, (ushort)(y - 1), z, out index);
                    lvl.AddUpdate(index, 0, true);
                }
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
            else if(block == placerDBlock)
            {
                BlockID above = lvl.GetBlock(x, (ushort)(y + 1), z);
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && lvl.GetBlock(x, (ushort)(y - 1), z) != 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && above == 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (above == placerDBlock) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                int index;
                lvl.GetBlock(x, (ushort)(y - 1), z, out index);
                lvl.AddUpdate(index, above, true);

                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName))
                {
                    lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                    lvl.AddUpdate(index, 0, true);
                }
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
            else if(block == placerNBlock)
            {
                BlockID behind = lvl.GetBlock(x, y, (ushort)(z + 1));
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && lvl.GetBlock(x, y, (ushort)(z - 1)) != 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && behind == 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (behind == placerNBlock) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                int index;
                lvl.GetBlock(x, y, (ushort)(z - 1), out index);
                lvl.AddUpdate(index, behind, true);

                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName))
                {
                    lvl.GetBlock(x, y, (ushort)(z + 1), out index);
                    lvl.AddUpdate(index, 0, true);
                }
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
            else if(block == placerSBlock)
            {
                BlockID behind = lvl.GetBlock(x, y, (ushort)(z - 1));
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && lvl.GetBlock(x, y, (ushort)(z + 1)) != 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && behind == 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (behind == placerSBlock) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                int index;
                lvl.GetBlock(x, y, (ushort)(z + 1), out index);
                lvl.AddUpdate(index, behind, true);

                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName))
                {
                    lvl.GetBlock(x, y, (ushort)(z - 1), out index);
                    lvl.AddUpdate(index, 0, true);
                }
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
            else if(block == placerEBlock)
            {
                BlockID behind = lvl.GetBlock((ushort)(x - 1), y, z);
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && lvl.GetBlock((ushort)(x + 1), y, z) != 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && behind == 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (behind == placerEBlock) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                int index;
                lvl.GetBlock((ushort)(x + 1), y, z, out index);
                lvl.AddUpdate(index, behind, true);

                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName))
                {
                    lvl.GetBlock((ushort)(x - 1), y, z, out index);
                    lvl.AddUpdate(index, 0, true);
                }
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
            else if(block == placerWBlock)
            {
                BlockID behind = lvl.GetBlock((ushort)(x + 1), y, z);
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && lvl.GetBlock((ushort)(x - 1), y, z) != 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName) && behind == 0) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                if (behind == placerWBlock) { C.Data.Data = PhysicsArgs.RemoveFromChecks; return; }
                int index;
                lvl.GetBlock((ushort)(x - 1), y, z, out index);
                lvl.AddUpdate(index, behind, true);

                if (lvl.Config.MOTD.ToLower().Contains(infMOTDName))
                {
                    lvl.GetBlock((ushort)(x + 1), y, z, out index);
                    lvl.AddUpdate(index, 0, true);
                }
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
        }

        //basic wire can activate doors,leaves and sand 
        static void DoWire(Level lvl, ref PhysInfo C) {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            ushort yCur = y;

            bool instant = true;
            ActivateablePhysics.DoDoors(lvl, (ushort)(x + 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, (ushort)(x - 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z + 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z - 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y - 1), z, instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y + 1), z, instant);

            int index;
            lvl.GetBlock((ushort)(x + 1), y, z, out index);
            lvl.AddCheck(index);

            lvl.GetBlock((ushort)(x - 1), y, z, out index);
            lvl.AddCheck(index);

            lvl.GetBlock(x, y, (ushort)(z + 1), out index);
            lvl.AddCheck(index);

            lvl.GetBlock(x, y, (ushort)(z - 1), out index);
            lvl.AddCheck(index);

            if (lvl.GetBlock(x, (ushort)(y + 1), z) != placerDBlock && (lvl.GetBlock(x, (ushort)(y + 1), z) != placerUBlock))
            {
                lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                lvl.AddCheck(index);
            }
            if (lvl.GetBlock(x, (ushort)(y - 1), z) != placerDBlock && (lvl.GetBlock(x, (ushort)(y - 1), z) != placerUBlock)) 
            {
                lvl.GetBlock(x, (ushort)(y - 1), z, out index);
                lvl.AddCheck(index);
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }

        //copy of wire but is activated on click
        static void Dotoggle(Player p, ushort x, ushort y, ushort z)
        {
            Level lvl = p.level;

            bool instant = true;
            ActivateablePhysics.DoDoors(lvl, (ushort)(x + 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, (ushort)(x - 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z + 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z - 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y - 1), z, instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y + 1), z, instant);

            int index;
            lvl.GetBlock((ushort)(x + 1), y, z, out index);
            lvl.AddCheck(index);

            lvl.GetBlock((ushort)(x - 1), y, z, out index);
            lvl.AddCheck(index);

            lvl.GetBlock(x, y, (ushort)(z + 1), out index);
            lvl.AddCheck(index);

            lvl.GetBlock(x, y, (ushort)(z - 1), out index);
            lvl.AddCheck(index);

            lvl.GetBlock(x, (ushort)(y + 1), z, out index);
            lvl.AddCheck(index);

            lvl.GetBlock(x, (ushort)(y - 1), z, out index);
            lvl.AddCheck(index);
        }

        static void MsgDebugger(string message, params object[] args) {
            Player debugger = PlayerInfo.FindExact(illLogic.author); if (debugger == null) { return; }
            debugger.Message(message, args);
        }
        
	}
	
}
