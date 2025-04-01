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
        static string author = "LJplays";
		public override string creator { get { return author; } }
        
        //The MOTD to add custom physics to a Level.
        //Note: Not in use
        static string physicsMOTDName = "+logic";

        const int pistonActionLimit = 16;

        //This is a server-side block IDs for all the blocks added
        const BlockID obsidian = 49;
        const BlockID bedrock = 7;
        const BlockID buttonBlock = 337;
        const BlockID buttonOnBlock = 342;

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

        const BlockID doorClosedBlock = 435;
        const BlockID doorOpenBlock = 436;

        const BlockID pistonNBlock = 437;
        const BlockID pistonEBlock = 438;
        const BlockID pistonSBlock = 439;
        const BlockID pistonWBlock = 440;
        const BlockID pistonUBlock = 441;
        const BlockID pistonDBlock = 442;
        const BlockID pistonHeadBlock = 443;

        const BlockID stickyPistonNBlock = 444;
        const BlockID stickyPistonEBlock = 445;
        const BlockID stickyPistonSBlock = 446;
        const BlockID stickyPistonWBlock = 447;
        const BlockID stickyPistonUBlock = 448;
        const BlockID stickyPistonDBlock = 449;
        const BlockID stickyPistonHeadBlock = 450;
        const BlockID delay1TBlock = 451;
        const BlockID delay2TBlock = 452;
        const BlockID delay3TBlock = 453;
        const BlockID delay4TBlock = 454;
        static readonly BlockID[] stickyPistonSet = {stickyPistonNBlock,stickyPistonSBlock,stickyPistonEBlock,stickyPistonWBlock,stickyPistonUBlock,stickyPistonDBlock};
        static readonly BlockID[] pistonSet = {pistonNBlock,pistonSBlock,pistonEBlock,pistonWBlock,pistonUBlock,pistonDBlock};
        static readonly BlockID[] pistonHeadSet = {stickyPistonHeadBlock,pistonHeadBlock};
        static readonly BlockID[] nonPushableSet = {obsidian,bedrock,Block.Invalid};
        static readonly BlockID[] dontOverrideSet = {delay1TBlock,delay2TBlock,delay3TBlock,delay4TBlock};
        //You can find the server-side block ID in a custom command with:
        //Vec3S32 pos = p.Pos.FeetBlockCoords;
        //p.Message("Server-side BlockID at this location is {0}", p.level.FastGetBlock((ushort)pos.X, (ushort)pos.Y, (ushort)pos.Z));
        public override void Load(bool startup) {
            
            //The map we want to add a physics block to might already be loaded when the plugin starts, thus we have to add it right away in that case.
			Level[] levels = LevelInfo.Loaded.Items;
			foreach (Level lvl in levels) {
                AddCustomPhysicsToLevel(lvl);
			}
            
            //Otherwise, we will look for when it loads using an event
            OnBlockChangedEvent.Register(OnBlockChanged, Priority.Low);
            OnBlockHandlersUpdatedEvent.Register(OnBlockHandlersUpdated, Priority.Low);
            OnPhysicsLevelChangedEvent.Register(OnPhysicsLevelChanged, Priority.Low);
            OnPlayerClickEvent.Register(toggles, Priority.Low);
        }
		public override void Unload(bool shutdown) {
            OnBlockChangedEvent.Unregister(OnBlockChanged);
            OnBlockHandlersUpdatedEvent.Unregister(OnBlockHandlersUpdated);
            OnPhysicsLevelChangedEvent.Unregister(OnPhysicsLevelChanged);
            OnPlayerClickEvent.Unregister(toggles);
        }
        static void OnPhysicsLevelChanged(Level lvl, int level){
            AddCustomPhysicsToLevel(lvl);
        }
		static void OnBlockHandlersUpdated(Level lvl, BlockID block) {
            AddCustomPhysicsToLevel(lvl);
        }
        static void AddCustomPhysicsToLevel(Level lvl)
        {
            lvl.PhysicsHandlers[wireNBlock] = Dowire;
            lvl.PhysicsHandlers[wireSBlock] = Dowire;
            lvl.PhysicsHandlers[wireEBlock] = Dowire;
            lvl.PhysicsHandlers[wireWBlock] = Dowire;
            lvl.PhysicsHandlers[wireUBlock] = Dowire;
            lvl.PhysicsHandlers[wireDBlock] = Dowire;

            lvl.PhysicsHandlers[wireOnNBlock] = Dowire;
            lvl.PhysicsHandlers[wireOnSBlock] = Dowire;
            lvl.PhysicsHandlers[wireOnEBlock] = Dowire;
            lvl.PhysicsHandlers[wireOnWBlock] = Dowire;
            lvl.PhysicsHandlers[wireOnUBlock] = Dowire;
            lvl.PhysicsHandlers[wireOnDBlock] = Dowire;

            lvl.PhysicsHandlers[pistonNBlock] = Dopiston;
            lvl.PhysicsHandlers[pistonSBlock] = Dopiston;
            lvl.PhysicsHandlers[pistonEBlock] = Dopiston;
            lvl.PhysicsHandlers[pistonWBlock] = Dopiston;
            lvl.PhysicsHandlers[pistonUBlock] = Dopiston;
            lvl.PhysicsHandlers[pistonDBlock] = Dopiston;

            lvl.PhysicsHandlers[stickyPistonNBlock] = Dopiston;
            lvl.PhysicsHandlers[stickyPistonSBlock] = Dopiston;
            lvl.PhysicsHandlers[stickyPistonEBlock] = Dopiston;
            lvl.PhysicsHandlers[stickyPistonWBlock] = Dopiston;
            lvl.PhysicsHandlers[stickyPistonUBlock] = Dopiston;
            lvl.PhysicsHandlers[stickyPistonDBlock] = Dopiston;

            lvl.PhysicsHandlers[stickyPistonHeadBlock] = DoPistonHead;
            lvl.PhysicsHandlers[pistonHeadBlock] = DoPistonHead;

            lvl.PhysicsHandlers[doorClosedBlock] = DoDoorClosedBlock;
            lvl.PhysicsHandlers[doorOpenBlock] = DoDoorOpenBlock;

            lvl.PhysicsHandlers[delay1TBlock] = DoDelay;
            lvl.PhysicsHandlers[delay2TBlock] = DoDelay;
            lvl.PhysicsHandlers[delay3TBlock] = DoDelay;
            lvl.PhysicsHandlers[delay4TBlock] = DoDelay;
        }

        public enum Facing
        {
            North,
            East,
            South,
            West,
            Up,
            Down,
            None
        }
        void OnBlockChanged(Player p, ushort x, ushort y, ushort z, ChangeResult result)
        {
            Level lvl = p.level;
            AddCheck(lvl,lvl.IntOffset(lvl.PosToInt(x, y, z), 1, 0, 0),false);
            AddCheck(lvl,lvl.IntOffset(lvl.PosToInt(x, y, z), -1, 0, 0),false);
            AddCheck(lvl,lvl.IntOffset(lvl.PosToInt(x, y, z), 0, 1, 0),false);
            AddCheck(lvl,lvl.IntOffset(lvl.PosToInt(x, y, z), 0, -1, 0),false);
            AddCheck(lvl,lvl.IntOffset(lvl.PosToInt(x, y, z), 0, 0, 1),false);
            AddCheck(lvl,lvl.IntOffset(lvl.PosToInt(x, y, z), 0, 0, -1),false);
        }

        void toggles(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            if (button != MouseButton.Right) return;
            if (action == MouseAction.Pressed) return;
            BlockID block;
            try {block = p.level.FastGetBlock(x,y,z);}
            catch(Exception e) {return;}//player didn't click a vaild block position

            if (block == buttonBlock) Dotoggle(p, p.level, p.level.PosToInt(x,y,z));
            else if (block == buttonOnBlock) Dotoggle(p, p.level, p.level.PosToInt(x,y,z));
        }
        static void DoDelay(Level lvl, ref PhysInfo C)
        {
            byte test = C.Data.Data;
            if (getInputsLogic(lvl, C.Index, true) == 0) 
            {
                updateNextTo(lvl, C.Index, Facing.None);
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
                return;
            }
            switch(C.Block)
            {
                case delay1TBlock: C.Data.Value1 = 1; break;
                case delay2TBlock: C.Data.Value1 = 2; break;
                case delay3TBlock: C.Data.Value1 = 3; break;
                case delay4TBlock: C.Data.Value1 = 4; break;
            }
            if (C.Data.Data < C.Data.Value1) { // value1 for wait time
                C.Data.Data++;
            } 
            else 
            {
                updateNextTo(lvl, C.Index, Facing.None);
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
        }
        static void DoDoorClosedBlock(Level lvl, ref PhysInfo C)
        {
            int index = C.Index;
            if (getInputsLogic(lvl, C.Index, true) == 0) return;
            lvl.AddUpdate(index, doorOpenBlock, true);
            lvl.AddCheck(index,true);
        }
        static void DoDoorOpenBlock(Level lvl, ref PhysInfo C)
        {
            int index = C.Index;
            if (getInputsLogic(lvl, C.Index, true) != 0) return;
            lvl.AddUpdate(index, doorClosedBlock, true);
            lvl.AddCheck(index,true);
        }
        //turn on wires
        static void Dowire(Level lvl, ref PhysInfo C)
        {
            int index = C.Index;
            BlockID block = C.Block;
            switch (block)
            {
                case wireNBlock:
                    if (getInputsWire(lvl, index, Facing.North) == 0) break;
                    updateNextTo(lvl, index, Facing.North);
                    lvl.AddUpdate(index, wireOnNBlock, true);
                    break;
                case wireEBlock:
                    if (getInputsWire(lvl, index, Facing.East) == 0) break;
                    lvl.AddUpdate(index, wireOnEBlock, true);
                    updateNextTo(lvl, index, Facing.East);
                    break;
                case wireSBlock:
                    if (getInputsWire(lvl, index, Facing.South) == 0) break;
                    lvl.AddUpdate(index, wireOnSBlock, true);
                    updateNextTo(lvl, index, Facing.South);
                    break;
                case wireWBlock:
                    if (getInputsWire(lvl, index, Facing.West) == 0) break;
                    lvl.AddUpdate(index, wireOnWBlock, true);
                    updateNextTo(lvl, index, Facing.West);
                    break;
                case wireUBlock:
                    if (getInputsWire(lvl, index, Facing.Up) == 0) break;
                    lvl.AddUpdate(index, wireOnUBlock, true);
                    updateNextTo(lvl, index, Facing.Up);
                    break;
                case wireDBlock:
                    if (getInputsWire(lvl, index, Facing.Down) == 0) break;
                    lvl.AddUpdate(index, wireOnDBlock, true);
                    updateNextTo(lvl, index, Facing.Down);
                    break;
                case wireOnNBlock:
                    if (getInputsWire(lvl, index, Facing.North) != 0) break;
                    lvl.AddUpdate(index, wireNBlock, true);
                    updateNextTo(lvl, index, Facing.North);
                    break;
                case wireOnEBlock:
                    if (getInputsWire(lvl, index, Facing.East) != 0) break;
                    lvl.AddUpdate(index, wireEBlock, true);
                    updateNextTo(lvl, index, Facing.East);
                    break;
                case wireOnSBlock:
                    if (getInputsWire(lvl, index, Facing.South) != 0) break;
                    lvl.AddUpdate(index, wireSBlock, true);
                    updateNextTo(lvl, index, Facing.South);
                    break;
                case wireOnWBlock:
                    if (getInputsWire(lvl, index, Facing.West) != 0) break;
                    lvl.AddUpdate(index, wireWBlock, true);
                    updateNextTo(lvl, index, Facing.West);
                    break;
                case wireOnUBlock:
                    if (getInputsWire(lvl, index, Facing.Up) != 0) break;
                    lvl.AddUpdate(index, wireUBlock, true);
                    updateNextTo(lvl, index, Facing.Up);
                    break;
                case wireOnDBlock:
                    if (getInputsWire(lvl, index, Facing.Down) != 0) break;
                    lvl.AddUpdate(index, wireDBlock, true);
                    updateNextTo(lvl, index, Facing.Down);
                    break;
            }
            lvl.AddCheck(index,true);
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
        
        static void Dopiston(Level lvl, ref PhysInfo C)
        {
            int index = C.Index;
            BlockID block = C.Block;
            switch (block)
            {
                case pistonNBlock:
                case stickyPistonNBlock:
                    pistonAction(lvl, C,0,0,-1,Facing.North);
                break;
                case pistonSBlock:
                case stickyPistonSBlock:
                    pistonAction(lvl, C,0,0,1,Facing.South);
                break;
                case pistonEBlock:
                case stickyPistonEBlock:
                    pistonAction(lvl, C,1,0,0,Facing.East);
                break;
                case pistonWBlock:
                case stickyPistonWBlock:
                    pistonAction(lvl, C,-1,0,0,Facing.West);
                break;
                case pistonUBlock:
                case stickyPistonUBlock:
                    pistonAction(lvl, C,0,1,0,Facing.Up);
                break;
                case pistonDBlock:
                case stickyPistonDBlock:
                    pistonAction(lvl, C,0,-1,0,Facing.Down);
                break;
            }
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
        }
        static void pistonAction(Level lvl, PhysInfo C,int xOffset,int yOffset,int zOffset,Facing facing)
        {
            BlockID lastBlock;
            int frontIndex = lvl.IntOffset(C.Index,xOffset,yOffset,zOffset);
            BlockID frontBlock = lvl.FastGetBlock(frontIndex);
            BlockID pullBlock = lvl.FastGetBlock(lvl.IntOffset(frontIndex,xOffset,yOffset,zOffset));
            BlockID pistonBlock = C.Block;
            if(getInputsWire(lvl, C.Index, facing) == 0 && (SetContains(pistonHeadSet,frontBlock)))
            {
                if (SetContains(stickyPistonSet,pistonBlock) && !(SetContains(nonPushableSet,pullBlock) || SetContains(pistonHeadSet,pullBlock)))
                {
                    lvl.AddUpdate(frontIndex, pullBlock, true);
                    lvl.AddUpdate(lvl.IntOffset(frontIndex,xOffset,yOffset,zOffset), Block.Air, true);
                }
                else lvl.AddUpdate(frontIndex, Block.Air, true);
            }
            else if(getInputsWire(lvl,C.Index, facing) != 0 && !SetContains(pistonHeadSet,frontBlock))
            {
                if (canPistonPush(lvl,frontIndex,xOffset,yOffset,zOffset) == false) return;
                if (SetContains(stickyPistonSet,pistonBlock)) lastBlock = stickyPistonHeadBlock;
                else lastBlock = pistonHeadBlock;
                for(int i = 0;i<=pistonActionLimit;i++)
                {
                    BlockID curBlock = lvl.FastGetBlock(lvl.IntOffset(frontIndex,xOffset*i,yOffset*i,zOffset*i));
                    lvl.AddUpdate(lvl.IntOffset(frontIndex,xOffset*i,yOffset*i,zOffset*i), lastBlock, true);
                    if (curBlock == Block.Air) return;
                    lastBlock = curBlock;
                    powerNextTo(lvl,lvl.IntOffset(frontIndex,xOffset*i,yOffset*i,zOffset*i));
                }
            }
        }
        static bool canPistonPush(Level lvl, int index,int xOffset,int yOffset,int zOffset)
        {
            for(int i = 0;i<pistonActionLimit;i++)
            {
                BlockID curBlock = lvl.FastGetBlock(lvl.IntOffset(index,xOffset*i,yOffset*i,zOffset*i));
                if (curBlock == Block.Air) return true;
                ushort x,y,z;
                lvl.IntToPos(index,out x,out y,out z);
                if (SetContains(nonPushableSet,lvl.GetBlock((ushort)(x+xOffset*i),(ushort)(y+yOffset*i),(ushort)(z+zOffset*i)))) return false;
            }
            return false;
        }
        static void DoPistonHead(Level lvl, ref PhysInfo C)
        {
            C.Data.Data = PhysicsArgs.RemoveFromChecks;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,1,0,0)) == pistonWBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,-1,0,0))== pistonEBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,1,0)) == pistonDBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,-1,0))== pistonUBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,0,1)) == pistonNBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,0,-1))== pistonSBlock)return;

            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,1,0,0)) == stickyPistonWBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,-1,0,0))== stickyPistonEBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,1,0)) == stickyPistonDBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,-1,0))== stickyPistonUBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,0,1)) == stickyPistonNBlock)return;
            if (lvl.FastGetBlock(lvl.IntOffset(C.Index,0,0,-1))== stickyPistonSBlock)return;
            
            lvl.AddUpdate(C.Index, Block.Air, false);
        }
        //copy of wire but is activated on click
        static void Dotoggle(Player p,Level lvl, int index)
        {
            if (lvl.Config.MOTD.ToLower().Contains("+permtoggles") && !p.AllowBuild) {p.Message("%cYou can't use buttons in this level!"); return;}
            BlockID block = lvl.FastGetBlock(index);
            if (block == buttonOnBlock) lvl.AddUpdate(index, buttonBlock, true);
            else if (block == buttonBlock) lvl.AddUpdate(index, buttonOnBlock, true);
            powerNextTo(lvl,index);
        }

        static int getInputsWire(Level lvl,int index, Facing facing)
        {
            int inputs = 0;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,1,0,0)) == wireOnWBlock) && facing != Facing.East && facing != Facing.West) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,-1,0,0)) == wireOnEBlock) && facing != Facing.East && facing != Facing.West) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,1,0)) == wireOnDBlock) && facing != Facing.Up && facing != Facing.Down) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,-1,0)) == wireOnUBlock) && facing != Facing.Up && facing != Facing.Down) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,0,1)) == wireOnNBlock) && facing != Facing.North && facing != Facing.South) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,0,-1)) == wireOnSBlock) && facing != Facing.North && facing != Facing.South) inputs++;

            if ((lvl.FastGetBlock(lvl.IntOffset(index,1,0,0)) == buttonOnBlock) && facing != Facing.East) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,-1,0,0)) == buttonOnBlock) && facing != Facing.West) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,1,0)) == buttonOnBlock) && facing != Facing.Up) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,-1,0)) == buttonOnBlock) && facing != Facing.Down) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,0,1)) == buttonOnBlock) && facing != Facing.South) inputs++;
            if ((lvl.FastGetBlock(lvl.IntOffset(index,0,0,-1)) == buttonOnBlock) && facing != Facing.North) inputs++;

            switch(facing)
            {
                case Facing.East:
                    if (IsBlockPowered(lvl, lvl.IntOffset(index,-1,0,0))) inputs++;
                    break;
                case Facing.West:
                    if (IsBlockPowered(lvl, lvl.IntOffset(index,1,0,0))) inputs++;
                    break;
                case Facing.Up:
                    if (IsBlockPowered(lvl, lvl.IntOffset(index,0,-1,0))) inputs++;
                    break;
                case Facing.Down:
                    if (IsBlockPowered(lvl, lvl.IntOffset(index,0,1,0))) inputs++;
                    break;
                case Facing.South:
                    if (IsBlockPowered(lvl, lvl.IntOffset(index,0,0,-1))) inputs++;
                    break;
                case Facing.North:
                    if (IsBlockPowered(lvl, lvl.IntOffset(index,0,0,1))) inputs++;
                    break;
            }
            return inputs;
        }
        static bool IsBlockPowered(Level lvl, int index)
        {
            BlockID block = lvl.FastGetBlock(index);
            switch (block)
            {
                case wireOnNBlock: return true;
                case wireOnEBlock: return true;
                case wireOnSBlock: return true;
                case wireOnWBlock: return true;
                case wireOnUBlock: return true;
                case wireOnDBlock: return true;
                case andBlock: if (getInputsLogic(lvl, index, false) == 0 && getInputsLogic(lvl, index, true) != 0) return true;
                    else return false;
                case notBlock:
                    if (getInputsLogic(lvl, index, true) == 0) return true;
                    else return false;
                case orBlock:
                    if (getInputsLogic(lvl, index, true) > 0) return true;
                    else return false;
                case nandBlock:
                    if (getInputsLogic(lvl, index, false) == 0 && getInputsLogic(lvl, index, true) != 0) return false;
                    else return true;
                case norBlock:
                    if (getInputsLogic(lvl, index, true) > 0) return false;
                    else return true;
                case xorBlock:
                    if (getInputsLogic(lvl, index, true) == 1) return true;
                    else return false;
                case xnorBlock:
                    if (getInputsLogic(lvl, index, true) == 1) return false;
                    else return true;
                case delay1TBlock:
                case delay2TBlock:
                case delay3TBlock:
                case delay4TBlock:
                    if (getInputsLogic(lvl, index, true) == 0) return false;
                    else return true;
                default: return false;
            }
        }
        static int getInputsLogic(Level lvl, int index, bool On)
        {
            int inputs = 0;
            if(On == true)
            {
                if (lvl.FastGetBlock(lvl.IntOffset(index,1,0,0)) == wireOnWBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,-1,0,0)) == wireOnEBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,1,0)) == wireOnDBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,-1,0)) == wireOnUBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,1)) == wireOnNBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,-1)) == wireOnSBlock) inputs++;

                if (lvl.FastGetBlock(lvl.IntOffset(index,1,0,0)) == buttonOnBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,-1,0,0)) == buttonOnBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,1,0)) == buttonOnBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,-1,0)) == buttonOnBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,1)) == buttonOnBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,-1)) == buttonOnBlock) inputs++;
            }
            else
            {
                if (lvl.FastGetBlock(lvl.IntOffset(index,1,0,0)) == wireWBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,-1,0,0)) == wireEBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,1,0)) == wireDBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,-1,0)) == wireUBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,1)) == wireNBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,-1)) == wireSBlock) inputs++;

                if (lvl.FastGetBlock(lvl.IntOffset(index,1,0,0)) == buttonBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,-1,0,0)) == buttonBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,1,0)) == buttonBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,-1,0)) == buttonBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,1)) == buttonBlock) inputs++;
                if (lvl.FastGetBlock(lvl.IntOffset(index,0,0,-1)) == buttonBlock) inputs++;
            }
            return inputs;
        }
        static void powerNextTo(Level lvl,int index)
        {
            AddCheck(lvl,lvl.IntOffset(index, 1, 0, 0),true);
            AddCheck(lvl,lvl.IntOffset(index, -1, 0, 0),true);
            AddCheck(lvl,lvl.IntOffset(index, 0, 1, 0),true);
            AddCheck(lvl,lvl.IntOffset(index, 0, -1, 0),true);
            AddCheck(lvl,lvl.IntOffset(index, 0, 0, 1),true);
            AddCheck(lvl,lvl.IntOffset(index, 0, 0, -1),true);
        }
        static void updateNextTo(Level lvl, int index, Facing facing)
        {
            if (SetContains(new BlockID[] {wireOnEBlock,wireEBlock,pistonEBlock,stickyPistonEBlock},lvl.FastGetBlock(lvl.IntOffset(index, 1,0,0))) && facing != Facing.East && facing != Facing.West){
                AddCheck(lvl,lvl.IntOffset(index, 1,0,0),true);}
            if (SetContains(new BlockID[] {wireOnWBlock,wireWBlock,pistonWBlock,stickyPistonWBlock},lvl.FastGetBlock(lvl.IntOffset(index, -1,0,0))) && facing != Facing.East && facing != Facing.West){
                AddCheck(lvl,lvl.IntOffset(index, -1,0,0),true);}
            if (SetContains(new BlockID[] {wireOnUBlock,wireUBlock,pistonUBlock,stickyPistonUBlock},lvl.FastGetBlock(lvl.IntOffset(index, 0,1,0))) && facing != Facing.Up && facing != Facing.Down){
                AddCheck(lvl,lvl.IntOffset(index, 0,1,0),true);}
            if (SetContains(new BlockID[] {wireOnDBlock,wireDBlock,pistonDBlock,stickyPistonDBlock},lvl.FastGetBlock(lvl.IntOffset(index, 0,-1,0))) && facing != Facing.Up && facing != Facing.Down){
                AddCheck(lvl,lvl.IntOffset(index, 0,-1,0),true);}
            if (SetContains(new BlockID[] {wireOnSBlock,wireSBlock,pistonSBlock,stickyPistonSBlock},lvl.FastGetBlock(lvl.IntOffset(index, 0,0,1))) && facing != Facing.North && facing != Facing.South){
                AddCheck(lvl,lvl.IntOffset(index, 0,0,1),true);}
            if (SetContains(new BlockID[] {wireOnNBlock,wireNBlock,pistonNBlock,stickyPistonNBlock},lvl.FastGetBlock(lvl.IntOffset(index, 0,0,-1))) && facing != Facing.North && facing != Facing.South){
                AddCheck(lvl,lvl.IntOffset(index, 0,0,-1),true);}
            switch (facing)
            {
                case Facing.East:
                    AddCheck(lvl,lvl.IntOffset(index, 1,0,0),true);
                    if (IsBlockLogic(lvl.FastGetBlock(lvl.IntOffset(index, 1,0,0)))) updateNextTo(lvl, lvl.IntOffset(index, 1,0,0), Facing.None);
                    break;
                case Facing.West:
                    AddCheck(lvl,lvl.IntOffset(index, -1,0,0),true);
                    if (IsBlockLogic(lvl.FastGetBlock(lvl.IntOffset(index, -1,0,0)))) updateNextTo(lvl, lvl.IntOffset(index, -1,0,0), Facing.None);
                    break;
                case Facing.Up:
                    AddCheck(lvl,lvl.IntOffset(index, 0,1,0),true);
                    if (IsBlockLogic(lvl.FastGetBlock(lvl.IntOffset(index, 0,1,0)))) updateNextTo(lvl, lvl.IntOffset(index, 0,1,0), Facing.None);
                    break;
                case Facing.Down:
                    AddCheck(lvl,lvl.IntOffset(index, 0,-1,0),true);
                    if (IsBlockLogic(lvl.FastGetBlock(lvl.IntOffset(index, 0,-1,0)))) updateNextTo(lvl, lvl.IntOffset(index, 0,-1,0), Facing.None);
                    break;
                case Facing.South:
                    AddCheck(lvl,lvl.IntOffset(index, 0,0,1),true);
                    if (IsBlockLogic(lvl.FastGetBlock(lvl.IntOffset(index, 0,0,1)))) updateNextTo(lvl, lvl.IntOffset(index, 0,0,1), Facing.None);
                    break;
                case Facing.North:
                    AddCheck(lvl,lvl.IntOffset(index, 0,0,-1),true);
                    if (IsBlockLogic(lvl.FastGetBlock(lvl.IntOffset(index, 0,0,-1)))) updateNextTo(lvl, lvl.IntOffset(index, 0,0,-1), Facing.None);
                    break;
                default : break;

            }
        }
        static bool IsBlockLogic(BlockID block)
        {
            switch (block)
            {
                case andBlock:
                case notBlock:
                case orBlock:
                case nandBlock:
                case norBlock:
                case xorBlock:
                case xnorBlock:
                return true;
                default: return false;
            }
        }
        static bool SetContains(BlockID[] set, BlockID block)
        {
            for (int i = 0; i < set.Length; i++)
            {
                if (set[i] == block) { return true; }
            }
            return false;
        }
        //delay blocks get upset if you override there data os we have to make our own addcheck to account for that
        static void AddCheck(Level lvl, int index, bool overRide) {
            if (overRide == true && SetContains(dontOverrideSet,lvl.FastGetBlock(index)))
            {
                ushort x,y,z;
                lvl.IntToPos(index,out x,out y,out z);
                lvl.AddCheck(index,false,lvl.foundInfo(x,y,z));
            }
            lvl.AddCheck(index,overRide);
        }

        static void MsgDebugger(string message) {
            Player debugger = PlayerInfo.FindExact(compLogic.author); if (debugger == null) { return; }
            debugger.Message(message);
        }
        
	}
	
}
