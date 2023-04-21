using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace WotlkBotGui
{
    public partial class MainWindow : Form
    {
        private Database db;
        private List<Thread> threads = new List<Thread>();
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
                    threads.Add(t);
                    t.Start();
                }
            }
            buttonStop.Enabled = true;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
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
           KillThreads();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
            KillThreads();
            buttonStart.Enabled = true;
        }

        private void KillThreads()
        {
            foreach (Thread t in threads)
            {
                t.Abort();
            }
        }
    }
}
