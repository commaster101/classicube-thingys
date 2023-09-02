//
//this is intended to give infected players a diffrent texture pack 
//that is tinted green so that zombies in zs can have green vision
//
//SETUP:
//replace the url with the url of the texture you want zombies to see
//add +infectedtex to the motd in maps you want this effect in

//thanks to Alex_ for the idea

using System;
using System.IO;

using MCGalaxy;
using MCGalaxy.Network;
using MCGalaxy.Events;
using MCGalaxy.Events.EntityEvents;

namespace MCGalaxy
{
    public class webtex : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "infectedtexture"; } }

        public override void Load(bool startup)
        {
            OnTabListEntryAddedEvent.Register(OnGameTeamsUpdate, Priority.Low);
        }

        static void OnGameTeamsUpdate(Entity e, ref string tabName, ref string tabGroup, Player dst)
        {
            Player p = e as Player;
            if (p.level.Config.MOTD.ToLower().Contains("+infectedtex"))
            {
                if (p.infected)
                { 
                    //url for zombies
                    string url = ("https://123dmwm.com/terrain/IsabellaII.png");

                    p.Send(Packet.EnvMapUrl(url, p.hasCP437));
                }
            }
        }

        public override void Unload(bool shutdown)
        {
            OnTabListEntryAddedEvent.Unregister(OnGameTeamsUpdate);
        }
    }
}