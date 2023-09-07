using System;
using System.IO;

using MCGalaxy;
using MCGalaxy.Tasks;

namespace MCGalaxy
{
    public class PLRecord : Plugin
    {
        public override string creator { get { return "LJplayers"; } }
        public override string MCGalaxy_Version { get { return "1.9.2.8"; } }
        public override string name { get { return "PLRecord"; } }

        public override void Load(bool startup)
        {
            Command.Register(new CmdPLR());
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("PLR"));
        }
    }

    public class CmdPLR : Command2
    {
        public override string name { get { return "PLR"; } }
        public override string type { get { return "other"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message)
        {
            string[] args = message.SplitSpaces();

            if (args[0] == "record")
            {
                if (args.Length < 3) { p.Message("%S- type /help PLR for list of cmds"); return; }

                p.Message("%S-started recording");

                p.cmdTimer = true;

                TimerArgs targs = new TimerArgs();
                targs.Message = args[1];
                targs.Repeats = (Int32.Parse(args[2]) * 10);
                targs.Player = p;
                targs.Bot = "null";

                Server.MainScheduler.QueueRepeat(TimerCallback, targs, TimeSpan.FromMilliseconds(100));
            }
            else if (args[0] == "play")
            {
                if (args.Length < 2) { p.Message("%S- type /help PLR for list of cmds"); return; }

                p.cmdTimer = true;

                TimerArgs targs = new TimerArgs();
                targs.Message = args[1];
                targs.Repeats = (-1);
                targs.Player = p;
                targs.Bot = "null";

                Server.MainScheduler.QueueRepeat(TimerCallback, targs, TimeSpan.FromMilliseconds(100));
            }
            else if (args[0] == "playbot")
            {
                if (args.Length < 3) { p.Message("%S- type /help PLR for list of cmds"); return; }

                p.cmdTimer = true;

                TimerArgs targs = new TimerArgs();
                targs.Message = args[1];
                targs.Repeats = (-1);
                targs.Player = p;
                targs.Bot = args[2];

                Server.MainScheduler.QueueRepeat(TimerCallback, targs, TimeSpan.FromMilliseconds(100));
            }
            else if (args[0] == "stop")
            {
                p.cmdTimer = false;
            }
            else if (args[0] == "all")
            {
                DirectoryInfo d = new DirectoryInfo("PLR/");
                FileInfo[] Files = d.GetFiles("*.txt");
                string str = "";

                foreach (FileInfo file in Files)
                {
                    str = str + file.Name + ", ";
                }
                p.Message(str);
            }
            else p.Message("%S- type /help PLR for list of cmds");
        }

        class TimerArgs
        {
            public string Message;
            public int Repeats;
            public Player Player;
            public string Bot;
        }

        static void TimerCallback(SchedulerTask task)
        {
            TimerArgs targs = (TimerArgs)task.State;
            Player p = targs.Player;

            if (targs.Repeats == 0 || p.cmdTimer == false)
            {
                p.Message("Stopped recording/playing path.");
                task.Repeating = false;
            }
            else if (targs.Repeats > 0)
            {
                string path = ("PLR/" + p.name + targs.Message + ".txt");
                string data = (p.Pos.X.ToString() + " " + p.Pos.Y.ToString() + " " + p.Pos.Z.ToString() + " " + p.Rot.HeadX.ToString() + " " + p.Rot.RotY.ToString() + "\n");

                if (!File.Exists(path)) File.WriteAllText(path, targs.Message + "\n");
                File.AppendAllText(Path.Combine(path), data);
            }
            else if (targs.Bot != "null")
            {
                PlayerBot bot = Matcher.FindBots(p, targs.Bot);
                string path = ("PLR/" + targs.Message + ".txt");
                string[] poslist = File.ReadAllLines(path);

                if (Math.Abs(targs.Repeats) >= poslist.Length)
                {
                    p.cmdTimer = false;
                    task.Repeating = false;
                    return;
                }
                string[] newpos = poslist[Math.Abs(targs.Repeats)].SplitSpaces();

                Position pos = new Position(Int32.Parse(newpos[0]), Int32.Parse(newpos[1]), Int32.Parse(newpos[2]));
                bot.Pos = pos; bot.SetYawPitch(Byte.Parse(newpos[4]), Byte.Parse(newpos[3]));
                //p.Level.Message(poslist.Length.ToString());
            }
            else
            {
                string path = ("PLR/" + targs.Message + ".txt");
                string[] poslist = File.ReadAllLines(path);

                if (Math.Abs(targs.Repeats) >= poslist.Length)
                {
                    p.cmdTimer = false;
                    task.Repeating = false;
                    return;
                }
                string[] newpos = poslist[Math.Abs(targs.Repeats)].SplitSpaces();

                Position pos = new Position(Int32.Parse(newpos[0]), Int32.Parse(newpos[1]), Int32.Parse(newpos[2]));
                p.SendPosition(pos, new Orientation(Byte.Parse(newpos[4]), Byte.Parse(newpos[3])));
                //p.Level.Message(poslist.Length.ToString());
            }
            targs.Repeats--;

        }

        public override void Help(Player p)
        {
            p.Message("%S- for recording players path and playing it back");
            p.Message("%A/PLR record [filename] [runtime in secounds]");
            p.Message("%A/PLR play [filename]");
            p.Message("%A/PLR playbot [filename] [botname]");
            p.Message("%A/PLR all %S- shows all recording file names");
            p.Message("%A/PLR stop");
            p.Message("%S- file names always starts with the player who made them");
        }
    }
}