using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace delve
{
    public partial class PickAdventurers : Form
    {
        public PickAdventurers()
        {
            InitializeComponent();
        }
        public int[] Picks = new int[5];
        public bool Ok = false;

        private void button1_Click(object sender, EventArgs e)
        {
            int n = 0;
            foreach (Control c in groupBox2.Controls)
            {
                if (((CheckBox)c).Checked)
                    n++;
            }
            if (n != 2)
            {
                MessageBox.Show("选2个敏捷型冒险者！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (radioButton1.Checked)
                Picks[1] = 1;
            else if (radioButton2.Checked)
                Picks[1] = 2;
            else if (radioButton3.Checked)
                Picks[1] = 3;
            else
                Picks[1] = 4;

            foreach (Control c in groupBox2.Controls)
            {
                if (((CheckBox)c).Checked)
                {
                    if (Picks[2] == 0)
                        Picks[2] = int.Parse(c.Name.Replace("checkBox", "")) + 4;
                    else
                    {
                        Picks[3] = int.Parse(c.Name.Replace("checkBox", "")) + 4;
                        break;
                    }
                }
            }

            if (radioButton12.Checked)
                Picks[4] = 13;
            else if (radioButton11.Checked)
                Picks[4] = 14;
            else if (radioButton10.Checked)
                Picks[4] = 15;
            else
                Picks[4] = 16;

            Ok = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}