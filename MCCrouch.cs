//reference System.Core.dll

/*
    IMPORTANT:
    - This plugin closely replicates the crouching system from Minecraft 1.14 you can bridge but you can't glide on the very edge of blocks
    - also see my swimming/crawling plugin https://github.com/LJplayswasused/classicube-thingys/blob/main/swimming.cs
    - this plugin is based off of venk's crouching plugin and I used the cmdfly.cs file as refernce
    
    Required setup:
    1./gb copy 0
    2./gb edit [new block id] collide 2
    3./gb edit [new block id] min 0 0 8
    4./gb edit [new block id] max 16 16 8
    5.copy this new block and make one with fliped values (0 0 8 - 8 16 16)
    6.goto line 142 - 144 and change the ids to your block ids

    Recommended setup (optional, but better if you do):
    1. add CustomModels plugin: https://github.com/NotAwesome2/MCGalaxy-CustomModels/releases/tag/v1.4.2
    2. /cm upload crouch https://www.dropbox.com/s/doq1g2q0fjmep4v/crouch.bbmodel
    3. /cm config crouch eyeY 24
    optionally so you can go under top slabs 
    /cm config crouch collisionbounds 8.6 22 8.6
    
    Press RSHIFT to toggle between crouching/not crouching
*/

using System;
using System.Linq;
using System.Collections.Generic;

using MCGalaxy.Commands;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;
using MCGalaxy.Blocks;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;

using BlockID = System.UInt16;

namespace MCGalaxy
{
    public class Crouching : Plugin
    {
        public override string name { get { return "MCCrouch"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.0"; } }
        public override string creator { get { return "LJplays"; } }

        public override void Load(bool startup)
        {
            OnGettingMotdEvent.Register(HandleGettingMOTD, Priority.Low);

            Command.Register(new CmdCrouch());
        }


        public override void Unload(bool shutdown)
        {
            OnGettingMotdEvent.Unregister(HandleGettingMOTD);

            Command.Unregister(Command.Find("Crouch"));
        }

        static void HandleGettingMOTD(Player p, ref string motd)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players)
            {
                if (!p.Supports(CpeExt.TextHotkey)) continue;
                pl.Send(Packet.TextHotKey("Crouch", "/Crouch◙", 54, 0, true));
            }

            // Check if player has actually toggled crouch, since defaults to false
            if (!p.Extras.GetBoolean("IS_CROUCHING")) return;

            // Remove current horspeed rule because client does MOTD checking lamely
            motd = motd
                   .SplitSpaces()
                   .Where(word => !word.CaselessStarts("horspeed="))
                   .Join(" ");

            motd += " horspeed=0.52";
        }

        public override void Help(Player p)
        {
        }
    }

    public sealed class CmdCrouch : Command2
    {
        public override string name { get { return "Crouch"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data)
        {
            if (p.Extras.GetBoolean("IS_CROUCHING"))
            {
                p.Extras["IS_CROUCHING"] = false;
                p.Extras["HAS_CROUCHED"] = true;
                p.SendMapMotd();
                p.UpdateModel("humanoid|1");
            }
            else
            {
                p.Extras["IS_CROUCHING"] = true;
                p.Extras["HAS_CROUCHED"] = true;
                p.SendMapMotd();
                p.UpdateModel("crouch");
            }

            CrouchState state = new CrouchState();
            state.player = p;
            SchedulerTask task = new SchedulerTask(CrouchCallback, state, TimeSpan.Zero, true);
            p.CriticalTasks.Add(task);
        }

        class CrouchState
        {
            public Player player;
            public Position oldPos = default(Position);
            public List<Vec3U16> lastBlock = new List<Vec3U16>();
            public List<Vec3U16> BlockCoords = new List<Vec3U16>();
        }

        static void CrouchCallback(SchedulerTask task)
        {
            CrouchState state = (CrouchState)task.State;
            Player p = state.player;
            if (p.Extras.GetBoolean("IS_CROUCHING") == true) { DoCrouch(state); return; }

            foreach (Vec3U16 pos in state.lastBlock)
            {
                p.RevertBlock(pos.X, pos.Y, pos.Z);
            }
            task.Repeating = false;
        }

        static void DoCrouch(CrouchState state)
        {

            //put the ids of the blocks that you setup
            int AltBlockID = 66;
            int NSBlockID = 67;
            int EWBlockID = 68;
            if (AltBlockID >= 66) AltBlockID = AltBlockID + 256;
            if (NSBlockID >= 66) NSBlockID = NSBlockID + 256;
            if (EWBlockID >= 66) EWBlockID = EWBlockID + 256;

            Player p = state.player;
            if (p.Pos == state.oldPos) return;
            
            int x = p.Pos.BlockX, y = p.Pos.BlockY, z = p.Pos.BlockZ;

            if (!CollideType.IsSolid(p.Level.CollideType(p.Level.GetBlock((ushort)x, (ushort)(y - 2), (ushort)z)))) return;  

            for (int zz = z - 1; zz <= z + 1; zz++)
                for (int xx = x - 1; xx <= x + 1; xx++)
                { 
                     BlockID block = p.Level.GetBlock((ushort)xx, (ushort)(y - 2), (ushort)zz);
                     BlockID block1 = p.Level.GetBlock((ushort)xx, (ushort)(y - 1), (ushort)zz);
                     BlockID block2 = p.Level.GetBlock((ushort)xx, (ushort)y, (ushort)zz);

                    Vec3U16 pos;
                    pos.X = (ushort)xx; pos.Y = (ushort)y; pos.Z = (ushort)zz;

                    if (!CollideType.IsSolid(p.Level.CollideType(block)) && !CollideType.IsSolid(p.Level.CollideType(block1)) && p.level.IsAirAt(pos.X, pos.Y, pos.Z) && (xx == x ^ zz == z)) state.BlockCoords.Add(pos);
                }
            
            foreach (Vec3U16 P in state.BlockCoords)
            {
                if (state.lastBlock.Contains(P)) continue;

                //TODO:add blocks/checks for directoin to allow brigding
                if (P.X == (x - 1) && P.Z == z) p.SendBlockchange(P.X, P.Y, P.Z, (ushort)EWBlockID);

                else if (P.X == x && P.Z == (z - 1)) p.SendBlockchange(P.X, P.Y, P.Z, (ushort)NSBlockID);

                else if (P.X == (x + 1) && P.Z == z) p.SendBlockchange(P.X, P.Y, P.Z, (ushort)EWBlockID);

                else if (P.X == x && P.Z == (z + 1)) p.SendBlockchange(P.X, P.Y, P.Z, (ushort)NSBlockID);

                else continue;
                
                state.lastBlock.Add(P);
            }

            for (int i = 0; i < state.lastBlock.Count; i++)
            {
                Vec3U16 P = state.lastBlock[i];
                if (state.BlockCoords.Contains(P)) continue;

                p.RevertBlock(P.X, P.Y, P.Z);
                state.lastBlock.RemoveAt(i); i--;
            }

            state.BlockCoords.Clear();
            state.oldPos = p.Pos;
        }

        public override void Help(Player p)
        {
            p.Message("%T/Crouch %H- Toggles crouching.");
        }
    }
}
