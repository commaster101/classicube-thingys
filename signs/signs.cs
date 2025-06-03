using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using MCGalaxy;
using MCGalaxy.Tasks;
using MCGalaxy.Network;
using MCGalaxy.Commands;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;

using BlockID = System.UInt16;

namespace MCGalaxy
{
    public class signs : Plugin
    {
        public override string creator { get { return "ljplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.8"; } }
        public override string name { get { return "signs"; } }
        public static SchedulerTask task;
        public static Dictionary<char, byte> charToInt = new Dictionary<char, byte>();//character, id
        public static Dictionary<string, SignData[]> signRenderCache = new Dictionary<string, SignData[]>();//level, sign data
        public static string charString = " ☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼ !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~⌂ÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜ¢£¥₧ƒáíóúñÑªº¿⌐¬½¼¡«»░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀αßΓπΣσµτΦΘΩδ∞φε∩≡±≥≤⌠⌡÷≈°∙·√ⁿ²■";
        //every letter in order, used to make dictonary
        private char[] intToChar = charString.ToCharArray();
        //convert to array for faster look up
        public override void Load(bool startup)
        {
            task = Server.MainScheduler.QueueRepeat(SpawnSigns, null, TimeSpan.FromMilliseconds(2500));
            Directory.CreateDirectory("Signs");
            OnPlayerClickEvent.Register(click, Priority.Low);
            OnLevelLoadedEvent.Register(levelLoaded, Priority.Low);
            OnLevelUnloadEvent.Register(levelUnload, Priority.Low);
            OnJoinedLevelEvent.Register(JoinedLevel, Priority.Low);
            Command.Register(new CmdSign());

            for (byte i = 0; i < charString.Length; i++)
            {
                try { charToInt.Add(intToChar[i], i); }
                catch { continue; }
            }
        }
        public override void Unload(bool shutdown)
        {
            Server.MainScheduler.Cancel(task);
            OnPlayerClickEvent.Unregister(click);
            OnLevelLoadedEvent.Unregister(levelLoaded);
            OnLevelUnloadEvent.Unregister(levelUnload);
            OnJoinedLevelEvent.Unregister(JoinedLevel);
            Command.Unregister(Command.Find("Sign"));
        }
        static void JoinedLevel(Player p, Level prevLevel, Level level, ref bool announce)
        {
            if (!level.Config.MOTD.ToLower().Contains("-signs")) defineLetters(p);
        }
        static void levelLoaded(Level lvl)
        {

            string signPath = "Signs/" + lvl.name + ".txt";

            if (!File.Exists(signPath))
            {
             File.WriteAllText(signPath, ""); // Creates empty file
                signRenderCache[lvl.name] = new SignData[0];
                return;
            }
            signRenderCache.Add(lvl.name, parseSignDataArray(File.ReadAllLines(signPath)));

        }
        static void levelUnload(Level lvl, ref bool cancel)
        {
            invalidateSignCache(lvl.name);
        }
        public static void invalidateSignCache(string levelName)
        {
            signRenderCache.Remove(levelName);
        }
        static void click(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            if (face == TargetBlockFace.AwayY || face == TargetBlockFace.TowardsY || face == TargetBlockFace.None || !p.AllowBuild) return;
            if (p.level.Config.MOTD.ToLower().Contains("-signs") || button != MouseButton.Right || action != MouseAction.Pressed) return;

            if (p.Extras.GetBoolean("usingSign") == false) return;
            invalidateSignCache(p.Level.name);
            BlockID block = p.Level.GetBlock(x, y, z);
            BlockDefinition def = p.Level.GetBlockDef(block);
            byte direction = 0;
            short xOffset = 0;
            short zOffset = 0;
            if (def == null)
            {
                switch (face)
                {
                    case TargetBlockFace.TowardsX: direction = 96; xOffset = -1; zOffset = 2; break;
                    case TargetBlockFace.TowardsZ: direction = 16; xOffset = 30; zOffset = -1; break;
                    case TargetBlockFace.AwayX: direction = 64; xOffset = 33; zOffset = 30; break;
                    case TargetBlockFace.AwayZ: direction = 144; xOffset = 2; zOffset = 33; break;
                }
            }
            else
            {
                switch (face)
                {
                    case TargetBlockFace.TowardsX: direction = 96; xOffset = (short)((def.MinX * 2) - 1); zOffset = 2; break;
                    case TargetBlockFace.TowardsZ: direction = 16; xOffset = 30; zOffset = (short)((def.MinY * 2) - 1); break;//block defs flip y and z
                    case TargetBlockFace.AwayX: direction = 64; xOffset = (short)((def.MaxX * 2) + 1); zOffset = 30; break;
                    case TargetBlockFace.AwayZ: direction = 144; xOffset = 2; zOffset = (short)((def.MaxY * 2) + 1); break;//block defs flip y and z
                }
            }

            signs.sendLetters(p, (x * 32) + xOffset, (y * 32) + 20, (z * 32) + zOffset, direction, p.Extras.GetString("signMessage"));//send the sign ASAP, other players can wait
            string newSignText = p.Extras.GetString("signMessage");
            string signPos = (((x * 32) + xOffset).ToString() + "⊕" + ((y * 32) + 20).ToString() + "⊕" + ((z * 32) + zOffset).ToString());
            string signData = (signPos + "⊕" + direction.ToString() + "⊕" + newSignText);
            string signPath = ("Signs/" + p.Level.name + ".txt");

            if (!File.Exists(signPath)) File.Create(signPath);
            string[] signList = File.ReadAllLines(signPath);

            bool replaced = false;
            bool removed = false;
            for (int i = 0; i < signList.Length; i++)
            {
                if (signList[i].StartsWith(signPos))
                {
                    if (newSignText == "")
                    {
                        removed = true;
                        replaced = true;
                        p.Message("sign removed");
                        continue;
                    }
                    signList[i] = signData;
                    replaced = true;
                }
                if (removed == true) signList[(i - 1)] = signList[(i)];
            }
            if (removed == true)
            {
                string[] trimmedLines = new string[signList.Length - 1];
                Array.Copy(signList, trimmedLines, signList.Length - 1);
                File.WriteAllLines(signPath, trimmedLines);
            }
            if (replaced == false) File.AppendAllText(signPath, signData + "\n");
            else if (removed == false) File.WriteAllLines(signPath, signList);
            //p.Message("sign placed at:{0},{1},{2}", x, y, z);
            //p.Message("on face:{0}", face);
            if (p.staticCommands == false) p.Extras["usingSign"] = false;
        }

        static void SpawnSigns(SchedulerTask task)
        {
            Level[] levels = LevelInfo.Loaded.Items;
            foreach (Level lvl in levels)
            {
                if (!signRenderCache.ContainsKey(lvl.name))
                {
                    string signPath = ("Signs/" + lvl.name + ".txt");
                    if (!File.Exists(signPath)) return;
                    signRenderCache.Add(lvl.name, parseSignDataArray(File.ReadAllLines(signPath)));
                }
            }
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
            {
                if (p.level.Config.MOTD.ToLower().Contains("-signs")) return;
                SignData[] signList = signRenderCache[p.Level.name];
                int?[] signDisList = new int?[signList.Length];
                for (int i = 0; i < signList.Length; i++)
                {
                    int signDis =
                    Math.Abs(signList[i].X - p.Pos.X) +
                    Math.Abs(signList[i].Y - p.Pos.Y) +
                    Math.Abs(signList[i].Z - p.Pos.Z);//taxicab distance
                    signDisList[i] = signDis;
                }
                const int maxCharacters = 512;
                const int renderDis = 1024;//view signs up to 32 blocks away
                int usedCharacters = 0;
                while (usedCharacters <= maxCharacters)
                {
                    int? lowestIndex;
                    int? lowestValue;
                    getLowestNumber(signDisList, out lowestValue, out lowestIndex);
                    if (lowestIndex == null) break;
                    if (lowestValue == null) break;
                    if (lowestValue > renderDis) break;
                    SignData Data = signList[(int)lowestIndex];
                    signs.sendLetters(p,Data.X,Data.Y,Data.Z,Data.Direction,Data.Text);
                    usedCharacters += Data.Text.Length;
                    signDisList[(int)lowestIndex] = null;
                }
            }
        }
        static void getLowestNumber(int?[] array, out int? lowestValue, out int? lowestIndex)
        {
            lowestIndex = null;//returns null if the whole array is null
            lowestValue = null;

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] == null) continue;
                if (lowestValue == null || array[i] < lowestValue)
                {
                    lowestValue = array[i];
                    lowestIndex = i;
                }
            }
        }
        static void sendLetters(Player p, int x, int y, int z, byte dir, string text)
        {
            char[] textArray = text.ToCharArray(0, text.Length);
            int xdir = (((dir >> 6) & 3) - 1);//dir is the direction store as 2 pairs in the upper 4 bits of a byte. the pairs store 0-2 when -1 the range is -1 to 1.
            int zdir = (((dir >> 4) & 3) - 1);//eg. 01100000 -> 01 10 -> 1 2 -> 0 1 -> write the letters in the positive z direction.
            for (int i = 0; i < text.Length; i++)
            {
                p.Send(Packet.SpawnEffect(
                charToInt[textArray[i]],
                (x + (i * xdir) % 30) / 32f,
                (y - i / 30) / 32f,
                (z + (i * zdir) % 30) / 32f,
                0f, 0f, 0f
                ));
            }
        }
        static void defineLetters(Player p)
        {
            for (int i = 0; i <= 255; i++)
            {
                byte x = (byte)((i % 16) * 16);
                byte y = (byte)((i / 16) * 16);

                byte effectID = (byte)i;
                byte U1 = x;
                byte V1 = y;
                byte U2 = (byte)(x + 15);
                byte V2 = (byte)(y + 15);
                byte tintRed = (byte)255;
                byte tintGreen = (byte)255;
                byte tintBlue = (byte)255;
                byte frameCount = (byte)1;
                byte particleCount = (byte)1;
                byte size = (byte)1;
                float sizeVariation = 0f;
                float spread = 0;
                float speed = 0f;
                float gravity = 0f;
                float baseLifetime = 5f;
                float lifetimeVariation = 0f;
                bool expireUponTouchingGround = false;
                bool collidesSolid = false;
                bool collidesLiquid = false;
                bool collidesLeaves = false;
                bool fullBright = false;
                p.Send(Packet.DefineEffect(effectID, U1, V1, U2, V2, tintRed, tintGreen, tintBlue, frameCount, particleCount, size, sizeVariation, spread, speed, gravity, baseLifetime, lifetimeVariation, expireUponTouchingGround, collidesSolid, collidesLiquid, collidesLeaves, fullBright));
            }
        }
        static SignData[] parseSignDataArray(string[] lines)
        {
            int count = lines.Length;
            SignData[] result = new SignData[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = SignData.FromString(lines[i]);
            }
            return result;
        }
        public sealed class CmdSign : Command2
        {
            public override string name { get { return "Sign"; } }
            public override string type { get { return CommandTypes.Building; } }
            public override bool museumUsable { get { return false; } }
            public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
            public override bool SuperUseable { get { return false; } }

            public override void Use(Player p, string message, CommandData data)
            {
                p.Extras["usingSign"] = true;
                p.Extras["signMessage"] = message;
                if (message == "") p.Extras["signMessage"] = null;
            }

            public override void Help(Player p)
            {
                p.Message("%T/Sign <some text>%H- Makes text in the world. if no text is given the sign will be removed.");
            }
        }

        public override void Help(Player p)
        {
            p.Message("%fplugin for making signs out of particals");
        }
    }
    public class SignData {
    public int X, Y, Z;
    public byte Direction;
    public string Text;

    public SignData(int x, int y, int z, byte dir, string text) {
        X = x; Y = y; Z = z;
        Direction = dir;
        Text = text;
    }

    public override string ToString() {
        // Convert back to the file format used before
        return (X.ToString()+"⊕"+Y.ToString()+"⊕"+Z.ToString()+"⊕"+Direction.ToString()+"⊕"+Text);
    }

    public static SignData FromString(string line) {
        string[] parts = line.Split('⊕');
        return new SignData(
            int.Parse(parts[0]),
            int.Parse(parts[1]),
            int.Parse(parts[2]),
            byte.Parse(parts[3]),
            parts[4]
        );
    }
}
}