using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IronPython.Modules._ast;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WotlkBotGui.Database;

namespace WotlkBotGui
{
    public enum Cls
    {
        Warrior = 1,
        Paladin = 2,
        Hunter = 3,
        Rogue = 4,
        Priest = 5,
        DeathKnight = 6,
        Shaman = 7,
        Mage = 8,
        Warlock = 9,
        Druid = 11
    }

    public class Database
    {
        //string cs = @"server=localhost;userid=bot;password=bot;database=bot";
        string cs = "Data Source=database.db;";

        public void Init()
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                
                SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS bots (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "account_name TEXT, " +
                    "char_name TEXT, " +
                    "password TEXT, " +
                    "script TEXT, " +
                    "class INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS config (id INTEGER PRIMARY KEY AUTOINCREMENT, host TEXT, master TEXT)";
                cmd.ExecuteNonQuery();

                con.Close();
            }
        }

        public string GetMaster()
        {
            string master = "";
            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                string sql = "SELECT master FROM config";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                SqliteDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    master = rdr.GetString(0);
            }
            return master; 
        }

        public string GetHost()
        {
            string host = "";
            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                string sql = "SELECT host FROM config";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                SqliteDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    host = rdr.GetString(0);
            }
            return host;
        }

        public void UpdateConfig(string host, string master)
        {
            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                string sql = "REPLACE INTO config (id, master, host) VALUES(1, @master,@host)";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@master", master);
                cmd.Parameters.AddWithValue("@host", host);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public List<Bot> GetBots()
        {
            List<Bot> bots = new List<Bot>();

            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                string sql = "SELECT id, account_name, char_name, password, script, class FROM bots";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                SqliteDataReader rdr= cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var bot = new Bot();
                    bot.ID = rdr.GetInt32(0);
                    bot.AccountName = rdr.GetString(1);
                    bot.CharName = rdr.GetString(2);
                    bot.Password = rdr.GetString(3);
                    bot.Script = rdr.GetString(4);
                    bot.Class = rdr.GetInt32(5);
                    bots.Add(bot);
                }
            }
            return bots;
        }

        public Bot AddBot()
        {
            Bot bot = new Bot { AccountName = "New", CharName = "", Password = "", Script = "", Class = (int)Cls.Warrior };
            
            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                var sql = "INSERT INTO bots(account_name, char_name, password, script, class) VALUES(@account_name, @char_name, @password, @script, @class);";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@account_name", bot.AccountName);
                cmd.Parameters.AddWithValue("@char_name", bot.CharName);
                cmd.Parameters.AddWithValue("@password", bot.Password);
                cmd.Parameters.AddWithValue("@script", bot.Script);
                cmd.Parameters.AddWithValue("@class", bot.Class);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT last_insert_rowid()";
                bot.ID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            
            return bot;
        }

        public void Update(Bot bot)
        {
            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                var sql = "REPLACE INTO bots(id, account_name, char_name, password, script, class) VALUES(@id, @account_name, @char_name, @password, @script, @class)";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@account_name", bot.AccountName);
                cmd.Parameters.AddWithValue("@char_name", bot.CharName);
                cmd.Parameters.AddWithValue("@password", bot.Password);
                cmd.Parameters.AddWithValue("@script", bot.Script);
                cmd.Parameters.AddWithValue("@class", bot.Class);
                cmd.Parameters.AddWithValue("@id", bot.ID);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void DeleteBot(Bot bot)
        {
            using (var con = new SqliteConnection(cs))
            {
                con.Open();
                var sql = "DELETE FROM bots WHERE id = @id";
                var cmd = con.CreateCommand();
                cmd.CommandText= sql;
                cmd.Parameters.AddWithValue("@id", bot.ID);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    public class Bot
    {
        public int ID { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string CharName { get; set; }
        public string Script { get; set; }
        public int Class { get; set; }
        public bool Run { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1} - {2} ({3})", Run ? "X" : "  ", AccountName, CharName, Enum.GetName(typeof(Cls), Class));
        }
    }
}
