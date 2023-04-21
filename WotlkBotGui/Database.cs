using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static IronPython.Modules._ast;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        string cs = @"server=localhost;userid=bot;password=bot;database=bot";

        public void Init()
        {
            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                var cmd = new MySqlCommand();
                cmd.Connection = con;

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS bots (" +
                    "id INTEGER PRIMARY KEY AUTO_INCREMENT, " +
                    "account_name TEXT, " +
                    "char_name TEXT, " +
                    "password TEXT, " +
                    "script TEXT, " +
                    "class INT)";
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public List<Bot> GetBots()
        {
            List<Bot> bots = new List<Bot>();

            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT id, account_name, char_name, password, script, class FROM bots";
                var cmd = new MySqlCommand(sql, con);
                MySqlDataReader rdr = cmd.ExecuteReader();

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
            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                var sql = "INSERT INTO bots(account_name, char_name, password, script, class) VALUES(@account_name, @char_name, @password, @script, @class);";
                var cmd = new MySqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@account_name", bot.AccountName);
                cmd.Parameters.AddWithValue("@char_name", bot.CharName);
                cmd.Parameters.AddWithValue("@password", bot.Password);
                cmd.Parameters.AddWithValue("@script", bot.Script);
                cmd.Parameters.AddWithValue("@class", bot.Class);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
                bot.ID = (int)cmd.LastInsertedId;
            }
            return bot;
        }

        public void Update(Bot bot)
        {
            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                var sql = "REPLACE INTO bots(id, account_name, char_name, password, script, class) VALUES(@id, @account_name, @char_name, @password, @script, @class)";
                var cmd = new MySqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@account_name", bot.AccountName);
                cmd.Parameters.AddWithValue("@char_name", bot.CharName);
                cmd.Parameters.AddWithValue("@password", bot.Password);
                cmd.Parameters.AddWithValue("@script", bot.Script);
                cmd.Parameters.AddWithValue("@class", bot.Class);
                cmd.Parameters.AddWithValue("@id", bot.ID);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteBot(Bot bot)
        {
            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                var sql = "DELETE FROM bots WHERE id = @id";
                var cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", bot.ID);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
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
