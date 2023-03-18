using System;
using MCGalaxy;
using MCGalaxy.Eco;
using MCGalaxy.Network;
using MCGalaxy.Tasks;
using MCGalaxy.SQL;
using System.Collections.Generic;

namespace Core
{
    public class cointime : Plugin
    {
        public override string creator { get { return "LJplays"; } }
        public override string MCGalaxy_Version { get { return "1.9.1.4"; } }
        public override string name { get { return "cointime"; } }

        SchedulerTask task;
        public override void Load(bool startup)
        {
            task = Server.MainScheduler.QueueRepeat(Use, null, TimeSpan.FromMinutes(30));
            
            InitDB();
            cointimeItem cointime = new cointimeItem();
            Economy.Items.Add(cointime);
            Item item = Economy.GetItem("cointime");
            item.Enabled = true;
        }

        public override void Unload(bool shutdown)
        {
            Server.MainScheduler.Cancel(task);

            Item item = Economy.GetItem("cointime");
            item.Enabled = false;
        }

        ColumnDesc[] createDatabase = new ColumnDesc[] {
            new ColumnDesc("Name", ColumnType.VarChar, 16),
            new ColumnDesc("Spent", ColumnType.VarChar, 255),
        };

        void InitDB()
        {
            Database.CreateTable("Coinbonus", createDatabase);
        }

        static void Use(SchedulerTask task)
        {
            Player[] players = PlayerInfo.Online.Items;

            foreach (Player p in players)
            {
                GiveReward(p);
            }
        }

        public static void GiveReward(Player p)
        {
            List<string[]> rows = Database.GetRows("Coinbonus", "Name, Spent", "WHERE Name=@0", p.truename);

            int bonus = rows.Count == 0 ? 0 : int.Parse(rows[0][1]);
            int reward = bonus*5;

            p.Message("%aYou got &6" + reward + " %a" + Server.Config.Currency + "%a. goto the shop to buy cointimemultiplyer or type /buy cointime to get 5 more coins every 30min");
            p.SetMoney(p.money + reward);
        }

        public sealed class cointimeItem : SimpleItem
        {
            public cointimeItem()
            {
                Aliases = new string[] { "coinbonus", "extracoin" }; // Aliases for store items. E.g, queuelevel and queuelvl would do the same thing
                Price = 50; // The cost of the item in server currency
            }

            public override string Name { get { return "cointime"; } } // The name of the store item under /store

            protected override void OnPurchase(Player p, string input)
            {
                string[] args = input.SplitSpaces();

                if (!CheckPrice(p)) return; // Check to see if player can afford the purchase

                List<string[]> pRows = Database.GetRows("Coinbonus", "Name, Spent", "WHERE Name=@0", p.truename);

                if (pRows.Count == 0)
                {
                    Database.AddRow("Coinbonus", "Name, Spent", p.truename, 1);
                }
                else
                {
                    int current = int.Parse(pRows[0][1]);
                    int newcoin = 1 + current;

                    Database.UpdateRows("Coinbonus", "Spent=@1", "WHERE NAME=@0", p.truename, newcoin);
                }

                Economy.MakePurchase(p, Price, "Example"); // Make the purchase
            }

            protected override void OnStoreCommand(Player p)
            {
                p.Message("%T/Buy {0} [description]", Name);
                OutputItemInfo(p);
                p.Message("[long description]");
            }
        }
    }
}