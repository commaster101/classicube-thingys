//reference System.Core.dll

//how to use
// add a bot with /bothp spawn (/help bothp for more info)
// goto line 60 to add more weapons and set the weapon ids
// type "/motd +guns" for the plugin to work on that map 

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Network;
using MCGalaxy.Tasks;
using MCGalaxy.Bots;
using MCGalaxy;

namespace MCGalaxy
{
	public class guns : Plugin
	{
		public override string name { get { return "guns"; } }

		public override string MCGalaxy_Version { get { return "1.9.4.5"; } }

		public override string welcome { get { return "Loaded Message!"; } }

		public override string creator { get { return "ljplays"; } }

		public static SchedulerTask task;

		public override void Load(bool startup)
		{
			task = Server.MainScheduler.QueueRepeat(Task, null, TimeSpan.FromMilliseconds(200));
			
			OnPlayerClickEvent.Register(ammo, Priority.Low);

			OnGettingMotdEvent.Register(onjoin, Priority.Low);

			Command.Register(new Cmdbothp());
		}

		static void Task(SchedulerTask task)
		{
			Player[] players = PlayerInfo.Online.Items;
			foreach (Player p in players)
			{
				if (!p.level.Config.MOTD.ToLower().Contains("+guns")) return;
				int held = p.GetHeldBlock();
				if (held >= 66) held = held - 256;

				int delay = 100;
				int maxammo = 1;
				float dist = 10;
				int packedDist = (int)(dist * 32);
				if (p.Extras.GetInt("lastheld") != held)
				{
					           //v set pistol weapon block id
					if (held == 70)
					{
						dist = 30;
						maxammo = 20;
						delay = 100;
						p.Extras["wdamage"] = 25;
						p.Extras["sleep"] = 750;

						p.SendCpeMessage(CpeMessageType.Status2, "%f[%b" + p.Extras.GetInt("ammo" + held.ToString()) + "%f/%b" + maxammo + "%f]");

						packedDist = (int)(dist * 32);
						p.ReachDistance = dist;
						Server.reach.Update(p.name, packedDist.ToString());
						Server.reach.Save();
						//uncomment if you have custommodels
						p.UpdateModel("pistolmodel");
						Server.models.Update(p.name, "pistolmodel");
						Server.models.Save();
					}
									//v set sniper weapon block id
					else if (held == 71)
					{
						dist = 75;
						maxammo = 3;
						delay = 2000;
						p.Extras["wdamage"] = 50;
						p.Extras["sleep"] = 3000;

						p.SendCpeMessage(CpeMessageType.Status2, "%f[%b" + p.Extras.GetInt("ammo" + held.ToString()) + "%f/%b" + maxammo + "%f]");

						packedDist = (int)(dist * 32);
						p.ReachDistance = dist;
						Server.reach.Update(p.name, packedDist.ToString());
						Server.reach.Save();
						//uncomment if you have custommodels
						p.UpdateModel("snipermodel");
						Server.models.Update(p.name, "snipermodel");
						Server.models.Save();
					}
									//v set smg weapon block id
					else if (held == 72)
					{
						dist = 30;
						maxammo = 30;
						delay = 1000;
						p.Extras["wdamage"] = 15;
						p.Extras["sleep"] = 1000;

						p.SendCpeMessage(CpeMessageType.Status2, "%f[%b" + p.Extras.GetInt("ammo" + held.ToString()) + "%f/%b" + maxammo + "%f]");

						packedDist = (int)(dist * 32);
						p.ReachDistance = dist;
						Server.reach.Update(p.name, packedDist.ToString());
						Server.reach.Save();
						//uncomment if you have custommodels
						p.UpdateModel("smgmodel");
						Server.models.Update(p.name, "smgmodel");
						Server.models.Save();
					}
									//v set shotgun weapon block id
					else if (held == 73)
					{
						dist = 7;
						maxammo = 6;
						delay = 1500;
						p.Extras["wdamage"] = 35;
						p.Extras["sleep"] = 2000;


						p.SendCpeMessage(CpeMessageType.Status2, "%f[%b" + p.Extras.GetInt("ammo" + held.ToString()) + "%f/%b" + maxammo + "%f]");

						packedDist = (int)(dist * 32);
						p.ReachDistance = dist;
						Server.reach.Update(p.name, packedDist.ToString());
						Server.reach.Save();
						//uncomment if you have custommodels
						p.UpdateModel("shotgunmodel");
						Server.models.Update(p.name, "shotgunmodel");
						Server.models.Save();
					}
					else
					{
						dist = 10;
						p.Extras["wdamage"] = 1;
						p.Extras["sleep"] = 50;

						p.SendCpeMessage(CpeMessageType.Status2, "%f[%b-%f/%b-%f]");

						packedDist = (int)(dist * 32);
						p.ReachDistance = dist;
						Server.reach.Update(p.name, packedDist.ToString());
						Server.reach.Save();

						p.UpdateModel("humanoid");

						Server.models.Update(p.name, "humanoid");
						Server.models.Save();
					}

					p.Extras["pdist"] = packedDist;
					p.Extras["maxammo"] = maxammo;
					p.Extras["delay"] = delay;

					p.Send(Packet.SetHotbar(70, 0, false));
					p.Send(Packet.SetHotbar(71, 1, false));
					p.Send(Packet.SetHotbar(72, 2, false));
					p.Send(Packet.SetHotbar(73, 3, false));
				}
				p.Extras["lastheld"] = held;

				Level lvl = p.level;
				PlayerBot[] bots = lvl.Bots.Items;

				int numbots = bots.Count() - 1;

				if (numbots != -1)
				{
					for (int i = 0; i <= numbots; i++)
					{
						PlayerBot bot = bots[i];

						int bposx = bot.Pos.BlockX;
						int bposy = bot.Pos.BlockY;
						int bposz = bot.Pos.BlockZ;

						int posx = p.Pos.BlockX;
						int posy = p.Pos.BlockY;
						int posz = p.Pos.BlockZ;

						if (Math.Abs(bposx - posx) <= 1 && Math.Abs(bposy - posy) <= 1 && Math.Abs(bposz - posz) <= 1)
						{
							string botnohp = "";
							var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
							botnohp = bot.name.TrimEnd(digits);

							if (p.Extras.GetInt("botsleep" + botnohp) <= 0)
							{
								phealth(p, bot, botnohp);
							}
							p.Extras["botsleep" + botnohp] = p.Extras.GetInt("botsleep" + botnohp) - 1;
						}
					}
				}
			}
		}
		public override void Unload(bool shutdown)
		{
			Server.MainScheduler.Cancel(task);

			OnPlayerClickEvent.Unregister(ammo);

			OnGettingMotdEvent.Unregister(onjoin);

			Command.Unregister(Command.Find("bothp"));
		}

		public sealed class Cmdbothp : Command2
		{
			public override string name { get { return "bothp"; } }
			public override string type { get { return CommandTypes.Games; } }
			public override bool museumUsable { get { return false; } }
			public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
			public override bool SuperUseable { get { return false; } }

			public override void Use(Player p, string message, CommandData data)
			{
				if (!p.level.Config.MOTD.ToLower().Contains("+guns")) return;

				message = message + " null";
				string[] botName2 = message.SplitSpaces();
				string botNamenohp = botName2[1];
				if (botName2[0] != "spawn")
				{
					if (p.Extras.GetBoolean("runing") == false)
					{
						p.Extras["runing"] = true;

						string botName = botName2[0];
						int bothp = Convert.ToInt32(string.Concat(botName.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse()));

						int held = p.GetHeldBlock();
						if (held >= 66) held = held - 256;

						bothp = bothp - p.Extras.GetInt("wdamage");

						string newName = botNamenohp + bothp.ToString();

						string text = "/bothp " + newName + " " + botNamenohp;

						SetBotText(p, botName, text, data.Rank);

						if (bothp <= 0)
						{
							RemoveBot(p, botName, botNamenohp);
						}
						else
						{
							RenameBot(p, botName, newName);
							p.Extras["botName"] = botName;
							string nick = "%f-|%4" + bothp.ToString() + "%f|-";
							SetBotNick(p, newName, nick);
						}
						Thread.Sleep(p.Extras.GetInt("sleep"));
						p.Extras["runing"] = false;
					}
				}//if first arg spawn add a bot with health
				else
                {
					
					if (!p.level.Config.MOTD.ToLower().Contains("+guns")) return;
					
					p.Extras["runing"] = false;
					string[] starthp2 = message.SplitSpaces();
					string starthp = starthp2[2];
					string aiIns = starthp2[3];
					string botName = botNamenohp + starthp;
					string text = "/bothp " + botName + " " + botNamenohp;

					AddBot(p, botName);

					SetBotText(p, botName, text, data.Rank);

					string nick = "%f-|%4" + starthp + "%f|-";
					SetBotNick(p, botName, nick);

					if (aiIns != "null")
					{
						string aimessage = botName + " " + aiIns;
						AI(p, aimessage, data);
					}
				}
			}

			void AddBot(Player p, string botName)
			{
				botName = botName.Replace(' ', '_');
				PlayerBot bot = new PlayerBot(botName, p.level);
				bot.Owner = p.name;
				TryAddBot(p, bot);
			}

			void TryAddBot(Player p, PlayerBot bot)
			{
				if (BotExists(p.level, bot.name, null))
				{
					p.Message("A bot with that name already exists."); return;
				}
				if (p.level.Bots.Count >= Server.Config.MaxBotsPerLevel)
				{
					p.Message("Reached maximum number of bots allowed on this map."); return;
				}

				Random wr = new Random();
				Thread.Sleep(wr.Next(100, 250));
				Random lr = new Random();
				Level lvl = p.level;
				Position Pos = new Position((wr.Next(0, lvl.Width)) * 32, (lvl.Height - 1) * 32, (lr.Next(0, lvl.Length)) * 32);

				bot.SetInitialPos(Pos);

				//bot.SetInitialPos(p.Pos);
				bot.SetYawPitch(p.Rot.RotY, 0);

				//p.Message("You added the bot " + bot.ColoredName);
				PlayerBot.Add(bot);
			}

			void RemoveBot(Player p, string botName , string botNamenohp)
			{
					PlayerBot bot = Matcher.FindBots(p, botName);
					if (bot == null) return;

					PlayerBot.Remove(bot);
				//kill message
					p.Message("killed " + botNamenohp, bot.ColoredName);
				
			}

			void SetBotText(Player p, string botName, string text, LevelPermission plRank)
			{
				PlayerBot bot = Matcher.FindBots(p, botName);
				if (bot == null) return;

				if (text == null)
				{
					p.Message("Removed text shown when bot {0} &Sclicked on", bot.ColoredName);
					bot.ClickedOnText = null;
				}
				else
				{
					//just copied the code from cmdbot this message is when there is a hit 
					p.Message("", bot.ColoredName, text);

					bot.ClickedOnText = text;
				}
				BotsFile.Save(p.level);
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
			void RenameBot(Player p, string botName, string newName)
			{
				if (newName == null) { p.Message("New name of bot required."); return; }

				PlayerBot bot = Matcher.FindBots(p, botName);
				if (bot == null) return;
				if (BotExists(p.level, newName, bot))
				{
					p.Message("A bot with the new name already exists change your spawning code so this doesnt happen."); return;
				}
				
				if (bot.DisplayName == bot.name)
				{
					bot.DisplayName = newName;
					bot.GlobalDespawn();
					bot.GlobalSpawn();
				}

				bot.name = newName;
				BotsFile.Save(p.level);
			}

			void SetBotNick(Player p, string newName, string nick)
			{
				string botName = newName;
				PlayerBot bot = Matcher.FindBots(p, botName);
				bot.DisplayName = Colors.Escape(nick);

				bot.GlobalDespawn();
				bot.GlobalSpawn();
				BotsFile.Save(p.level);
			}

			void AI(Player p, string aimessage, CommandData data)
			{
				if (aimessage.Length == 0) { Help(p); return; }
				string[] args = aimessage.SplitSpaces();
				PlayerBot bot = Matcher.FindBots(p, args[0]);
				if (bot == null) return;

				string ai = args[1].ToLower();
				if (ai.CaselessEq("hunt"))
				{
					bot.hunt = !bot.hunt;
					bot.Instructions.Clear();
					bot.AIName = null;
					UpdateBot(p, bot, "" + bot.hunt);
					return;
				}
				else if (ai.CaselessEq("kill"))
				{
					if (!CheckExtraPerm(p, data, 1)) return;
					bot.kill = !bot.kill;
					UpdateBot(p, bot, "" + bot.kill);
					return;
				}

				if (!ScriptFile.Parse(p, bot, ai)) return;
				UpdateBot(p, bot, "" + ai);
			}

			static void UpdateBot(Player p, PlayerBot bot, string msg)
			{
				Logger.Log(LogType.UserActivity, bot.name + msg);
				BotsFile.Save(p.level);
			}


			public override void Help(Player p)
			{
				p.Message("%T/bothp %H- gives bots hp; to add a bot with hp run /bothp spawn [bot name] [hp] <ai>.");
			}
		}

		void ammo(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
		{
			if (!p.level.Config.MOTD.ToLower().Contains("+guns")) return;

			if (button == MouseButton.Left)
			{
				if (action != MouseAction.Pressed) return;
				int held = p.GetHeldBlock();
				int maxammo = p.Extras.GetInt("maxammo");
				int delay = p.Extras.GetInt("delay");
				int shotdelay = p.Extras.GetInt("sleep");

				if (p.Extras.GetInt("ammo" + held.ToString()) <= 0)
				{
					p.Extras["ammo" + held.ToString()] = 0;

					p.SendCpeMessage(CpeMessageType.Status2, "%freloading...");
					Thread.Sleep(delay);
					p.Extras["ammo" + held.ToString()] = maxammo;
					p.SendCpeMessage(CpeMessageType.Status2, "%f[%b" + p.Extras["ammo" + held.ToString()] + "%f/%b" + maxammo + "%f]");
					return;
				}
				else
				{
						if (p.Extras.GetInt("ammo" + held.ToString()) >= maxammo)
						{
							p.Extras["ammo" + held.ToString()] = maxammo;
						}
						p.Extras["ammo" + held.ToString()] = p.Extras.GetInt("ammo" + held.ToString()) - 1;
						p.SendCpeMessage(CpeMessageType.Status2, "%f[%b" + p.Extras["ammo" + held.ToString()] + "%f/%b" + maxammo + "%f]");
					
						//return;
				}
			}
			else
			{
				if (button != MouseButton.Right) return;

				if (action != MouseAction.Pressed) return;

				int held = p.GetHeldBlock();
				int maxammo = p.Extras.GetInt("maxammo");
				int delay = p.Extras.GetInt("delay");

				p.Extras["runing"] = true;
				p.Extras["ammo" + held.ToString()] = 0;

				p.SendCpeMessage(CpeMessageType.Status2, "%freloading...");
				Thread.Sleep(delay);
				p.Extras["ammo" + held.ToString()] = maxammo;
				p.SendCpeMessage(CpeMessageType.Status2, "%f[%b" + p.Extras["ammo" + held.ToString()] + "%f/%b" + maxammo + "%f]");
				p.Extras["runing"] = false;
				return;
			}
		}

		static void phealth(Player p, PlayerBot bot, string botnohp)
		{
			//default damage done
			int damage = 10;

			//change damage/speed based on model
			if (bot.Model == "zombie")
			{
				p.Extras["botsleep" + botnohp] = 10;
				damage = 30;
			}
			else if (bot.Model == "giant")
			{
				p.Extras["botsleep" + botnohp] = 20;
				damage = 50;
			}
			else if (bot.Model == "spider")
			{
				p.Extras["botsleep" + botnohp] = 8;
				damage = 15;
			}
			else if (bot.Model == "skeleton")
			{
				p.Extras["botsleep" + botnohp] = 8;
				damage = 30;
			}
			else
			{
				p.Extras["botsleep" + botnohp] = 10;
				damage = 10;
			}
			dodamage(p, damage, bot);
		}
		//do the damage (this makes it more flexable)
		static void dodamage(Player p, int damage, PlayerBot bot)
		{
			int health = p.Extras.GetInt("health");

			health = health - damage;
			if (health <= 0)
			{
				//set custom death msg
				string msg = "@p has died";
				p.HandleDeath(Block.Cobblestone, msg);
				health = 100;
			}
			if (health > 15)
			{
				p.SendCpeMessage(CpeMessageType.BottomRight2, "%f-|[%a" + health.ToString() + "%f]|-");
			}
			else
			{
				p.SendCpeMessage(CpeMessageType.BottomRight2, "%f-|[%c" + health.ToString() + "%f]|-");
			}
			p.Extras["health"] = health;

			int xv = -1;
			int yv = 0;
			int zv = -1;
			if ((p.Pos.X - bot.Pos.X) > 0) xv = 1;
			if ((p.Pos.Y - bot.Pos.Y) > 0) yv = 1;
			if ((p.Pos.Z - bot.Pos.Z) > 0) zv = 1;

			p.Send(Packet.VelocityControl(xv, yv, zv, 0, 0, 0));
		}

		static void onjoin(Player p, ref string motd)
		{
			if (p.level.Config.MOTD.ToLower().Contains("+guns"))
			{
				p.UpdateModel("humanoid");

				p.Extras["health"] = 100;
				p.SendCpeMessage(CpeMessageType.BottomRight2, "%f-|[%a100%f]|-");

				p.Extras["lastheld"] = -1;

				p.Send(Packet.SetHotbar(70, 0, false));
				p.Send(Packet.SetHotbar(71, 1, false));
				p.Send(Packet.SetHotbar(72, 2, false));
				p.Send(Packet.SetHotbar(73, 3, false));
				p.Send(Packet.SetHotbar(0, 4, false));
				p.Send(Packet.SetHotbar(0, 5, false));
				p.Send(Packet.SetHotbar(0, 6, false));
				p.Send(Packet.SetHotbar(0, 7, false));
				p.Send(Packet.SetHotbar(0, 8, false));
			}
			else
            {
				p.Send(Packet.SetHotbar(1, 0, false));
				p.Send(Packet.SetHotbar(4, 1, false));
				p.Send(Packet.SetHotbar(45, 2, false));
				p.Send(Packet.SetHotbar(3, 3, false));
				p.Send(Packet.SetHotbar(5, 4, false));
				p.Send(Packet.SetHotbar(17, 5, false));
				p.Send(Packet.SetHotbar(18, 6, false));
				p.Send(Packet.SetHotbar(20, 7, false));
				p.Send(Packet.SetHotbar(44, 8, false));

				p.SendCpeMessage(CpeMessageType.BottomRight2, "");
				p.SendCpeMessage(CpeMessageType.Status2, "");
			}
		}

		// Displays help for or information about this plugin
		public override void Help(Player p)
		{
			p.Message("a plugin for dropin pvp/pve with guns");
		}
	}
}