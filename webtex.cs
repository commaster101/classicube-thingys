//goto line 34 and replace the url to the web only url
//add +webtex to any level you want to use the alt webclient texture

using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Network;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy
{
    public class webtex : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "webtex"; } }

        public override void Load(bool startup)
        {
            OnJoinedLevelEvent.Register(HandleTexture, Priority.Low);
        }

        static void HandleTexture(Player p, Level prevLevel, Level level, ref bool announce)
        {
            if(p.level.Config.MOTD.ToLower().Contains("+webtex"))
            {
                string appName = p.Session.ClientName();
                
                if(appName.Contains("web"))
                {
                    //url for webclients
                    string weburl = ("https://123dmwm.com/terrain/64xDefault.png");

                    //url for webclients
                    string url = (p.level.Config.TexturePack);

                    if (weburl.CaselessEnds(".png"))
                    {
                        p.level.Config.Terrain = weburl;
                    }
                    else if (weburl.CaselessEnds(".zip"))
                    {
                        p.level.Config.TexturePack = weburl;
                    }
                    p.SendCurrentTextures();

                    p.level.Config.Terrain = url;
                    p.level.Config.TexturePack = url;
                }
            }
        }

        public override void Unload(bool shutdown)
        {
            OnJoinedLevelEvent.Unregister(HandleTexture);
        }
    }
}
