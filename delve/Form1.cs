using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Threading;
using System.IO;

namespace delve
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Graphics gra;
        private Pen pen;
        private PointF prePoint;
        private Image image, backImage;
        private Color color;
        private int penWidth;
        private int alpha;
        private SoundPlayer soundRoll, sound1, sound2, sound3;
        private int[] diceValue = new int[7];
        private bool[] diceAside = new bool[7];
        private PictureBox[] theDice = new PictureBox[7];
        private Image[] dicePictures = new Bitmap[13];
        private PointF leftUp = new PointF();
        private RectangleF rec = new RectangleF();
        private Point pointO = new Point(0, 0);

        private int[] adventurers = new int[5];


        private void setPen(Color c, int w, int a)
        {
            alpha = a;
            color = Color.FromArgb(alpha, c);
            penWidth = w;
            if (pen != null)
                pen.Dispose();
            pen = new Pen(color, penWidth);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 6; i++)
            {
                diceValue[i] = i;
            }
            theDice[1] = dice1;
            theDice[2] = dice2;
            theDice[3] = dice3;
            theDice[4] = dice4;
            theDice[5] = dice5;
            theDice[6] = dice6;

            soundRoll = new SoundPlayer(Properties.Resources.sound);
            sound1 = new SoundPlayer(Properties.Resources.WHOOSH);
            sound2 = new SoundPlayer(Properties.Resources.type);
            sound3 = new SoundPlayer(Properties.Resources.COIN);
            setPen(Color.Red, 6, 128);
            setStateLabel("迷你地城 - 最初的冒险");
            dicePictures[1] = Properties.Resources.d1;
            dicePictures[2] = Properties.Resources.d2;
            dicePictures[3] = Properties.Resources.d3;
            dicePictures[4] = Properties.Resources.d4;
            dicePictures[5] = Properties.Resources.d5;
            dicePictures[6] = Properties.Resources.d6;
            dicePictures[7] = Properties.Resources.d11;
            dicePictures[8] = Properties.Resources.d22;
            dicePictures[9] = Properties.Resources.d33;
            dicePictures[10] = Properties.Resources.d44;
            dicePictures[11] = Properties.Resources.d55;
            dicePictures[12] = Properties.Resources.d66;
            backImage = Properties.Resources.DelveTheDiceGameStartingAdventure;
            start(backImage);

            comboBox1.SelectedIndex = 0;

            loadSkills();
            adventurers[1] = 1;
            adventurers[2] = 5;
            adventurers[3] = 6;
            adventurers[4] = 13;
            for (int i = 1; i <= 3; i++)
            {
                textBox1.Text += skills[adventurers[i]];
                textBox1.Text += "\r\n";
            }
            textBox1.Text += skills[adventurers[4]];
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (image != null)
            {
                pictureBox1.Width = this.Width - 6;
                pictureBox1.Height = pictureBox1.Width * image.Height / image.Width;
            }
        }
        private void start(Image loadImage)
        {
            image = (Image)loadImage.Clone();

            pictureBox1.BackgroundImage = image;

            if (gra != null)
                gra.Dispose();
            gra = Graphics.FromImage(image);
            gra.SmoothingMode = SmoothingMode.AntiAlias;

            pictureBox1.Width = this.Width - 6;
            pictureBox1.Height = pictureBox1.Width * image.Height / image.Width;

            gra.DrawLine(pen, pointO, new Point(1, 0));
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            prePoint.X = e.X * image.Width / (float)pictureBox1.Width;
            prePoint.Y = e.Y * image.Height / (float)pictureBox1.Height;
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF p = new PointF(e.X * image.Width / (float)pictureBox1.Width, e.Y * image.Height / (float)pictureBox1.Height);
                if (prePoint.IsEmpty)
                {
                    prePoint = p;
                    return;
                }
                gra.DrawLine(pen, prePoint, p);
                prePoint = p;
                pictureBox1.Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                PointF p = new PointF(e.X * image.Width / (float)pictureBox1.Width, e.Y * image.Height / (float)pictureBox1.Height);
                if (prePoint.IsEmpty)
                {
                    prePoint = p;
                    return;
                }
                leftUp.X = prePoint.X < p.X ? prePoint.X : p.X;
                leftUp.Y = prePoint.Y < p.Y ? prePoint.Y : p.Y;
                rec.Location = leftUp;
                rec.Width = Math.Abs(prePoint.X - p.X);
                rec.Height = Math.Abs(prePoint.Y - p.Y);
                gra.SetClip(rec);

                gra.DrawImage(backImage, pointO);
                gra.ResetClip();

                pictureBox1.Refresh();
            }
        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            splitContainer1.Panel1.Select();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            prePoint = PointF.Empty;
        }
        private void setStateLabel(string msg)
        {
            statusStrip1.Items[0].Text = msg;
        }
        private string getStateLabel()
        {
            return statusStrip1.Items[0].Text;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (音效ToolStripMenuItem.Checked == true)
                soundRoll.Play();

            Random ran = new Random();

            for (int j = 1; j <= 20; j++)
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (theDice[i].Visible == false || diceAside[i] == true)
                    {
                        continue;
                    }
                    diceValue[i] = ran.Next(1, 7);
                    setDice(theDice[i], diceValue[i], diceAside[i]);

                }
                panel1.Refresh();
                Thread.Sleep(100);
            }
        }

        private void setDice(PictureBox picBox, int diceValue, bool aside)
        {
            if (aside == false)
            {
                picBox.Image = dicePictures[diceValue];
            }
            else
            {
                picBox.Image = dicePictures[diceValue + 6];
            }
        }


        #region 菜单

        private void 退出游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确实要退出迷你地城吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
        }

        private void 重新开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start(backImage);
        }

        private void 最初的冒险ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            最初的冒险ToolStripMenuItem.Checked = true;
            时之沙ToolStripMenuItem.Checked = false;
            空白场景ToolStripMenuItem.Checked = false;
            setStateLabel("迷你地城 - 最初的冒险");
            backImage = Properties.Resources.DelveTheDiceGameStartingAdventure;
            start(backImage);
        }

        private void 时之沙ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            最初的冒险ToolStripMenuItem.Checked = false;
            时之沙ToolStripMenuItem.Checked = true;
            空白场景ToolStripMenuItem.Checked = false;
            setStateLabel("迷你地城 - 时之沙");
            backImage = Properties.Resources.DelveTheDiceGameSandsOfTime;
            start(backImage);
        }

        private void 空白场景ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            最初的冒险ToolStripMenuItem.Checked = false;
            时之沙ToolStripMenuItem.Checked = false;
            空白场景ToolStripMenuItem.Checked = true;
            setStateLabel("迷你地城 - 空白场景");
            backImage = Properties.Resources.BlankDelvePlaysheet;
            start(backImage);
        }

        private void 红色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            红色ToolStripMenuItem.Checked = true;
            蓝色ToolStripMenuItem.Checked = false;
            绿色ToolStripMenuItem.Checked = false;
            黑色ToolStripMenuItem.Checked = false;
            setPen(Color.Red, penWidth, alpha);
        }

        private void 蓝色toolStripMenuItem_Click(object sender, EventArgs e)
        {
            红色ToolStripMenuItem.Checked = false;
            蓝色ToolStripMenuItem.Checked = true;
            绿色ToolStripMenuItem.Checked = false;
            黑色ToolStripMenuItem.Checked = false;
            setPen(Color.Blue, penWidth, alpha);
        }

        private void 绿色toolStripMenuItem_Click(object sender, EventArgs e)
        {
            红色ToolStripMenuItem.Checked = false;
            蓝色ToolStripMenuItem.Checked = false;
            绿色ToolStripMenuItem.Checked = true;
            黑色ToolStripMenuItem.Checked = false;
            setPen(Color.Green, penWidth, alpha);
        }

        private void 黑色toolStripMenuItem_Click(object sender, EventArgs e)
        {
            红色ToolStripMenuItem.Checked = false;
            蓝色ToolStripMenuItem.Checked = false;
            绿色ToolStripMenuItem.Checked = false;
            黑色ToolStripMenuItem.Checked = true;
            setPen(Color.Black, penWidth, alpha);
        }

        private void 较细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            较细ToolStripMenuItem.Checked = true;
            中等ToolStripMenuItem.Checked = false;
            较粗ToolStripMenuItem.Checked = false;
            setPen(color, 2, alpha);
        }

        private void 中等ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            较细ToolStripMenuItem.Checked = false;
            中等ToolStripMenuItem.Checked = true;
            较粗ToolStripMenuItem.Checked = false;
            setPen(color, 6, alpha);
        }

        private void 较粗ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            较细ToolStripMenuItem.Checked = false;
            中等ToolStripMenuItem.Checked = false;
            较粗ToolStripMenuItem.Checked = true;
            setPen(color, 12, alpha);
        }

        private void 半透明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            半透明ToolStripMenuItem.Checked = true;
            不透明ToolStripMenuItem.Checked = false;
            setPen(color, penWidth, 128);
        }

        private void 不透明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            半透明ToolStripMenuItem.Checked = false;
            不透明ToolStripMenuItem.Checked = true;
            setPen(color, penWidth, 255);
        }
        private void 鼠标指针铅笔ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (鼠标指针铅笔ToolStripMenuItem.Checked == true)
                pictureBox1.Cursor = new Cursor(Properties.Resources.pencil.GetHicon());
            else
                pictureBox1.Cursor = Cursors.Default;

        }
        private void 规则ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("迷你地城(DelveTheDiceGame).pdf") == false)
            {
                MessageBox.Show("规则书“迷你地城(DelveTheDiceGame).pdf”不在哦？去下载一个吧。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start("迷你地城(DelveTheDiceGame).pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 新人物表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("迷你地城新人物表.pdf") == false)
            {
                MessageBox.Show("“迷你地城新人物表.pdf”不在哦？去下载一个吧。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start("迷你地城新人物表.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.ShowDialog();
        }

        private void 使用技巧ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoteForm noteForm = new NoteForm();
            noteForm.ShowDialog();
        }

        #endregion

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBox1.SelectedIndex;
            for (int i = 1; i <= n; i++)
            {
                theDice[i].Visible = false;
            }
            for (int i = n + 1; i <= 6; i++)
            {
                theDice[i].Visible = true;
            }

            for (int i = 1; i <= 6; i++)
            {
                diceAside[i] = false;
                setDice(theDice[i], diceValue[i], diceAside[i]);
            }
        }


        private void dice_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int n = int.Parse(((PictureBox)sender).Name[4].ToString());
                if (diceAside[n] == true)
                {
                    if (音效ToolStripMenuItem.Checked == true)
                        sound2.Play();
                    diceAside[n] = false;
                }
                else
                {
                    if (音效ToolStripMenuItem.Checked == true)
                        sound1.Play();
                    diceAside[n] = true;
                }
                setDice(theDice[n], diceValue[n], diceAside[n]);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (音效ToolStripMenuItem.Checked == true)
                    sound3.Play();
                for (int i = 1; i <= 6; i++)
                {
                    diceAside[i] = false;
                    setDice(theDice[i], diceValue[i], diceAside[i]);
                }
            }
        }

        private void 选择冒险者ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PickAdventurers pickAdventurersForm = new PickAdventurers();
            pickAdventurersForm.ShowDialog();
            if (pickAdventurersForm.Ok == true)
            {
                adventurers = pickAdventurersForm.Picks;
                textBox1.Clear();
                for (int i = 1; i <= 3; i++)
                {
                    textBox1.Text += skills[adventurers[i]];
                    textBox1.Text += "\r\n";
                }
                textBox1.Text += skills[adventurers[4]];
            }
        }


        private string[] skills = new string[17];

        private void loadSkills()
        {
            skills[1] = "【战士】\r\n\t冲锋——每个6造成1点伤害，但是每回合最多使用2个6用来冲锋。";
            skills[2] = "【圣骑士】\r\n\t圣疗术——每个6治疗1点生命。每回合只能为1个冒险者治疗。\r\n\t驱邪术——两个6造成1点伤害。";
            skills[3] = "【矮人防御者】\r\n\t防御姿态——如果2个骰子的数字都与一个目标的当前生命值相等，这个目标在此回合不能攻击。可作用于多个目标，但是每个目标需要2个骰子（举例，四个骰子：3355，可以阻止生命为3和5的两个目标的攻击）";
            skills[4] = "【野蛮人】\r\n\t狂暴——每个6造成1点伤害，可以使用任意数量，而且可以对多个目标使用。如果骰子结果中有5，冒险者们要受到1点伤害。无论有多少5，冒险者们最多只受到1点伤害。5仍然可以用在其他技能中。";
            skills[5] = "【盗贼】\r\n\t偷袭——每个1造成1点伤害，可以使用任意多的1，但是每回合只能偷袭一个目标。\r\n\t要害攻击——Full house（又叫葫芦，即1对+3个相同数，如44555），伤害值为第6个骰子的数值。这个伤害只能用在一个目标上。";
            skills[6] = "【圣职者】\r\n\t次级治疗——4个连续数，如2345. 治疗2点生命，可加在不同的冒险者身上。\r\n\t治疗——5个连续数，如12345. 治疗量为第6个骰子的数值。可加在不同冒险者身上。特殊地，如果第6个骰子是1，为每个冒险者加1点生命。\r\n\t奇迹——6个连续数，即123456. 复活所有冒险者，并且所有冒险者都恢复为满的生命。";
            skills[7] = "【德鲁伊】\r\n\t动物伙伴——冒险者们每次受到1点以上的伤害时，你可以忽略1点伤害。\r\n\t野兽形态——将其他技能没有使用的骰子的点数加起来（之前投的不累加），除以4，向下取整，结果就是德鲁伊在这回合造成的伤害。";
            skills[8] = "【吟游诗人】\r\n\t（吟游诗人每回合只能使用下面技能中的一种）\r\n\t灵感——你进攻的骰子投完后，可以把其中的一个改为任一个数字。\r\n\t扰乱——当你替你的对手投骰子时，你可以重投一次全部的骰子。\r\n\t暗示——在怪物攻击前，你选择一个怪物然后投一个骰子，如果是1或2，这个怪物造成的伤害被转移给任何怪物身上，甚至此怪物自己身上。如果是3或4，暗示失败，跟通常一样造成伤害。如果是5或6，暗示严重失败，伤害优先由吟游诗人承受伤害。";
            skills[9] = "【游侠】\r\n\t远程攻击——每次战斗之前，投6个骰子，每有1个对子，造成2点伤害。\r\n\t双持攻击——每有1个对子，造成2点伤害。\r\n\t闪避——如果其他的冒险者都已经死亡，而且你正在跟一个命中点数是5和6的怪物战斗，那么在怪物进攻时只有投到6你才受到伤害。";
            skills[10] = "【僧侣】\r\n\t渗透劲——在连续两回合中，每回合都恰巧有5个骰子的点数和等于目标的当前生命，目标立即毙命。如果你尝试使用渗透劲，这两回合里你不能使用其他技能（包括其他人），即使渗透劲失败了也不能。\r\n\t斗气击——2颗骰子都恰好等于僧侣的当前生命，僧侣给一个目标造成相当于僧侣当前生命的伤害。\r\n\t空灵体——3颗骰子都恰好等于僧侣的当前生命，这回合里冒险者们受到的任何伤害均无效。\r\n\t混元体——4颗骰子都恰好等于僧侣的当前生命，僧侣恢复满生命。";
            skills[11] = "【附魔师】\r\n\t精神鼓舞——在你投攻击的骰子之前，你可以投一个骰子，这回合你根据这个骰子来确定你投攻击骰子的次数，而不是通常的3次。但如果是6，则跳过你的攻击阶段，直接轮到怪物攻击。\r\n\t强化英勇——每个回合里，在你投过攻击骰子之后，你可以骰一个骰子，如果是1或2，你造成的伤害加倍，如果是5或者6，你受到的伤害加倍，如果是3或者4，无效果。";
            skills[12] = "【刺客】\r\n\t夺命一击——4个1，直接杀死一个目标。\r\n\t毒药——无论是哪个冒险者使用哪些技能，每回合可以额外地造成1点伤害。";
            skills[13] = "【巫师】\r\n\t冰冻射线——3个相同数。这个回合，怪物少投2个骰子。\r\n\t闪电链——4个相同数。每个目标受到1点伤害。\r\n\t火球术——5个相同数。6点伤害，可以分配给多个目标。\r\n\t即死——6个相同数。直接杀死这场战斗中的所有怪物。";
            skills[14] = "【死灵法师】\r\n\t操纵死尸——冒险者死后被死灵法师操纵，每回合投一个骰子，造成相当于骰子点数的伤害。但他（们）不能攻击（使用技能），也不能被治疗。如果死灵法师死了，他（们）立即消失。\r\n\t通道——如果冒险者死后你选择不去操纵他（们），那么死灵法师可以获得这个冒险者的能力。\r\n\t吸取生命——每回合在你投攻击骰之前，你可以投4个骰子，怪物们承受相当于2个骰子点数之和的伤害，冒险者们承受相当于另2个骰子点数之和的伤害。死灵法师以及被操纵的死尸不受到这个伤害。";
            skills[15] = "【术士】\r\n\t用来发动术士技能的骰子，必须全都是在同一次投出的结果。\r\n\t冰墙——3个连续数，你的对手此回合不投骰子。\r\n\t疾病——4个连续数，每个目标减少生命，数值相当于目标当前生命的一半（向下取整）。比如目标生命为5，减少2点生命。\r\n\t火焰之环——5个连续数，在之后的游戏中，冒险者们受到的伤害同时也反弹给怪物。\r\n\t飓风——6个连续数，消灭这场战斗中的所有怪物。";
            skills[16] = "【奇术师】\r\n\t召唤次级异界盟友——2对（如1122），召唤一个次级异界盟友加入这场战斗，到下一场战斗时消失。\r\n\t召唤异界盟友——3对（如112233），召唤一个异界盟友，可存活到游戏结束。\r\n\t（无论是次级异界盟友还是异界盟友，在奇术师死后立即消失。他们不会受到伤害。在冒险者们行动之后，怪物攻击之前，为盟友投6个骰子，每个5或6造成1点伤害，可以分配给多个目标。奇术师不能同时拥有1个以上的盟友）";
        }

        private void 保存地城ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Application.StartupPath;
            saveDialog.Filter = "jpg files (*.jpg)|*.jpg";
            saveDialog.Title = "保存地城";
            if (saveDialog.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                File.Delete(saveDialog.FileName);
                image.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            backImage = new Bitmap(image);
            start(new Bitmap(backImage));

            setStateLabel("迷你地城 - " + Path.GetFileNameWithoutExtension(saveDialog.FileName));
            最初的冒险ToolStripMenuItem.Checked = false;
            时之沙ToolStripMenuItem.Checked = false;
            空白场景ToolStripMenuItem.Checked = false;
        }

        private void 载入地城ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image tempImage;
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Application.StartupPath;
            openDialog.Filter = "jpg files (*.jpg)|*.jpg";
            openDialog.Title = "载入地城";
            if (openDialog.ShowDialog() == DialogResult.Cancel)
                return;
            tempImage = Image.FromFile(openDialog.FileName);
            backImage = new Bitmap(tempImage);
            start(new Bitmap(backImage));
            setStateLabel("迷你地城 - " + Path.GetFileNameWithoutExtension(openDialog.FileName));

            tempImage.Dispose();

            最初的冒险ToolStripMenuItem.Checked = false;
            时之沙ToolStripMenuItem.Checked = false;
            空白场景ToolStripMenuItem.Checked = false;
        }

        private void 保存进度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveFileName = string.Format("{0}\\[进度]{1}.jpg", Application.StartupPath, getStateLabel().Remove(0, 7));
            try
            {
                File.Delete(saveFileName);
                image.Save(saveFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("保存进度成功！", getStateLabel(), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 载入进度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string backImageFileName, loadImageFileName;
            Image tempImage, loadImage;

            loadImageFileName = string.Format("{0}\\[进度]{1}.jpg", Application.StartupPath, getStateLabel().Remove(0, 7));
            if (File.Exists(loadImageFileName) == false)
            {
                MessageBox.Show(string.Format("进度文件({0})不存在，\r\n\r\n你还没有存过这个地城的进度吧？", loadImageFileName), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (最初的冒险ToolStripMenuItem.Checked == true)
                tempImage = Properties.Resources.DelveTheDiceGameStartingAdventure;
            else if (时之沙ToolStripMenuItem.Checked == true)
                tempImage = Properties.Resources.DelveTheDiceGameSandsOfTime;
            else if (空白场景ToolStripMenuItem.Checked == true)
                tempImage = Properties.Resources.DelveTheDiceGameSandsOfTime;
            else
            {
                backImageFileName = string.Format("{0}\\{1}.jpg", Application.StartupPath, getStateLabel().Remove(0, 7));
                if (File.Exists(backImageFileName) == false)
                {
                    MessageBox.Show(string.Format("地城文件({0})不存在，\r\n\r\n请将地城文件跟本程序放在同一位置!", backImageFileName), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tempImage = Image.FromFile(backImageFileName);
            }
            backImage = new Bitmap(tempImage);
            loadImage = Image.FromFile(loadImageFileName);
            start(new Bitmap(loadImage));
            tempImage.Dispose();
            loadImage.Dispose();
        }





    }
}