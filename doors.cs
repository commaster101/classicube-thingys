using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;

using BlockID = System.UInt16;

namespace MCGalaxy
{
    public class doors : Plugin
    {
        public override string creator { get { return "ljplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "doors"; } }

        public override void Load(bool startup)
        {
            OnPlayerClickEvent.Register(door, Priority.Low);
        }

        void door(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            if (p.level.Config.MOTD.ToLower().Contains("-doors")) return;

            if (button == MouseButton.Right)
            {
                if (action != MouseAction.Pressed)
                {
                    BlockID block = p.level.GetBlock(x, y, z);
                    int fixblock = block;
                    if (fixblock >= 66) fixblock = block - 256;

                    //dark doors
                    if (fixblock == 99) p.level.UpdateBlock(p, x, y, z, 101);
                    if (fixblock == 101) p.level.UpdateBlock(p, x, y, z, 99);
                    if (fixblock == 100) p.level.UpdateBlock(p, x, y, z, 102);
                    if (fixblock == 102) p.level.UpdateBlock(p, x, y, z, 100);

                    //light doors
                    if (fixblock == 94) p.level.UpdateBlock(p, x, y, z, 96);
                    if (fixblock == 96) p.level.UpdateBlock(p, x, y, z, 94);
                    if (fixblock == 95) p.level.UpdateBlock(p, x, y, z, 97);
                    if (fixblock == 97) p.level.UpdateBlock(p, x, y, z, 95);
                }
            }
        }

        public override void Unload(bool shutdown)
        {
            OnPlayerClickEvent.Unregister(door);
        }
    }
}