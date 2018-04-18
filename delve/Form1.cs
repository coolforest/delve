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
            setStateLabel("����س� - �����ð��");
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
            if (��ЧToolStripMenuItem.Checked == true)
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


        #region �˵�

        private void �˳���ϷToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ȷʵҪ�˳�����س���", "ȷ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
        }

        private void ���¿�ʼToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start(backImage);
        }

        private void �����ð��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            �����ð��ToolStripMenuItem.Checked = true;
            ʱ֮ɳToolStripMenuItem.Checked = false;
            �հ׳���ToolStripMenuItem.Checked = false;
            setStateLabel("����س� - �����ð��");
            backImage = Properties.Resources.DelveTheDiceGameStartingAdventure;
            start(backImage);
        }

        private void ʱ֮ɳToolStripMenuItem_Click(object sender, EventArgs e)
        {
            �����ð��ToolStripMenuItem.Checked = false;
            ʱ֮ɳToolStripMenuItem.Checked = true;
            �հ׳���ToolStripMenuItem.Checked = false;
            setStateLabel("����س� - ʱ֮ɳ");
            backImage = Properties.Resources.DelveTheDiceGameSandsOfTime;
            start(backImage);
        }

        private void �հ׳���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            �����ð��ToolStripMenuItem.Checked = false;
            ʱ֮ɳToolStripMenuItem.Checked = false;
            �հ׳���ToolStripMenuItem.Checked = true;
            setStateLabel("����س� - �հ׳���");
            backImage = Properties.Resources.BlankDelvePlaysheet;
            start(backImage);
        }

        private void ��ɫToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��ɫToolStripMenuItem.Checked = true;
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = false;
            setPen(Color.Red, penWidth, alpha);
        }

        private void ��ɫtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = true;
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = false;
            setPen(Color.Blue, penWidth, alpha);
        }

        private void ��ɫtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = true;
            ��ɫToolStripMenuItem.Checked = false;
            setPen(Color.Green, penWidth, alpha);
        }

        private void ��ɫtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = false;
            ��ɫToolStripMenuItem.Checked = true;
            setPen(Color.Black, penWidth, alpha);
        }

        private void ��ϸToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��ϸToolStripMenuItem.Checked = true;
            �е�ToolStripMenuItem.Checked = false;
            �ϴ�ToolStripMenuItem.Checked = false;
            setPen(color, 2, alpha);
        }

        private void �е�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��ϸToolStripMenuItem.Checked = false;
            �е�ToolStripMenuItem.Checked = true;
            �ϴ�ToolStripMenuItem.Checked = false;
            setPen(color, 6, alpha);
        }

        private void �ϴ�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��ϸToolStripMenuItem.Checked = false;
            �е�ToolStripMenuItem.Checked = false;
            �ϴ�ToolStripMenuItem.Checked = true;
            setPen(color, 12, alpha);
        }

        private void ��͸��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��͸��ToolStripMenuItem.Checked = true;
            ��͸��ToolStripMenuItem.Checked = false;
            setPen(color, penWidth, 128);
        }

        private void ��͸��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ��͸��ToolStripMenuItem.Checked = false;
            ��͸��ToolStripMenuItem.Checked = true;
            setPen(color, penWidth, 255);
        }
        private void ���ָ��Ǧ��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (���ָ��Ǧ��ToolStripMenuItem.Checked == true)
                pictureBox1.Cursor = new Cursor(Properties.Resources.pencil.GetHicon());
            else
                pictureBox1.Cursor = Cursors.Default;

        }
        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("����س�(DelveTheDiceGame).pdf") == false)
            {
                MessageBox.Show("�����顰����س�(DelveTheDiceGame).pdf������Ŷ��ȥ����һ���ɡ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start("����س�(DelveTheDiceGame).pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("����س��������.pdf") == false)
            {
                MessageBox.Show("������س��������.pdf������Ŷ��ȥ����һ���ɡ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start("����س��������.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.ShowDialog();
        }

        private void ʹ�ü���ToolStripMenuItem_Click(object sender, EventArgs e)
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
                    if (��ЧToolStripMenuItem.Checked == true)
                        sound2.Play();
                    diceAside[n] = false;
                }
                else
                {
                    if (��ЧToolStripMenuItem.Checked == true)
                        sound1.Play();
                    diceAside[n] = true;
                }
                setDice(theDice[n], diceValue[n], diceAside[n]);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (��ЧToolStripMenuItem.Checked == true)
                    sound3.Play();
                for (int i = 1; i <= 6; i++)
                {
                    diceAside[i] = false;
                    setDice(theDice[i], diceValue[i], diceAside[i]);
                }
            }
        }

        private void ѡ��ð����ToolStripMenuItem_Click(object sender, EventArgs e)
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
            skills[1] = "��սʿ��\r\n\t��桪��ÿ��6���1���˺�������ÿ�غ����ʹ��2��6������档";
            skills[2] = "��ʥ��ʿ��\r\n\tʥ��������ÿ��6����1��������ÿ�غ�ֻ��Ϊ1��ð�������ơ�\r\n\t��а����������6���1���˺���";
            skills[3] = "�����˷����ߡ�\r\n\t������̬�������2�����ӵ����ֶ���һ��Ŀ��ĵ�ǰ����ֵ��ȣ����Ŀ���ڴ˻غϲ��ܹ������������ڶ��Ŀ�꣬����ÿ��Ŀ����Ҫ2�����ӣ��������ĸ����ӣ�3355��������ֹ����Ϊ3��5������Ŀ��Ĺ�����";
            skills[4] = "��Ұ���ˡ�\r\n\t�񱩡���ÿ��6���1���˺�������ʹ���������������ҿ��ԶԶ��Ŀ��ʹ�á�������ӽ������5��ð������Ҫ�ܵ�1���˺��������ж���5��ð���������ֻ�ܵ�1���˺���5��Ȼ�����������������С�";
            skills[5] = "��������\r\n\t͵Ϯ����ÿ��1���1���˺�������ʹ��������1������ÿ�غ�ֻ��͵Ϯһ��Ŀ�ꡣ\r\n\tҪ����������Full house���ֽк�«����1��+3����ͬ������44555�����˺�ֵΪ��6�����ӵ���ֵ������˺�ֻ������һ��Ŀ���ϡ�";
            skills[6] = "��ʥְ�ߡ�\r\n\t�μ����ơ���4������������2345. ����2���������ɼ��ڲ�ͬ��ð�������ϡ�\r\n\t���ơ���5������������12345. ������Ϊ��6�����ӵ���ֵ���ɼ��ڲ�ͬð�������ϡ�����أ������6��������1��Ϊÿ��ð���߼�1��������\r\n\t�漣����6������������123456. ��������ð���ߣ���������ð���߶��ָ�Ϊ����������";
            skills[7] = "����³����\r\n\t�����顪��ð������ÿ���ܵ�1�����ϵ��˺�ʱ������Ժ���1���˺���\r\n\tҰ����̬��������������û��ʹ�õ����ӵĵ�����������֮ǰͶ�Ĳ��ۼӣ�������4������ȡ����������ǵ�³������غ���ɵ��˺���";
            skills[8] = "������ʫ�ˡ�\r\n\t������ʫ��ÿ�غ�ֻ��ʹ�����漼���е�һ�֣�\r\n\t��С��������������Ͷ��󣬿��԰����е�һ����Ϊ��һ�����֡�\r\n\t���ҡ�����������Ķ���Ͷ����ʱ���������Ͷһ��ȫ�������ӡ�\r\n\t��ʾ�����ڹ��﹥��ǰ����ѡ��һ������Ȼ��Ͷһ�����ӣ������1��2�����������ɵ��˺���ת�Ƹ��κι������ϣ������˹����Լ����ϡ������3��4����ʾʧ�ܣ���ͨ��һ������˺��������5��6����ʾ����ʧ�ܣ��˺�����������ʫ�˳����˺���";
            skills[9] = "��������\r\n\tԶ�̹�������ÿ��ս��֮ǰ��Ͷ6�����ӣ�ÿ��1�����ӣ����2���˺���\r\n\t˫�ֹ�������ÿ��1�����ӣ����2���˺���\r\n\t���ܡ������������ð���߶��Ѿ����������������ڸ�һ�����е�����5��6�Ĺ���ս������ô�ڹ������ʱֻ��Ͷ��6����ܵ��˺���";
            skills[10] = "��ɮ�¡�\r\n\t��͸���������������غ��У�ÿ�غ϶�ǡ����5�����ӵĵ����͵���Ŀ��ĵ�ǰ������Ŀ����������������㳢��ʹ����͸���������غ����㲻��ʹ���������ܣ����������ˣ�����ʹ��͸��ʧ����Ҳ���ܡ�\r\n\t����������2�����Ӷ�ǡ�õ���ɮ�µĵ�ǰ������ɮ�¸�һ��Ŀ������൱��ɮ�µ�ǰ�������˺���\r\n\t�����塪��3�����Ӷ�ǡ�õ���ɮ�µĵ�ǰ��������غ���ð�������ܵ����κ��˺�����Ч��\r\n\t��Ԫ�塪��4�����Ӷ�ǡ�õ���ɮ�µĵ�ǰ������ɮ�»ָ���������";
            skills[11] = "����ħʦ��\r\n\t������衪������Ͷ����������֮ǰ�������Ͷһ�����ӣ���غ���������������ȷ����Ͷ�������ӵĴ�����������ͨ����3�Ρ��������6����������Ĺ����׶Σ�ֱ���ֵ����﹥����\r\n\tǿ��Ӣ�¡���ÿ���غ������Ͷ����������֮���������һ�����ӣ������1��2������ɵ��˺��ӱ��������5����6�����ܵ����˺��ӱ��������3����4����Ч����";
            skills[12] = "���̿͡�\r\n\t����һ������4��1��ֱ��ɱ��һ��Ŀ�ꡣ\r\n\t��ҩ�����������ĸ�ð����ʹ����Щ���ܣ�ÿ�غϿ��Զ�������1���˺���";
            skills[13] = "����ʦ��\r\n\t�������ߡ���3����ͬ��������غϣ�������Ͷ2�����ӡ�\r\n\t����������4����ͬ����ÿ��Ŀ���ܵ�1���˺���\r\n\t����������5����ͬ����6���˺������Է�������Ŀ�ꡣ\r\n\t��������6����ͬ����ֱ��ɱ���ⳡս���е����й��";
            skills[14] = "�����鷨ʦ��\r\n\t������ʬ����ð�����������鷨ʦ���ݣ�ÿ�غ�Ͷһ�����ӣ�����൱�����ӵ������˺����������ǣ����ܹ�����ʹ�ü��ܣ���Ҳ���ܱ����ơ�������鷨ʦ���ˣ������ǣ�������ʧ��\r\n\tͨ���������ð����������ѡ��ȥ���������ǣ�����ô���鷨ʦ���Ի�����ð���ߵ�������\r\n\t��ȡ��������ÿ�غ�����Ͷ������֮ǰ�������Ͷ4�����ӣ������ǳ����൱��2�����ӵ���֮�͵��˺���ð�����ǳ����൱����2�����ӵ���֮�͵��˺������鷨ʦ�Լ������ݵ���ʬ���ܵ�����˺���";
            skills[15] = "����ʿ��\r\n\t����������ʿ���ܵ����ӣ�����ȫ������ͬһ��Ͷ���Ľ����\r\n\t��ǽ����3������������Ķ��ִ˻غϲ�Ͷ���ӡ�\r\n\t��������4����������ÿ��Ŀ�������������ֵ�൱��Ŀ�굱ǰ������һ�루����ȡ����������Ŀ������Ϊ5������2��������\r\n\t����֮������5������������֮�����Ϸ�У�ð�������ܵ����˺�ͬʱҲ���������\r\n\t쫷硪��6���������������ⳡս���е����й��";
            skills[16] = "������ʦ��\r\n\t�ٻ��μ�������ѡ���2�ԣ���1122�����ٻ�һ���μ�������Ѽ����ⳡս��������һ��ս��ʱ��ʧ��\r\n\t�ٻ�������ѡ���3�ԣ���112233�����ٻ�һ��������ѣ��ɴ���Ϸ������\r\n\t�������Ǵμ�������ѻ���������ѣ�������ʦ����������ʧ�����ǲ����ܵ��˺�����ð�������ж�֮�󣬹��﹥��֮ǰ��Ϊ����Ͷ6�����ӣ�ÿ��5��6���1���˺������Է�������Ŀ�ꡣ����ʦ����ͬʱӵ��1�����ϵ����ѣ�";
        }

        private void ����س�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Application.StartupPath;
            saveDialog.Filter = "jpg files (*.jpg)|*.jpg";
            saveDialog.Title = "����س�";
            if (saveDialog.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                File.Delete(saveDialog.FileName);
                image.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            backImage = new Bitmap(image);
            start(new Bitmap(backImage));

            setStateLabel("����س� - " + Path.GetFileNameWithoutExtension(saveDialog.FileName));
            �����ð��ToolStripMenuItem.Checked = false;
            ʱ֮ɳToolStripMenuItem.Checked = false;
            �հ׳���ToolStripMenuItem.Checked = false;
        }

        private void ����س�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image tempImage;
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Application.StartupPath;
            openDialog.Filter = "jpg files (*.jpg)|*.jpg";
            openDialog.Title = "����س�";
            if (openDialog.ShowDialog() == DialogResult.Cancel)
                return;
            tempImage = Image.FromFile(openDialog.FileName);
            backImage = new Bitmap(tempImage);
            start(new Bitmap(backImage));
            setStateLabel("����س� - " + Path.GetFileNameWithoutExtension(openDialog.FileName));

            tempImage.Dispose();

            �����ð��ToolStripMenuItem.Checked = false;
            ʱ֮ɳToolStripMenuItem.Checked = false;
            �հ׳���ToolStripMenuItem.Checked = false;
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveFileName = string.Format("{0}\\[����]{1}.jpg", Application.StartupPath, getStateLabel().Remove(0, 7));
            try
            {
                File.Delete(saveFileName);
                image.Save(saveFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("������ȳɹ���", getStateLabel(), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string backImageFileName, loadImageFileName;
            Image tempImage, loadImage;

            loadImageFileName = string.Format("{0}\\[����]{1}.jpg", Application.StartupPath, getStateLabel().Remove(0, 7));
            if (File.Exists(loadImageFileName) == false)
            {
                MessageBox.Show(string.Format("�����ļ�({0})�����ڣ�\r\n\r\n�㻹û�д������سǵĽ��Ȱɣ�", loadImageFileName), "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (�����ð��ToolStripMenuItem.Checked == true)
                tempImage = Properties.Resources.DelveTheDiceGameStartingAdventure;
            else if (ʱ֮ɳToolStripMenuItem.Checked == true)
                tempImage = Properties.Resources.DelveTheDiceGameSandsOfTime;
            else if (�հ׳���ToolStripMenuItem.Checked == true)
                tempImage = Properties.Resources.DelveTheDiceGameSandsOfTime;
            else
            {
                backImageFileName = string.Format("{0}\\{1}.jpg", Application.StartupPath, getStateLabel().Remove(0, 7));
                if (File.Exists(backImageFileName) == false)
                {
                    MessageBox.Show(string.Format("�س��ļ�({0})�����ڣ�\r\n\r\n�뽫�س��ļ������������ͬһλ��!", backImageFileName), "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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