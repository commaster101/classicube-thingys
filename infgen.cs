//	Auto-generated plugin skeleton class
//	Use this as a basis for custom MCGalaxy plugins

// To reference other assemblies, put a "//reference [assembly filename]" at the top of the file
//   e.g. to reference the System.Data assembly, put "//reference System.Data.dll"

// Add any other using statements you need after this
using System;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using MCGalaxy.Generator;

namespace MCGalaxy
{
	public class infworld : Plugin
	{
		public override string name { get { return "infgen"; } }

		public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

		public override string welcome { get { return "infworld loaded"; } }

		public override string creator { get { return "LJplays"; } }

		public static SchedulerTask task;
		// Called when this plugin is being loaded (e.g. on server startup)
		public override void Load(bool startup)
		{
			task = Server.MainScheduler.QueueRepeat(use, null, TimeSpan.FromMilliseconds(250));
		}

		// Called when this plugin is being unloaded (e.g. on server shutdown)
		public override void Unload(bool shutdown)
		{
			Server.MainScheduler.Cancel(task);
		}

		static void use(SchedulerTask task)
        {
			Player[] players = PlayerInfo.Online.Items;
			foreach (Player p in players)
			{
				if (!p.level.Config.MOTD.ToLower().Contains("+infgen")) continue;
				string[] bits = p.level.name.Split('_');
				if (bits.Length != 2) continue;
				string[] chunks = bits[1].Split(',');
				if (chunks.Length != 2) continue;
				if (p.Pos.BlockX <= 0) gen(p, bits[0] + "_" + (Int32.Parse(chunks[0]) - 1).ToString() + "," + chunks[1], 1);//-x
				else if (p.Pos.BlockZ <= 0) gen(p, bits[0] + "_" + chunks[0] + "," + (Int32.Parse(chunks[1]) - 1).ToString(), 2);//-z
				else if (p.Pos.BlockX >= p.level.MaxX) gen(p, bits[0] + "_" + (Int32.Parse(chunks[0]) + 1).ToString() + "," + chunks[1], 3);//+x
				else if (p.Pos.BlockZ >= p.level.MaxZ) gen(p, bits[0] + "_" + chunks[0] + "," + (Int32.Parse(chunks[1]) + 1).ToString(), 4);//+z
				else { p.Extras["changedlvl"] = false; }
			}
		}

		static void gen(Player p,string mapname,byte directoin)
        {
			if (p.Extras.GetBoolean("changedlvl") == true) return; 
			if (LevelInfo.MapExists(mapname))
			{
				Position pos;
				Vec3S32 P = p.Pos.FeetBlockCoords;
				PlayerActions.ChangeMap(p, mapname);
				p.BlockUntilLoad(10);  //Wait for player to spawn in new map
                switch (directoin)
                {
					case 1:
						pos = Position.FromFeetBlockCoords((p.level.MaxX + 1), P.Y, P.Z);
						break;
					case 2:
						pos = Position.FromFeetBlockCoords(P.X, P.Y, (p.level.MaxZ + 1));
						break;
					case 3:
						pos = Position.FromFeetBlockCoords(-1, P.Y, P.Z);
						break;
					case 4:
						pos = Position.FromFeetBlockCoords(P.X, P.Y, -1);
						break;
					default:
						p.Message("%can error occurred when changing maps");
						p.Message("%please report this to LJplays");
						pos = Position.FromFeetBlockCoords(100, 100, 100);
						break;
				}
				p.Extras["changedlvl"] = true;
				p.SendPos(Entities.SelfID, pos, p.Rot);
				return;
			}
			Level lvl = null;
			try
			{
				MapGen gen = MapGen.Find("nasgen");
				lvl = MapGen.Generate(p, gen, mapname, 384, 256, 384, "dorpoj");
				if (lvl == null) return;

				lvl.Save(true);
			}
			finally
			{
				if (lvl != null) lvl.Dispose();
				Server.DoGC();
			}
		}
	
		// Displays help for or information about this plugin
		public override void Help(Player p)
		{
			p.Message("No help is available for this plugin.");
		}
	}
}