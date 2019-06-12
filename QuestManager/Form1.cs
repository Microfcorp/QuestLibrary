using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using QuestManager.QuestFiles;
using QuestManager.QuestFiles.Quest;

namespace QuestManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(string path)
        {
            InitializeComponent();

            var g = File.Open(path);

            if (g.ReadOnly)
            {
                включитьToolStripMenuItem.Checked = true;
                выключитьToolStripMenuItem.Checked = false;

                сохранитьToolStripMenuItem.Enabled = false;
                защитаОтЗаписиToolStripMenuItem.Enabled = false;
                MessageBox.Show("Внимание! Включена защита от записи данного файла. Редактирование невозможно");
            }
            else
            {
                включитьToolStripMenuItem.Checked = false;
                выключитьToolStripMenuItem.Checked = true;

                сохранитьToolStripMenuItem.Enabled = true;
                защитаОтЗаписиToolStripMenuItem.Enabled = true;
            }

            toolStripTextBox1.Text = g.Author;

            qm = QuestFiles.Quest.QuestManager.FromFile(g);
            TreeCreate();
            treeView1.CollapseAll();
        }

        QuestFiles.Quest.QuestManager qm = null;


        private TreeNode QuestToTree(Quest q)
        {
            return new TreeNode(q.Name);
        }

        List<string> added = new List<string>();

        private TreeNode[] QuestToTree(Quest[] q)
        {
            List<TreeNode> tn = new List<TreeNode>();
            foreach (var item in q)
            {
                var t = qm.GetPostChild(item);
                TreeNode tr = new TreeNode(item.Name, QuestToTree(t));
                tr.ToolTipText = item.ParentText;
                tn.Add(tr);
                comboBox1.Items.Add(item.Name);
                added.Add(item.Name);
            }
            return tn.ToArray();
        }

        private void TreeCreate()
        {            
            treeView1.Nodes.Clear();
            added.Clear();
            comboBox1.Items.Clear();
            foreach (var item in qm.Quests.ToArray())
            {
                if (!added.Contains(item.Name))
                {
                    var t = qm.GetPostChild(item);
                    TreeNode tr = new TreeNode(item.Name, QuestToTree(t));
                    tr.ToolTipText = item.ParentText;
                    added.Add(item.Name);
                    comboBox1.Items.Add(item.Name);
                    treeView1.Nodes.Add(tr);
                }
            }
            treeView1.ExpandAll();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //var f = new File("TT","EETTEE");
            //f.Save("f.bin");
            ///var f = File.Open("f.bin");
            //qm = QuestFiles.Quest.QuestManager.Create("Test");
               /* var g = File.Open("g.bin");
                qm = QuestFiles.Quest.QuestManager.FromFile(g);
                TreeCreate();*/
            treeView1.CollapseAll();
           /* Manager.AddQuest("Start", "Вы повернете налево или направо", new string[1], "");
            Manager.AddQuest("11", "Вы живы", new string[1] { "Start" }, "налево");
            Manager.AddQuest("21", "Вы тю", new string[1] { "11" }, "Прямо");
            Manager.AddQuest("22", "Вы ку", new string[1] { "11" }, "Назад");
            Manager.AddQuest("12", "Вы труп", new string[1] { "Start" }, "направо");
            Manager.ToFile().Save("g.bin");*/
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog();
            opg.Filter = "*.qbin|*.qbin";
            if (opg.ShowDialog() == DialogResult.OK)
            {
                var g = File.Open(opg.FileName);

                if (g.ReadOnly)
                {
                    включитьToolStripMenuItem.Checked = true;
                    выключитьToolStripMenuItem.Checked = false;

                    сохранитьToolStripMenuItem.Enabled = false;
                    защитаОтЗаписиToolStripMenuItem.Enabled = false;
                    MessageBox.Show("Внимание! Включена защита от записи данного файла. Редактирование невозможно");
                }
                else
                {
                    включитьToolStripMenuItem.Checked = false;
                    выключитьToolStripMenuItem.Checked = true;

                    сохранитьToolStripMenuItem.Enabled = true;
                    защитаОтЗаписиToolStripMenuItem.Enabled = true;
                }

                toolStripTextBox1.Text = g.Author;

                qm = QuestFiles.Quest.QuestManager.FromFile(g);
                TreeCreate();
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog opg = new SaveFileDialog();
            opg.Filter = "*.qbin|*.qbin";
            opg.FileName = qm.Name;
            if (opg.ShowDialog() == DialogResult.OK)
            {
                var sfile = qm.ToFile();

                var file = new File(sfile.Name, sfile.Quest, FormatVersion.Two, sfile.Checksym, File.BoolToByte(GetReadOnly), QuestLibrary.QuestFiles.Sign.Sign.GetSign, GetAuthor);

                file.Save(opg.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            qm.AddQuest(textBox1.Text, richTextBox1.Text, comboBox1.Text.Split(','), textBox2.Text);
            TreeCreate();
            panel1.Visible = false;
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
            textBox1.Text = "";
            comboBox1.Text = "";
            textBox2.Text = "";
            richTextBox1.Text = "";
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            
        }

        TreeNode activenode = null;

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
                activenode = e.Node;
                contextMenuStrip1.Show(treeView1, e.Location);
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            qm.RemoveQuest(qm.FromGetName(activenode.Text));
            TreeCreate();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            textBox1.Text = qm.FromGetName(activenode.Text).Name;
            comboBox1.Text = String.Join(",", qm.FromGetName(activenode.Text).ParentChild);
            textBox2.Text = qm.FromGetName(activenode.Text).ParentText;
            richTextBox1.Text = qm.FromGetName(activenode.Text).Text;
        }

        private void добавитьДочернийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            textBox1.Text = "";
            comboBox1.Text = qm.FromGetName(activenode.Text).Name;
            textBox2.Text = "";
            richTextBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void просмотретьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(qm.FromGetName(activenode.Text).Text);
        }

        private void свернутьВсёToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void развернутьВсёToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetText ST = new SetText();
            if (ST.ShowDialog() == DialogResult.OK)
            {
                qm = QuestFiles.Quest.QuestManager.Create(ST.Return);
                toolStripTextBox1.Text = Environment.UserName;
                qm.AddQuest("Start", "Start", new string[0], "");
                TreeCreate();
            }
            else
                MessageBox.Show("Ошибка");
        }

        private bool GetReadOnly
        {
            get
            {
                if (включитьToolStripMenuItem.Checked)
                    return true;
                else if (выключитьToolStripMenuItem.Checked)
                    return false;
                else
                    return false;
            }
        }

        private string GetAuthor
        {
            get
            {
                return toolStripTextBox1.Text;
            }
        }

        private void включитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            включитьToolStripMenuItem.Checked = true;
            выключитьToolStripMenuItem.Checked = false;
        }

        private void выключитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            включитьToolStripMenuItem.Checked = false;
            выключитьToolStripMenuItem.Checked = true;
        }
    }
}
