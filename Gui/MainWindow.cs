using IronPython.Runtime;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Linq;
using System.Windows.Forms;
using WotlkClient.Network;
using System.Reflection.Emit;
using WotlkClient.Constants;
using WotlkClient.Clients;
using System.Collections;
using static Community.CsharpSqlite.Sqlite3;

namespace WotlkBotGui
{
    public partial class MainWindow : Form
    {
        private Database db;
        private List<BotMgr> bots = new List<BotMgr>();
        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            db.Init();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (textBoxMaster.Text == "")
            {
                MessageBox.Show("Please fillout the field MASTER. The bots accept invites only from this character.");
                return;
            }
            buttonStart.Enabled = false;
            foreach (Bot b in listBoxBots.Items)
            {
                if (b.Run)
                {
                    BotMgr bm = new BotMgr();
                    Thread t = new Thread(() => bm.Main(b, textBoxHost.Text, 3724, textBoxMaster.Text));
                    bots.Add(bm);
                    t.Start();
                }
            }
            buttonStop.Enabled = true;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            textBoxMaster.Text = db.GetMaster();
            textBoxHost.Text = db.GetHost();

            Cls[] values = (Cls[])Enum.GetValues(typeof(Cls));

            foreach (Cls value in values)
            {
                comboBoxClass.Items.Add(value.ToString());
            }

            List<Bot> bots;
            bots = db.GetBots();
            foreach (Bot b in bots)
            {
                b.Run = true;
                listBoxBots.Items.Add(b);
            }
            if(listBoxBots.Items.Count > 0)
                listBoxBots.SelectedIndex = 0;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Bot bot = db.AddBot();
            listBoxBots.Items.Add(bot);
            listBoxBots.SelectedIndex = listBoxBots.Items.Count - 1;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Bot bot = (Bot)listBoxBots.SelectedItem;
            db.DeleteBot(bot);
            listBoxBots.Items.Remove(bot);
        }

        private void textBoxAccount_TextChanged(object sender, EventArgs e)
        {
            Bot bot = (Bot)listBoxBots.SelectedItem;
            bot.AccountName = textBoxAccount.Text;
            listBoxBots.Items[listBoxBots.SelectedIndex] = bot;
            listBoxBots.Refresh();
            db.Update(bot);
        }

        private void textBoxCharacter_TextChanged(object sender, EventArgs e)
        {
            Bot bot = (Bot)listBoxBots.SelectedItem;
            bot.CharName = textBoxCharacter.Text;
            listBoxBots.Items[listBoxBots.SelectedIndex] = bot;
            listBoxBots.Refresh();
            db.Update(bot);
        }

        private void textBoxHost_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBoxBots_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxBots.SelectedItems.Count > 0)
            {
                Bot bot = (Bot)listBoxBots.SelectedItem;
                textBoxAccount.Text = bot.AccountName;
                textBoxCharacter.Text = bot.CharName;
                textBoxPassword.Text = bot.Password;
                comboBoxClass.Text = Enum.GetName(typeof(Cls), bot.Class);
                textBoxScript.Text = bot.Script;
                checkBoxStart.Checked = bot.Run;
            }
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            Bot bot = (Bot)listBoxBots.SelectedItem;
            bot.Password = textBoxPassword.Text;
            db.Update(bot);
        }

        private void comboBoxCLass_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bot bot = (Bot)listBoxBots.SelectedItem;
            Cls selectedClass = (Cls)Enum.Parse(typeof(Cls), comboBoxClass.Text);
            bot.Class = Convert.ToInt32(selectedClass);
            listBoxBots.Items[listBoxBots.SelectedIndex] = bot;
            listBoxBots.Refresh();
            db.Update(bot);
        }

        private void checkBoxStart_CheckedChanged(object sender, EventArgs e)
        {
            Bot bot = (Bot)listBoxBots.SelectedItem;
            bot.Run = checkBoxStart.Checked;
            listBoxBots.Items[listBoxBots.SelectedIndex] = bot;
            listBoxBots.Refresh();
        }

        private void textBoxScript_TextChanged(object sender, EventArgs e)
        {
            Bot bot = (Bot)listBoxBots.SelectedItem;
            bot.Script = textBoxScript.Text;
            db.Update(bot);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            db.UpdateConfig(textBoxHost.Text, textBoxMaster.Text);
            Logout();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
            Logout();
            buttonStart.Enabled = true;
        }

        private void Logout()
        {
            Console.WriteLine("Logout");
            foreach (BotMgr bm in bots)
            {
                bm.Logout();
            }
            bots.Clear();
        }

        public static byte[] HexStringToBytes(string hexString)
        {
            string hexStringNoSpaces = hexString.Replace(" ", "");
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < hexStringNoSpaces.Length; i += 2)
            {
                string byteString = hexStringNoSpaces.Substring(i, 2);
                byte byteValue = Convert.ToByte(byteString, 16);
                bytes.Add(byteValue);
            }
            return bytes.ToArray();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string hexString = "53 04 00 00 78 01 85 53 4d 48 15 51 14 3e f7 fa d4 f7 52 9c 30 cd ac 88 89 de 2a df e2 b5 09 82 e8 dd 5c 44 8f 36 46 b9 96 d4 9d 08 25 2d 6a 53 03 b3 89 08 0a 89 5c b4 28 04 a1 44 c5 85 54 50 0b 45 30 17 2d 6a dd 42 09 17 e2 32 10 dc f8 f3 7d e7 de 21 5f 16 9e e1 bb df f9 b9 df 9d 99 33 67 1a 45 c4 a6 53 c6 99 a3 32 65 e0 f7 5d 45 06 c6 00 e6 ea b0 fc e8 a0 9b 54 48 04 4b 36 9d 54 d1 e4 3e 51 99 01 4c 45 97 d4 ad 15 9d 43 8e b0 e9 8c 8a 67 f6 8b 19 c0 54 7c bf 9e 6e ad b8 13 19 c2 a6 cb 2a 5e f6 e2 18 29 29 33 80 a9 b8 47 dd 5a 31 cb 6d 01 36 9d d5 03 66 fd 01 ba bb cc 00 a6 07 f0 11 ff 7f f7 39 15 cf 79 71 ac 5b 19 c0 54 5c 3c 45 f7 e0 dd eb 4c 47 ee 0e 4b b0 ed 11 91 bb ef 9b 96 da 5e b6 2e 16 77 27 ba 7b be bd aa f8 4a ec 44 56 81 17 ee de e9 31 f0 9f 78 35 aa ba c5 cd eb ee 7c bb 24 c7 c6 cc fc d6 4a b2 79 36 2f 72 a3 41 72 d7 f2 fe 01 fc 19 58 9f 5a b1 72 39 92 66 5c 87 d8 97 e1 9f 1f 86 f5 99 0f d9 18 ca f1 af 47 e5 bf 76 36 c8 0e 33 4e d3 09 57 be 0d 27 85 df 8b cd 30 05 91 5b f0 3e 03 83 40 e6 af 63 bc 18 93 8b 9f 16 b4 81 7c 15 bc 57 58 9c dc 6e 12 f9 de e8 61 bb be 5e 11 99 af 5c 40 99 68 d9 78 e7 5a 36 d6 1d 5c 58 a2 4d 6c 87 47 1c 07 4e 04 80 e4 22 30 04 74 01 38 52 51 1d 2d ba ea e8 a0 a3 36 2f f5 85 1c 86 3b 42 ad 15 70 70 38 f9 1c 64 ce 23 c7 8a 93 91 fd 19 d9 b0 67 73 9b 8d 60 36 49 d9 50 50 f3 00 fa 3e 70 4e 22 e9 07 97 4c c9 3c 01 5b c4 e3 60 03 9e 0e fc 11 1c e1 5a 0b 71 3f 1a c2 78 20 f0 1b b0 c1 75 d3 fa fc ef c0 05 f4 90 e7 1c 09 8c b4 94 ce 3c 76 af 7b 9f bb ca d2 33 85 c8 5b f7 6f 48 9c 60 ff 43 80 fd f7 bd 64 3f 0f 62 17 c6 5e 64 3d e6 77 66 9f f9 ad 4f 02 7b 55 a6 80 2e";
            byte[] data = HexStringToBytes(hexString);
            PacketIn pi = new PacketIn(data, (int)WorldServerOpCode.SMSG_COMPRESSED_UPDATE_OBJECT);
            //Int32 size = pi.ReadInt32();
            //byte[] decomped = WotlkClient.Shared.Compression.Decompress(size, pi.ReadRemaining());
            //foreach (byte b in decomped)
            //{
            //    Console.Write($"{b:X2} ");
            //}
            //Console.WriteLine();
            //                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 c6 0b a9 70
            // 07 00 00 00 02 83 AC 01 40 01 10 00 AC 01 00 00 02 5F 41 00 00 00 00 00 00 AC 01 00 00 00 00 00 40 03 00 00 00 D1 17 00 00 00 00 80 3F 17 00 00 00 17 00 00 00 01 00 00 00 02 83 AA 01 40 01 10 00 AA 01 00 00 02 5F 41 00 00 00 00 00 30 AA 01 00 00 00 00 00 40 03 00 00 00 39 00 00 00 00 00 80 3F 17 00 00 00 17 00 00 00 01 00 00 00 23 00 00 00 23 00 00 00 02 83 AE 01 40 01 10 00 AE 01 00 00 02 5F 41 00 00 00 00 00 30 AE 01 00 00 00 00 00 40 03 00 00 00 74 05 00 00 00 00 80 3F 17 00 00 00 17 00 00 00 01 00 00 00 19 00 00 00 19 00 00 00 02 83 C8 01 40 01 10 00 C8 01 00 00 02 5F 41 20 00 00 00 00 30 C8 01 00 00 00 00 00 40 03 00 00 00 50 00 00 00 00 00 80 3F 17 00 00 00 17 00 00 00 01 00 00 00 01 00 00 00 14 00 00 00 14 00 00 00 02 83 B0 01 40 01 10 00 B0 01 00 00 02 5F 41 00 00 00 00 00 30 B0 01 00 00 00 00 00 40 03 00 00 00 23 00 00 00 00 00 80 3F 17 00 00 00 17 00 00 00 01 00 00 00 19 00 00 00 19 00 00 00 02 83 B4 01 40 01 10 00 B4 01 00 00 02 5F 41 20 00 00 00 00 00 B4 01 00 00 00 00 00 40 03 00 00 00 24 1B 00 00 00 00 80 3F 17 00 00 00 17 00 00 00 01 00 00 00 01 00 00 00 03 01 17 04 61 00 00 00 00 00 00 00 FD 72 00 00 70 A9 0B C6 14 94 12 C3 24 FF A5 42 50 CC 96 3F 00 00 00 00 00 00 20 40 00 00 E0 40 00 00 90 40 71 1C 97 40 00 00 20 40 00 00 E0 40 00 00 90 40 E0 0F 49 40 C3 F5 48 40 2A 15 00 80 13 97 01 C0 F8 DF 80 F5 21 08 00 00 4B 06 00 04 46 08 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 8A 02 00 02 00 3C 0F 00 0C 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 BC 6D DB B6 6D 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 E2 7E 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 FE 00 00 00 00 40 00 00 00 00 00 80 00 00 00 00 3F 00 00 00 17 00 00 00 19 00 00 00 00 00 80 3F 01 09 00 00 53 00 00 00 BB 00 00 00 64 00 00 00 53 00 00 00 BB 00 00 00 E8 03 00 00 64 00 00 00 E8 03 00 00 24 B8 C1 40 03 00 00 00 01 00 00 00 08 00 00 00 00 08 00 00 00 00 40 00 54 0B 00 00 D0 07 00 00 D0 07 00 00 02 2B C7 3E 00 00 C0 3F 31 00 00 00 31 00 00 00 0E EA A8 40 0E EA E8 40 00 00 00 00 00 00 80 3F 00 00 00 00 15 00 00 00 15 00 00 00 16 00 00 00 18 00 00 00 18 00 00 00 00 00 00 00 36 00 00 00 6B 00 00 00 2B 00 00 00 0B 00 00 00 0B 00 00 00 49 92 24 40 49 92 64 40 00 00 80 3F 08 00 05 09 04 00 00 01 0F 00 00 00 12 00 00 00 40 0F 00 00 D1 17 00 00 39 00 00 00 74 05 00 00 50 00 00 00 23 00 00 00 AC 01 00 00 00 00 00 40 AA 01 00 00 00 00 00 40 AE 01 00 00 00 00 00 40 C8 01 00 00 00 00 00 40 B0 01 00 00 00 00 00 40 B4 01 00 00 00 00 00 40 23 00 00 00 78 05 00 00 5F 00 00 00 04 00 0F 00 62 00 00 00 2C 01 2C 01 88 00 00 00 02 00 0F 00 A2 00 00 00 01 00 0F 00 AD 00 00 00 01 00 0F 00 B7 00 00 00 0F 00 0F 00 E4 00 00 00 01 00 0F 00 62 01 00 00 0F 00 0F 00 63 01 00 00 0F 00 0F 00 9F 01 00 00 01 00 01 00 51 02 00 00 0F 00 0F 00 F2 02 00 00 0F 00 0F 00 09 03 00 00 01 00 0F 00 0A 03 00 00 01 00 0F 00 02 00 00 00 2C 1E 7F 40 9B 55 8F 40 3F C6 8C 40 3F C6 8C 40 00 00 A0 40 00 00 A0 40 00 00 A0 40 00 00 A0 40 00 00 A0 40 00 00 A0 40 00 00 00 20 80 00 00 00 79 00 00 00 01 00 00 00 00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 80 3F FF FF FF FF 50 00 00 00 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00 19 00 00 00 1A 00 00 00
            WorldServerClient wc = new WorldServerClient();
            wc.HandleCompressedObjectUpdate(pi); 
        }
    }
}
