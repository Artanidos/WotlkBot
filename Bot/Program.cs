using System;
using WotlkClient;
using WotlkClient.Clients;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using static Community.CsharpSqlite.Sqlite3;
using WotlkClient.Constants;

namespace WotlkBot
{
    partial class Program
    {
        static LogonServerClient lclient;
        static WorldServerClient wclient;
        static string accountName;
        static string charName;

        public static void Main(string[] args)
        {
            string password;
            string host;
            int port = 3724;

            if (args.Length < 4)
            {
                System.Console.WriteLine("Usage: WotlkBot <host> <accountname> <password> <charname>");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
                return;
            }
            host = args[0];
            accountName = args[1];
            password = args[2];
            charName = args[3];

            System.Console.WriteLine("WotlkBot");
            System.Console.WriteLine("--------");
            System.Console.WriteLine("Trying to connect to " + host);
            System.Console.WriteLine("");

            LoginCompletedCallBack callback = LoginComplete;

            try
            {
                lclient = new LogonServerClient(host, port, accountName, password, callback);
                lclient.Connect();

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An error occured: {0}", ex.Message);
            }
            /* just a test to call python for later bot scripting
            string filename = "priest.py";
            string path = Assembly.GetExecutingAssembly().Location;
            string rootDir = Directory.GetParent(path).FullName;

            ScriptEngine engine = Python.CreateEngine();

            ScriptSource source;
            source = engine.CreateScriptSourceFromFile(rootDir + "\\" + filename);

            ScriptScope scope = engine.CreateScope();

            int result = source.ExecuteProgram();
            */

            while (true)
            {

            }
        }

        public static void LoginComplete(uint result)
        {
            if (result == 0)
            {
                RealmListCompletedCallBack callback = RealmListComplete;
                lclient.RequestRealmlist(callback);
            }
            else
                System.Console.WriteLine("Log in falied");
        }

        public static void RealmListComplete(uint result)
        {
            if (result == 0)
            {
                Realm? realm = null;
                System.Console.WriteLine("Realms");
                System.Console.WriteLine("------");
                foreach (Realm r in lclient.Realmlist)
                {
                    System.Console.WriteLine(r.Name);
                    realm = r;
                }
                System.Console.WriteLine("");

                if (realm != null)
                {
                    AuthCompletedCallBack callback = AuthCompleted;
                    wclient = new WorldServerClient(accountName, realm.Value, lclient.mKey, charName, callback);
                    wclient.Connect();
                }
            }
            else
                System.Console.WriteLine("Realmlist failed");
        }

        public static void AuthCompleted(uint result)
        {
            if (result == 0)
            {
                CharEnumCompletedCallBack callback = CharEnumComplete;
                wclient.CharEnumRequest(callback);
            }
            else
                System.Console.WriteLine("Auth failed");
        }

        public static void CharEnumComplete(uint result)
        {
            if(result == 0)
            {
                Character? toLogin = null;
                System.Console.WriteLine("Chars");
                System.Console.WriteLine("-----");
                foreach (Character ch in wclient.Charlist)
                {
                    Console.WriteLine(ch.Name + " (" + ch.Level + ")");
                    if (ch.Name == charName)
                        toLogin = ch;
                }
                System.Console.WriteLine("");

                if (toLogin.HasValue)
                {
                    CharLoginCompletedCallBack callback = CharLoginComplete;
                    wclient.LoginPlayer(toLogin.Value, callback);
                }
            }
            
        }
        public static void CharLoginComplete(uint result)
        {
            if(result == 0)
            {
                System.Console.WriteLine("Logged into world with " + charName);
            }
            else
                System.Console.WriteLine("Char login failed");
        }
    }
}
