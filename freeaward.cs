using System;
using System.Threading;

using MCGalaxy;
using MCGalaxy.Eco;


namespace MCGalaxy.Modules.Awards
{
    public class freeaward : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "freeaward"; } }

        public override void Load(bool startup)
        {
            Command.Register(new CmdFreeAward());
        }

        public sealed class CmdFreeAward : Command2
        {
            public override string name { get { return "FreeAward"; } }
            public override string type { get { return CommandTypes.Economy; } }
            public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

            public override void Use(Player p, string message, CommandData data)
            {
                string[] args = message.SplitSpaces(2);
                if (args[0] == "secwetcode")
                {
                    string plName = p.name;
                    string award = Matcher.FindAwards(p, args[1]);
                    string displayName = p.FormatNick(plName);

                    p.Message(award);

                    PlayerAwards.Give(plName, award);
                    
                    Chat.MessageGlobal("{0} &Swas awarded: &b{1}", displayName, award);
                    PlayerAwards.Save();
                }
            }

            public override void Help(Player p)
            {
                p.Message("&TFreeAward");
            }
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("FreeAward"));
        }
    }
}