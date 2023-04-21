using System;
using System.Threading;
using WotlkClient.Clients;
using WotlkClient.Constants;

namespace WotlkBotGui
{
    internal class BotMgr
    {
        private LogonServerClient loginClient;
        private WorldServerClient worldClient;
        private Bot bot;
        string master;
        public void Main(Bot bot, string host, int port, string _master)
        {
            master = _master;

            LoginCompletedCallBack callback = new LoginCompletedCallBack(LoginComplete); ;
            this.bot = bot;
            try
            {
                loginClient = new LogonServerClient(host, port, bot.AccountName, bot.Password, callback);
                loginClient.Connect();
            }
            
            catch (Exception ex)
            {
                System.Console.WriteLine("An error occured: {0}", ex.Message);
            }

            while (true)
            {
                try
                {
                    Thread.Sleep(500);
                }
                catch (ThreadAbortException)
                {
                    loginClient.Disconnect();
                    System.Console.WriteLine("Thread has been aborted"); 
                }    
            }
        }

        public void LoginComplete(uint result)
        {
            if (result == 0)
            {
                RealmListCompletedCallBack callback = new RealmListCompletedCallBack(RealmListComplete);
                loginClient.RequestRealmlist(callback);
                
            }
            else
                System.Console.WriteLine("Log in failed");
        }

        public void RealmListComplete(uint result)
        {
            if (result == 0)
            {
                Realm? realm = null;
                foreach (Realm r in loginClient.Realmlist)
                {
                    realm = r;
                }
                
                if (realm != null)
                {
                    AuthCompletedCallBack callback = new AuthCompletedCallBack(AuthCompleted);
                    worldClient = new WorldServerClient(bot.AccountName, realm.Value, loginClient.mKey, bot.CharName, master, callback);
                    worldClient.Connect();
                }
            }
            else
                System.Console.WriteLine("Realmlist failed");
        }

        public void AuthCompleted(uint result)
        {
            if (result == 0)
            {
                CharEnumCompletedCallBack callback = new CharEnumCompletedCallBack(CharEnumComplete);
                worldClient.CharEnumRequest(callback);
            }
            else
                System.Console.WriteLine("Auth failed");
        }

        public void CharEnumComplete(uint result)
        {
            if (result == 0)
            {
                Character? toLogin = null;
                foreach (Character ch in worldClient.Charlist)
                {
                    if (ch.Name == bot.CharName)
                        toLogin = ch;
                }
                
                if (toLogin.HasValue)
                {
                    CharLoginCompletedCallBack callback = new CharLoginCompletedCallBack(CharLoginComplete);
                    worldClient.LoginPlayer(toLogin.Value, callback);
                }
            }
        }

        public void CharLoginComplete(uint result)
        {
            if (result == 0)
            {
                System.Console.WriteLine("Logged into world with " + bot.CharName);
            }
            else
                System.Console.WriteLine("Char login failed");
        }
    }
}
