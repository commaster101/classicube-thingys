using System;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy {
	
	public sealed class wires : Plugin {
		public override string name { get { return "illLogic"; } }
		public override string MCGalaxy_Version { get { return "1.9.4.1"; } }
        //This name is used to determine who to send debug text to
        static string author = "LJplays";//used goodlyays physics exsample
		public override string creator { get { return author; } }
        
        //The level we want to add a custom physics block to.
        static string physicsLevelName = "ljplays";
        
        //This is a server-side block ID. Client-side this is 103.
        static BlockID wireBlock = 359;
        
        static BlockID placerDBlock = 361;
        static BlockID placerUBlock = 365;
        
        static BlockID lightOnBlock = 363;
        static BlockID lightOffBlock = 364;

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
                if (lvl.name == physicsLevelName) {
                    lvl.PhysicsHandlers[wireBlock] = DoWire;
                    
                    lvl.PhysicsHandlers[placerDBlock] = DoPlacerD;
                    lvl.PhysicsHandlers[placerUBlock] = DoPlacerU;
                    
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
            if (lvl.name != physicsLevelName) { return; }
            if (block == wireBlock) lvl.PhysicsHandlers[wireBlock] = DoWire;
           
            if (block == placerDBlock) lvl.PhysicsHandlers[placerDBlock] = DoPlacerD;
            if (block == placerUBlock) lvl.PhysicsHandlers[placerUBlock] = DoPlacerU;
            
            if (block == lightOnBlock) lvl.PhysicsHandlers[lightOnBlock] = Dolight;
            if (block == lightOffBlock) lvl.PhysicsHandlers[lightOffBlock] = Dolight;

            if (block == weyNBlock) lvl.PhysicsHandlers[weyNBlock] = Dowey;
            if (block == weySBlock) lvl.PhysicsHandlers[weySBlock] = Dowey;
            if (block == weyEBlock) lvl.PhysicsHandlers[weyEBlock] = Dowey;
            if (block == weyWBlock) lvl.PhysicsHandlers[weyWBlock] = Dowey;
        }

        void toggles(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            BlockID block = p.level.GetBlock(x, y, z);

            if (block != 360) return;
            if (button != MouseButton.Right) return;
            if (action == MouseAction.Pressed) return;
            Dotoggle(p, x, y, z);
        }

        static void Dowey(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            ushort yCur = y;

            int index;

            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (block == 369)//+x
            {
                lvl.GetBlock((ushort)(x + 1), y, z, out index);
                lvl.AddCheck(index);
            }
            else if (block == 370)//-x
            {
                lvl.GetBlock((ushort)(x - 1), y, z, out index);
                lvl.AddCheck(index);
            }
            else if (block == 368)//+z
            {
                lvl.GetBlock(x, y, (ushort)(z + 1), out index);
                lvl.AddCheck(index);
            }
            else if (block == 367)//-z
            { 
                lvl.GetBlock(x, y, (ushort)(z - 1), out index);
                lvl.AddCheck(index);
            }
            else if (block == 371)//+y
            {
                lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                lvl.AddCheck(index);
            }
            else if (block == 372)//-y
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
            if (block == 363)
            {
                block = 364;
                lvl.AddUpdate(index, block, true);
            }
            else if(block == 364) 
            {
                block = 363;
                lvl.AddUpdate(index, block, true);

                lvl.GetBlock((ushort)(x + 1), y, z, out index);
                lvl.AddCheck(index);

                lvl.GetBlock((ushort)(x - 1), y, z, out index);
                lvl.AddCheck(index);

                lvl.GetBlock(x, y, (ushort)(z + 1), out index);
                lvl.AddCheck(index);

                lvl.GetBlock(x, y, (ushort)(z - 1), out index);
                lvl.AddCheck(index);

                if (lvl.GetBlock(x, (ushort)(y + 1), z) != 361 && (lvl.GetBlock(x, (ushort)(y + 1), z) != 365))
                {
                    lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                    lvl.AddCheck(index);
                }
                if (lvl.GetBlock(x, (ushort)(y - 1), z) != 361 && (lvl.GetBlock(x, (ushort)(y - 1), z) != 365))
                {
                    lvl.GetBlock(x, (ushort)(y - 1), z, out index);
                    lvl.AddCheck(index);
                }
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }

        static void DoPlacerU(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            ushort yCur = y;
            BlockID below = lvl.GetBlock(x, (ushort)(y - 1), z);
            int index;
            lvl.GetBlock(x, (ushort)(y + 1), z, out index);
            lvl.AddUpdate(index, below, true);
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }

        static void DoPlacerD(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            ushort yCur = y;
            BlockID above = lvl.GetBlock(x, (ushort)(y + 1), z);
            int index;
            lvl.GetBlock(x, (ushort)(y - 1), z, out index);
            lvl.AddUpdate(index, above, true);
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
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

            if (lvl.GetBlock(x, (ushort)(y + 1), z) != 361 && (lvl.GetBlock(x, (ushort)(y + 1), z) != 365))
            {
                lvl.GetBlock(x, (ushort)(y + 1), z, out index);
                lvl.AddCheck(index);
            }
            if (lvl.GetBlock(x, (ushort)(y - 1), z) != 361 && (lvl.GetBlock(x, (ushort)(y - 1), z) != 365)) 
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
            Player debugger = PlayerInfo.FindExact(PluginPhysicsExample.author); if (debugger == null) { return; }
            debugger.Message(message, args);
        }
        
	}
	
}