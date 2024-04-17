using System;
using System.IO;

using MCGalaxy;
using MCGalaxy.Bots;
using MCGalaxy.Tasks;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.Commands;
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy
{
	public class gunsv2 : Plugin
	{

		public override string name { get { return "gunsv2"; } }
		public override string MCGalaxy_Version { get { return "1.9.4.9"; } }
		public override string welcome { get { return "Loaded gunsv2"; } }
		public override string creator { get { return "LJplays+"; } }
		public static SchedulerTask task;
		BotInstruction gunhit;
		// Called when this plugin is being loaded (e.g. on server startup)
		public override void Load(bool startup)
		{
			Command.Register(new CmdWeapon());
			OnPlayerClickEvent.Register(click, Priority.Low);
			OnJoinedLevelEvent.Register(joinedLevel, Priority.Low);
			gunhit = new gunhitInstruction();
			task = Server.MainScheduler.QueueRepeat(Spawn, null, TimeSpan.FromMilliseconds(15000));

			BotInstruction.Instructions.Add(gunhit);

			if (!Directory.Exists("gunsv2")) Directory.CreateDirectory("gunsv2");			
		}

		// Called when this plugin is being unloaded (e.g. on server shutdown)
		public override void Unload(bool shutdown)
		{
			Command.Unregister(Command.Find("weapon"));
			OnPlayerClickEvent.Unregister(click);
			OnJoinedLevelEvent.Unregister(joinedLevel);
			Server.MainScheduler.Cancel(task);

			BotInstruction.Instructions.Remove(gunhit);
		}

		static void Spawn(SchedulerTask task)
		{
			Player[] players = PlayerInfo.Online.Items;
			foreach (Player p in players)
			{
				if (!p.level.Config.MOTD.ToLower().Contains("+gunsv2")) return;
				if (!File.Exists("gunsv2/"+p.level.name+".txt")) File.AppendAllText("gunsv2/" + p.level.name + ".txt", "x y z yaw pitch\n");
				string[] poslist = File.ReadAllLines("gunsv2/" + p.level.name + ".txt");
				if (poslist.Length == 1) return;
				Random rand = new Random();
				string[] spawnpos = (poslist[rand.Next(1,poslist.Length)]).SplitSpaces();
				if (NextFreeId(p) == 0) return;
				PlayerBot bot = new PlayerBot(NextFreeId(p).ToString(), p.level);
				bot.Owner = "LJplays";
				
				if (BotExists(p.level, bot.name, null)) return;
				if (p.level.Bots.Count >= Server.Config.MaxBotsPerLevel) return;

				Position pos = new Position(Int32.Parse(spawnpos[0]), Int32.Parse(spawnpos[1]), Int32.Parse(spawnpos[2]));
				bot.SetInitialPos(pos);
				bot.SetYawPitch(byte.Parse(spawnpos[3]), byte.Parse(spawnpos[4]));
				PlayerBot.Add(bot);
				bot.AIName = "arena";
				bot.DeathMessage = "50";
				bot.UpdateModel("zombie");
				bot.movementSpeed = 1;
				if (!ScriptFile.Parse(p, bot, "arena")) return;
			}
		}
		unsafe static byte NextFreeId(Player p)
		{
			byte* used = stackalloc byte[256];
			for (int i = 0; i < 256; i++) used[i] = 0;

			PlayerBot[] bots = p.level.Bots.Items;
			for (int i = 0; i < bots.Length; i++)
			{
				byte id = bots[i].id;
				used[id] = 1;
			}

			for (byte i = 127; i >= 64; i--)
			{
				if (used[i] == 0) return i;
			}
			// NOTE: For some clients these IDs mean self ID
			for (byte i = 254; i > 127; i--)
			{
				if (used[i] == 0) return i;
			}
			// NOTE: These IDs may conflict with player IDs, so use as a last resort
			for (byte i = 63; i > 0; i--)
			{
				if (used[i] == 0) return i;
			}
			return 0;
		}

		static bool BotExists(Level lvl, string name, PlayerBot skip)
		{
			PlayerBot[] bots = lvl.Bots.Items;
			foreach (PlayerBot bot in bots)
			{
				if (bot == skip) continue;
				if (bot.name.CaselessEq(name)) return true;
			}
			return false;
		}

		static void click(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
		{
			if (!p.level.Config.MOTD.ToLower().Contains("+gunsv2")) return;
			Player[] players = PlayerInfo.Online.Items;
			if (button == MouseButton.Left)
			{
				if (action == MouseAction.Pressed)
				{
					if ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - long.Parse(p.Extras.GetString("lastfire"))) <= p.Extras.GetInt("firerate")*100) return;
					p.Extras["lastfire"] = (DateTimeOffset.Now.ToUnixTimeMilliseconds());
						raycast(p, yaw, pitch, p.Extras.GetInt("range"));
					if(p.Extras.GetString("weapon") == "shotgun")
                    {
						raycast(p, (ushort)(yaw + 1024), pitch, p.Extras.GetInt("range"));
						raycast(p, (ushort)(yaw - 1024), pitch, p.Extras.GetInt("range"));
						raycast(p, (ushort)(yaw + 512), pitch, p.Extras.GetInt("range"));
						raycast(p, (ushort)(yaw - 512), pitch, p.Extras.GetInt("range"));
					}
				}
			}
		}

		static void joinedLevel(Player p, Level prevLevel, Level level, ref bool announce)
		{
			p.SendCpeMessage(CpeMessageType.BottomRight2, "");
			if (prevLevel != null)
            {
				if (!level.Config.MOTD.ToLower().Contains("+gunsv2")&& prevLevel.Config.MOTD.ToLower().Contains("+gunsv2"))
				{
					Command.Find("SilentModel").Use(p, "humanoid|1");
				}
			}

			if (!p.level.Config.MOTD.ToLower().Contains("+gunsv2")) return;
			p.Extras["hp"] = 100;
			p.Extras["weapon"] = "fist";
			p.Extras["damage"] = 16;
			p.Extras["range"] = 196;
			p.Extras["firerate"] = 5;
			p.Extras["lastfire"] = (DateTimeOffset.Now.ToUnixTimeMilliseconds());
			sendhud(p);
			byte effectID = (byte)2;
			byte U1 = (byte)0;
			byte V1 = (byte)0;
			byte U2 = (byte)7;
			byte V2 = (byte)7;
			byte tintRed = (byte)255;
			byte tintGreen = (byte)255;
			byte tintBlue = (byte)255;
			byte frameCount = (byte)4;
			byte particleCount = (byte)1;
			byte size = (byte)2;
			float sizeVariation = 0f;
			float spread = 0;
			float speed = 0f;
			float gravity = 0f;
			float baseLifetime = 0.2f;
			float lifetimeVariation = 0.1f;
			bool expireUponTouchingGround = false;
			bool collidesSolid = false;
			bool collidesLiquid = false;
			bool collidesLeaves = false;
			bool fullBright = true;
			p.Send(Packet.DefineEffect(effectID, U1, V1, U2, V2, tintRed, tintGreen, tintBlue, frameCount, particleCount, size, sizeVariation, spread, speed, gravity, baseLifetime, lifetimeVariation, expireUponTouchingGround, collidesSolid, collidesLiquid, collidesLeaves, fullBright));
		}

		static void raycast(Player p, ushort yaw, ushort pitch, int dis)
		{
			Vec3F32 dir = DirUtils.GetDirVectorExt(yaw, pitch);
			Vec3F32 bullet;
			bullet.X = p.Pos.X;
			bullet.Y = p.Pos.Y;
			bullet.Z = p.Pos.Z;
			PlayerBot[] bots = p.level.Bots.Items;
			bool hit = false;
			int check = 0;
			for (int i = 0; i < dis; i++)
			{
				if (hit == true) break;
				bullet += (dir);
				if (check != 8) { check++; continue; }
				if (!p.level.IsAirAt((ushort)(bullet.X / 32), (ushort)(bullet.Y / 32), (ushort)(bullet.Z / 32))) break;
				//p.Message(bullet.ToString());
				foreach (PlayerBot bot in bots)
				{
					AABB bb = ModelInfo.CalcAABB(bot).OffsetPosition(bot.Pos);
					if (bullet.X > bb.Min.X && bullet.Y > bb.Min.Y && bullet.Z > bb.Min.Z &&
						bullet.X < bb.Max.X && bullet.Y < bb.Max.Y && bullet.Z < bb.Max.Z)
					{
						if (Int32.Parse(bot.DeathMessage) == null) return;
						int bothp = -1;
						bothp = Int32.Parse(bot.DeathMessage);
						if (bothp == -1) { hit = true; break; }
						bothp -= p.Extras.GetInt("damage");
						if(bothp <= 0) { PlayerBot.Remove(bot); }
						bot.DeathMessage = bothp.ToString();
						BotsFile.Save(p.level);
						hit = true; break;
					}
				}
				Player[] players = PlayerInfo.Online.Items;
				foreach (Player pl in players)
				{
					pl.Send(Packet.SpawnEffect((byte)2, bullet.X / 32f, bullet.Y / 32f, bullet.Z / 32f, bullet.X / 32f, bullet.Y / 32f, bullet.Z / 32f));
					if (p == pl) continue;
					AABB bb = ModelInfo.CalcAABB(pl).OffsetPosition(pl.Pos);
					if (bullet.X > bb.Min.X && bullet.Y > bb.Min.Y && bullet.Z > bb.Min.Z &&
						bullet.X < bb.Max.X && bullet.Y < bb.Max.Y && bullet.Z < bb.Max.Z)
					{
						pl.Extras["hp"] = pl.Extras.GetInt("hp") - p.Extras.GetInt("damage");
						sendhud(pl);
						if (pl.Extras.GetInt("hp") <= 0) die(pl, pl.color + pl.DisplayName + " %ewas shot by " + p.color + p.DisplayName);
						hit = true; break;
					}
				}
				check = 0;
			}
		}
		static void sendhud(Player p)
		{
			p.SendCpeMessage(CpeMessageType.BottomRight2, "%f-{%c" + p.Extras.GetString("hp") + "%F}-");
		}
		static void die(Player p, string cause)
		{
			p.Extras["hp"] = 100;
			p.HandleDeath(Block.Stone, cause);
			sendhud(p);
		}
		public override void Help(Player p)
		{
			p.Message("No help is available for this plugin.");
		}
	}
	public sealed class CmdWeapon : Command2
	{
		public override string name { get { return "Weapon"; } }
		public override string type { get { return CommandTypes.Games; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
		public override CommandPerm[] ExtraPerms{ get { return new[] { new CommandPerm(LevelPermission.Admin, "can add spawnpoints") }; }}
		public override bool SuperUseable { get { return true; } }

		public override void Use(Player p, string message, CommandData data)
		{
			if (message == "pistol")
			{
				p.Extras["weapon"] = "pistol";
				p.Extras["damage"] = 8;
				p.Extras["range"] = 768;
				p.Extras["firerate"] = 5;
				Command.Find("SilentModel").Use(p, "pistolmodel|1");
			}
			else if (message == "smg")
			{
				p.Extras["weapon"] = "smg";
				p.Extras["damage"] = 4;
				p.Extras["range"] = 512;
				p.Extras["firerate"] = 2;
				Command.Find("SilentModel").Use(p, "smgmodel|1");
			}
			else if (message == "sniper")
			{
				p.Extras["weapon"] = "sniper";
				p.Extras["damage"] = 60;
				p.Extras["range"] = 1536;
				p.Extras["firerate"] = 25;
				Command.Find("SilentModel").Use(p, "snipermodel|1");
			}
			else if (message == "shotgun")
			{
				p.Extras["weapon"] = "shotgun";
				p.Extras["damage"] = 15;
				p.Extras["range"] = 480;
				p.Extras["firerate"] = 15;
				Command.Find("SilentModel").Use(p, "shotgunmodel|1");
			}
			else if (message == "fist")
			{
				p.Extras["weapon"] = "fist";
				p.Extras["damage"] = 16;
				p.Extras["range"] = 196;
				p.Extras["firerate"] = 5;
				Command.Find("SilentModel").Use(p, "humanoid|1");
			}
			else if (HasExtraPerm(p, p.Rank, 1) == true && message == "add")
			{
				File.AppendAllText("gunsv2/" + p.level.name + ".txt", p.Pos.X.ToString() +" "+ p.Pos.Y.ToString() + " " + p.Pos.Z.ToString() + " " + p.Rot.RotY.ToString() + " " + p.Rot.HeadX.ToString() + "\n");
			}
			else
			{
				p.Message("/weapon [weapon]");
				p.Message("sets your weapon for gunsv2");
				p.Message("weapons: pistol, smg, sniper, shotgun");
				return;
			}
			p.Extras["lastfire"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			p.Message("Changed your weapon to " + p.Extras.GetString("weapon"));
		}
		public override void Help(Player p)
		{
			p.Message("sets your weapon for gunsv2");
			p.Message("weapons:pistol,smg,sniper,shotgun");
		}
	}
	public sealed class gunhitInstruction : BotInstruction
	{
		public gunhitInstruction() { Name = "gunhit"; }

		public override bool Execute(PlayerBot bot, InstructionData data)
		{
			int search = 128;
			if (data.Metadata != null) search = (ushort)data.Metadata;
			Player closest = ClosestPlayer(bot, search);
			if (closest == null) { bot.NextInstruction(); return false; }

			bot.TargetPos = closest.Pos;
			Random rand = new Random();
			PlayerBot[] bots = bot.level.Bots.Items;
			foreach (PlayerBot Cbot in bots)
			{
				if (Cbot.TargetPos == bot.TargetPos)
				{
					bot.TargetPos.X += (int)(Math.Sin(bot.id) * 64);
					bot.TargetPos.Z += (int)(Math.Cos(bot.id) * 64);
				}
			}
			bot.movement = true;
			FaceTowards(bot, closest);

			search = 3;
			if (data.Metadata != null) search = (ushort)data.Metadata;
			closest = ClosestPlayer(bot, search);

			if (closest == null) { bot.NextInstruction(); return false; }
			HitPlayer(bot, closest, bot.Rot);
			{ bot.NextInstruction(); return false;}
		}

		public override InstructionData Parse(string[] args)
		{
			InstructionData data = default(InstructionData);
			if (args.Length > 1)
				data.Metadata = ushort.Parse(args[1]);
			return data;
		}

		public override void Output(Player p, string[] args, TextWriter w)
		{
			if (args.Length > 3)
			{
				w.WriteLine(Name + " " + ushort.Parse(args[3]));
			}
			else
			{
				w.WriteLine(Name);
			}
		}

		static void FaceTowards(PlayerBot bot, Player p)
		{
			int srcHeight = ModelInfo.CalcEyeHeight(p);
			int dstHeight = ModelInfo.CalcEyeHeight(bot);

			int dx = p.Pos.X - bot.Pos.X, dy = (p.Pos.Y + srcHeight) - (bot.Pos.Y + dstHeight), dz = p.Pos.Z - bot.Pos.Z;
			Vec3F32 dir = new Vec3F32(dx, dy, dz);
			dir = Vec3F32.Normalise(dir);

			Orientation rot = bot.Rot;
			DirUtils.GetYawPitch(dir, out rot.RotY, out rot.HeadX);
			bot.Rot = rot;
		}

		public static void HitPlayer(PlayerBot bot, Player p, Orientation rot)
		{
			// Send player backwards if hit
			// Code "borrowed" from PvP plugin

			// If we are very close to a player, switch from trying to look
			// at them to just facing the opposite direction to them

			rot.RotY = (byte)(p.Rot.RotY + 128);
			bot.Rot = rot;

			int srcHeight = ModelInfo.CalcEyeHeight(bot);
			int dstHeight = ModelInfo.CalcEyeHeight(p);
			int dx2 = bot.Pos.X - p.Pos.X, dy2 = (bot.Pos.Y + srcHeight) - (p.Pos.Y + dstHeight), dz2 = bot.Pos.Z - p.Pos.Z;

			Vec3F32 dir2 = new Vec3F32(dx2, dy2, dz2);

			if (dir2.Length > 0) dir2 = Vec3F32.Normalise(dir2);

			float mult = 1 / ModelInfo.GetRawScale(p.Model);
			float plScale = ModelInfo.GetRawScale(p.Model);

			float VelocityY = 1.0117f * mult;

			if (dir2.Length <= 0) VelocityY = 0;

			if (p.Supports(CpeExt.VelocityControl))
			{
				// Intensity of force is in part determined by model scale
				p.Send(Packet.VelocityControl((-dir2.X * mult) * 0.57f, VelocityY, (-dir2.Z * mult) * 0.57f, 0, 1, 0));
			}

			int damage = 4;

			if (bot.Model == "bee") damage = 2;
			if (bot.Model == "blaze") damage = 4;
			if (bot.Model == "creeper") damage = 23; // 22.5 but either way, still kills the player
			if (bot.Model == "enderman") damage = 5; // 4.5
			if (bot.Model == "panda") damage = 4;
			if (bot.Model == "skeleton") damage = 2; // 3-5 damage when shot with bow
			if (bot.Model == "spider") damage = 2;
			if (bot.Model == "witherskeleton") damage = 5;
			if (bot.Model == "wither") damage = 15; // 34 with explosion
			if (bot.Model == "zombie") damage = 3; // 2.5

			// Update player's health
			p.Extras["hp"] = p.Extras.GetInt("hp") - damage;
			p.SendCpeMessage(CpeMessageType.BottomRight2, "%f-{%c" + p.Extras.GetString("hp") + "%F}-");
			if (p.Extras.GetInt("hp") <= 0) die(p, p.color + p.DisplayName + " %ewas killed by " + bot.color + bot.Model);


		}
		static void die(Player p, string cause)
		{
			p.Extras["hp"] = 100;
			p.HandleDeath(Block.Stone, cause);
			p.SendCpeMessage(CpeMessageType.BottomRight2, "%f-{%c" + p.Extras.GetString("hp") + "%F}-");
		}
		internal static Player ClosestPlayer(PlayerBot bot, int search)
		{
			int maxDist = search * 32;
			Player[] players = PlayerInfo.Online.Items;
			Player closest = null;

			foreach (Player p in players)
			{
				if (p.level != bot.level || p.invincible || p.hidden) continue;

				int dx = p.Pos.X - bot.Pos.X, dy = p.Pos.Y - bot.Pos.Y, dz = p.Pos.Z - bot.Pos.Z;
				int playerDist = Math.Abs(dx) + Math.Abs(dy) + Math.Abs(dz);
				if (playerDist >= maxDist) continue;

				closest = p;
				maxDist = playerDist;
			}
			return closest;
		}
		public override string[] Help { get { return help; } }
		static string[] help = new string[] {
			"&T/BotAI add [name] stare <radius>",
			"&HCauses the bot to stare at the closest player in the search radius.",
			"&H  <radius> defaults to 20000 blocks.",
		};
	}
}