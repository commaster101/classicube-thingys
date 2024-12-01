using System;
using System.Collections.Generic;
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy {
	
	public sealed class compLogic : Plugin {
		public override string name { get { return "compLogic"; } }
		public override string MCGalaxy_Version { get { return "1.9.4.1"; } }
        //This name is used to determine who to send debug text to
        static string author = "LJplays+";
		public override string creator { get { return author; } }
        
        //The MOTD to add custom physics to a Level.
        static string physicsMOTDName = "+logic";

        //the MOTD to make it blocks can't be made or deleted only moved
        static string infMOTDName = "-inflogic";

        //This is a server-side block IDs for all the blocks added

        const BlockID obsidian = 49;
        const BlockID bedrock = 7;
        const BlockID buttonBlock = 337;
        const BlockID buttonOnBlock = 342;

        //like a lamp lol
        //static BlockID lightOnBlock = 363;
        //static BlockID lightOffBlock = 364;

        //a basic arrow 
        const BlockID wireNBlock = 322;
        const BlockID wireEBlock = 323;
        const BlockID wireSBlock = 324;
        const BlockID wireWBlock = 325;
        const BlockID wireUBlock = 338;
        const BlockID wireDBlock = 339;
        
        const BlockID wireOnNBlock = 326;
        const BlockID wireOnEBlock = 327;
        const BlockID wireOnSBlock = 328;
        const BlockID wireOnWBlock = 329;
        const BlockID wireOnUBlock = 340;
        const BlockID wireOnDBlock = 341;

        const BlockID andBlock = 330;
        const BlockID notBlock = 331;
        const BlockID orBlock = 332;
        const BlockID nandBlock = 333;
        const BlockID norBlock = 334;
        const BlockID xorBlock = 335;
        const BlockID xnorBlock = 336;

        //You can find the server-side block ID in a custom command with:
        //Vec3S32 pos = p.Pos.FeetBlockCoords;
        //p.Message("Server-side BlockID at this location is {0}", p.level.GetBlock((ushort)pos.X, (ushort)pos.Y, (ushort)pos.Z));

        public override void Load(bool startup) {
            
            //The map we want to add a physics block to might already be loaded when the plugin starts, thus we have to add it right away in that case.
			Level[] levels = LevelInfo.Loaded.Items;
			foreach (Level lvl in levels) {
                if (lvl.Config.MOTD.ToLower().Contains(physicsMOTDName))
                {

                    //lvl.PhysicsHandlers[lightOnBlock] = Dolight;
                    //lvl.PhysicsHandlers[lightOffBlock] = Dolight;

                    lvl.PhysicsHandlers[wireNBlock] = Dowire;
                    lvl.PhysicsHandlers[wireSBlock] = Dowire;
                    lvl.PhysicsHandlers[wireEBlock] = Dowire;
                    lvl.PhysicsHandlers[wireWBlock] = Dowire;
                    lvl.PhysicsHandlers[wireUBlock] = Dowire;
                    lvl.PhysicsHandlers[wireDBlock] = Dowire;

                    lvl.PhysicsHandlers[wireOnNBlock] = DowireOn;
                    lvl.PhysicsHandlers[wireOnSBlock] = DowireOn;
                    lvl.PhysicsHandlers[wireOnEBlock] = DowireOn;
                    lvl.PhysicsHandlers[wireOnWBlock] = DowireOn;
                    lvl.PhysicsHandlers[wireOnUBlock] = DowireOn;
                    lvl.PhysicsHandlers[wireOnDBlock] = DowireOn;

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

            //if (block == lightOnBlock) lvl.PhysicsHandlers[lightOnBlock] = Dolight;
            //if (block == lightOffBlock) lvl.PhysicsHandlers[lightOffBlock] = Dolight;

            if (block == wireNBlock) lvl.PhysicsHandlers[wireNBlock] = Dowire;
            if (block == wireSBlock) lvl.PhysicsHandlers[wireSBlock] = Dowire;
            if (block == wireEBlock) lvl.PhysicsHandlers[wireEBlock] = Dowire;
            if (block == wireWBlock) lvl.PhysicsHandlers[wireWBlock] = Dowire;
            if (block == wireUBlock) lvl.PhysicsHandlers[wireUBlock] = Dowire;
            if (block == wireDBlock) lvl.PhysicsHandlers[wireDBlock] = Dowire;

            if (block == wireOnNBlock) lvl.PhysicsHandlers[wireOnNBlock] = DowireOn;
            if (block == wireOnSBlock) lvl.PhysicsHandlers[wireOnSBlock] = DowireOn;
            if (block == wireOnEBlock) lvl.PhysicsHandlers[wireOnEBlock] = DowireOn;
            if (block == wireOnWBlock) lvl.PhysicsHandlers[wireOnWBlock] = DowireOn;
            if (block == wireOnUBlock) lvl.PhysicsHandlers[wireOnUBlock] = DowireOn;
            if (block == wireOnDBlock) lvl.PhysicsHandlers[wireOnDBlock] = DowireOn;
        }

        public enum Facing
        {
            North,
            East,
            South,
            West,
            Up,
            Down
        }

        void toggles(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            BlockID block = p.level.GetBlock(x, y, z);

            if (block == buttonBlock)
            {
                if (button != MouseButton.Right) return;
                if (action == MouseAction.Pressed) return;
                Dotoggle(p, x, y, z);
            }
            if (block == buttonOnBlock)
            {
                if (button != MouseButton.Right) return;
                if (action == MouseAction.Pressed) return;
                Dotoggle(p, x, y, z);
            }
        }
        //turn on wires
        static void Dowire(Level lvl, ref PhysInfo C)
        {
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
            BlockID block = lvl.GetBlock(x, y, z, out index);
            switch (block)
            {
                case wireNBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.North) == 0) break;
                    lvl.AddUpdate(index, wireOnNBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireEBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.East) == 0) break;
                    lvl.AddUpdate(index, wireOnEBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireSBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.South) == 0) break;
                    lvl.AddUpdate(index, wireOnSBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireWBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.West) == 0) break;
                    lvl.AddUpdate(index, wireOnWBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireUBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.Up) == 0) break;
                    lvl.AddUpdate(index, wireOnUBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireDBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.Down) == 0) break;
                    lvl.AddUpdate(index, wireOnDBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
            }
            //C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
        //turn off wires
        static void DowireOn(Level lvl, ref PhysInfo C)
        {
            ushort x = C.X, y = C.Y, z = C.Z;
            //int index = C.Index;
            ushort yCur = y;

            int index;
            BlockID block = lvl.GetBlock(x, y, z, out index);
            switch (block)
            {
                case wireOnNBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.North) != 0) break;
                    lvl.AddUpdate(index, wireNBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireOnEBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.East) != 0) break;
                    lvl.AddUpdate(index, wireEBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireOnSBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.South) != 0) break;
                    lvl.AddUpdate(index, wireSBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireOnWBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.West) != 0) break;
                    lvl.AddUpdate(index, wireWBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireOnUBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.Up) != 0) break;
                    lvl.AddUpdate(index, wireUBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
                case wireOnDBlock:
                    if (getInputsWire(lvl, x, y, z, Facing.Down) != 0) break;
                    lvl.AddUpdate(index, wireDBlock, true);
                    lvl.AddCheck(index);
                    updateNextTo(lvl, x, y, z);
                    break;
            }
            //C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
        //copy of wire but is activated on click
        static void Dotoggle(Player p, ushort x, ushort y, ushort z)
        {
            Level lvl = p.level;

            int index;

            BlockID block = lvl.GetBlock(x, y, z, out index);
            if (block == buttonOnBlock)
            {
                block = buttonBlock;
                lvl.AddUpdate(index, block, true);
            }
            else if (block == buttonBlock)
            {
                block = buttonOnBlock;
                lvl.AddUpdate(index, block, true);
            }

            bool instant = true;
            ActivateablePhysics.DoDoors(lvl, (ushort)(x + 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, (ushort)(x - 1), y, z, instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z + 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, y, (ushort)(z - 1), instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y - 1), z, instant);
            ActivateablePhysics.DoDoors(lvl, x, (ushort)(y + 1), z, instant);

            powerNextTo(lvl, x, y, z);
        }

        static int getInputsWire(Level lvl,ushort x,ushort y, ushort z, Facing facing)
        {
            int inputs = 0;
            if ((lvl.GetBlock((ushort)(x + 1), y, z) == wireOnWBlock) && facing != Facing.East && facing != Facing.West) inputs++;
            if ((lvl.GetBlock((ushort)(x - 1), y, z) == wireOnEBlock) && facing != Facing.East && facing != Facing.West) inputs++;
            if ((lvl.GetBlock(x, y, (ushort)(z + 1)) == wireOnNBlock) && facing != Facing.North && facing != Facing.South) inputs++;
            if ((lvl.GetBlock(x, y, (ushort)(z - 1)) == wireOnSBlock) && facing != Facing.North && facing != Facing.South) inputs++;
            if ((lvl.GetBlock(x, (ushort)(y + 1), z) == wireOnDBlock) && facing != Facing.Up && facing != Facing.Down) inputs++;
            if ((lvl.GetBlock(x, (ushort)(y - 1), z) == wireOnUBlock) && facing != Facing.Up && facing != Facing.Down) inputs++;

            if ((lvl.GetBlock((ushort)(x + 1), y, z) == buttonOnBlock) && facing != Facing.East) inputs++;
            if ((lvl.GetBlock((ushort)(x - 1), y, z) == buttonOnBlock) && facing != Facing.West) inputs++;
            if ((lvl.GetBlock(x, y, (ushort)(z + 1)) == buttonOnBlock) && facing != Facing.South) inputs++;
            if ((lvl.GetBlock(x, y, (ushort)(z - 1)) == buttonOnBlock) && facing != Facing.North) inputs++;
            if ((lvl.GetBlock(x, (ushort)(y - 1), z) == buttonOnBlock) && facing != Facing.Down) inputs++;
            if ((lvl.GetBlock(x, (ushort)(y + 1), z) == buttonOnBlock) && facing != Facing.Up) inputs++;

            switch(facing)
            {
                case Facing.East:
                    if (IsBlockPowered(lvl,(ushort)(x - 1), y, z)) inputs++;
                    break;
                case Facing.West:
                    if (IsBlockPowered(lvl,(ushort)(x + 1), y, z)) inputs++;
                    break;
                case Facing.North:
                    if (IsBlockPowered(lvl, x, y, (ushort)(z + 1))) inputs++;
                    break;
                case Facing.South:
                    if (IsBlockPowered(lvl, x, y, (ushort)(z - 1))) inputs++;
                    break;
                case Facing.Up:
                    if (IsBlockPowered(lvl, x, (ushort)(y - 1), z)) inputs++;
                    break;
                case Facing.Down:
                    if (IsBlockPowered(lvl, x, (ushort)(y + 1), z)) inputs++;
                    break;
            }
            return inputs;
        }
        static int getInputsLogic(Level lvl, ushort x, ushort y, ushort z,bool On)
        {
            int inputs = 0;
            if(On == true)
            {
                if (lvl.GetBlock((ushort)(x + 1), y, z) == wireOnWBlock) inputs++;
                if (lvl.GetBlock((ushort)(x - 1), y, z) == wireOnEBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z + 1)) == wireOnNBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z - 1)) == wireOnSBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y + 1), z) == wireOnDBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y - 1), z) == wireOnUBlock) inputs++;

                if (lvl.GetBlock((ushort)(x + 1), y, z) == buttonOnBlock) inputs++;
                if (lvl.GetBlock((ushort)(x - 1), y, z) == buttonOnBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z + 1)) == buttonOnBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z - 1)) == buttonOnBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y - 1), z) == buttonOnBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y + 1), z) == buttonOnBlock) inputs++;
            }
            else
            {
                if (lvl.GetBlock((ushort)(x + 1), y, z) == wireWBlock) inputs++;
                if (lvl.GetBlock((ushort)(x - 1), y, z) == wireEBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z + 1)) == wireNBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z - 1)) == wireSBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y + 1), z) == wireDBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y - 1), z) == wireUBlock) inputs++;

                if (lvl.GetBlock((ushort)(x + 1), y, z) == buttonBlock) inputs++;
                if (lvl.GetBlock((ushort)(x - 1), y, z) == buttonBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z + 1)) == buttonBlock) inputs++;
                if (lvl.GetBlock(x, y, (ushort)(z - 1)) == buttonBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y - 1), z) == buttonBlock) inputs++;
                if (lvl.GetBlock(x, (ushort)(y + 1), z) == buttonBlock) inputs++;
            }
            return inputs;
        }
        static void powerBlock(Level lvl,ushort x,ushort y,ushort z)
        {
            int index;
            BlockID block = lvl.GetBlock(x, y, z, out index);    
            lvl.AddCheck(index);
        }
        static void powerNextTo(Level lvl,ushort x,ushort y,ushort z)
        {
            powerBlock(lvl, (ushort)(x + 1), y, z);
            powerBlock(lvl, (ushort)(x - 1), y, z);
            powerBlock(lvl, x, y, (ushort)(z + 1));
            powerBlock(lvl, x, y, (ushort)(z - 1));
            powerBlock(lvl, x, (ushort)(y + 1), z);
            powerBlock(lvl, x, (ushort)(y - 1), z);
        }
        static void updateNextTo(Level lvl, ushort x, ushort y, ushort z)
        {
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

        static bool IsBlockPowered(Level lvl, ushort x, ushort y, ushort z)
        {
            BlockID block = lvl.GetBlock(x, y, z);
            switch (block)
            {
                case wireOnNBlock: return true;
                case wireOnEBlock: return true;
                case wireOnSBlock: return true;
                case wireOnWBlock: return true;
                case wireOnUBlock: return true;
                case wireOnDBlock: return true;
                case andBlock: if (getInputsLogic(lvl, x, y, z, false) == 0 && getInputsLogic(lvl, x, y, z, true) != 0) return true;
                    else return false;
                case notBlock:
                    if (getInputsLogic(lvl, x, y, z, true) == 0) return true;
                    else return false;
                case orBlock:
                    if (getInputsLogic(lvl, x, y, z, true) > 0) return true;
                    else return false;
                case nandBlock:
                    if (getInputsLogic(lvl, x, y, z, false) == 0 && getInputsLogic(lvl, x, y, z, true) != 0) return false;
                    else return true;
                case norBlock:
                    if (getInputsLogic(lvl, x, y, z, true) > 0) return false;
                    else return true;
                case xorBlock:
                    if (getInputsLogic(lvl, x, y, z, true) == 1) return true;
                    else return false;
                case xnorBlock:
                    if (getInputsLogic(lvl, x, y, z, true) == 1) return false;
                    else return true;
                default: return false;
            }
        }

        static void MsgDebugger(string message, params object[] args) {
            Player debugger = PlayerInfo.FindExact(illLogic.author); if (debugger == null) { return; }
            debugger.Message(message, args);
        }
        
	}
	
}
