using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace enemy_export
{
    public partial class Form1 : Form
    {
        private string openDirectory;
        private RGSSReader rgssReader;

        private List<Enemy> enemies;

        public Form1()
        {
            enemies = null;
            rgssReader = new RGSSReader();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "读取动画";
            dialog.Filter = "RMXP数据文件(*.rxdata)|*.rxdata";

            dialog.AddExtension = true;
            dialog.DefaultExt = ".rxdata";

            if (openDirectory != null)
                dialog.InitialDirectory = openDirectory;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!"Enemies.rxdata".Equals(dialog.SafeFileName))
                {
                    MessageBox.Show("请选择一个Enemies.rxdata文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                // 检测是否合法
                if (!rgssReader.setPath(dialog.FileName)) return;
                rgssReader.init();
                
                if (rgssReader.GetEnemies() == null)
                {
                    MessageBox.Show("不是一个有效的Enemies.rxdata文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }

                openDirectory = Path.GetDirectoryName(dialog.FileName);
                label1.Text = dialog.FileName;
                button2.Enabled = true;
                button3.Enabled = true;

                enemies = rgssReader.GetEnemies();
            }

        }

        private string getOutputDirectory()
        {
            string directory = Directory.GetCurrentDirectory();
            if (Directory.Exists(Path.GetFullPath(directory + "\\..\\project")))
            {
                return Path.GetFullPath(directory + "\\..");
            }
            return directory;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string content = "// JS格式的怪物数据，请自行打开enemys.js文件并复制粘贴\n" +
                             "var enemys_fcae963b_31c9_42b4_b48c_bb48d09f3f80 = \n{\n";
            enemies.ForEach(enemy =>
            {
                content += "    "+enemy.toJS();
            });
            content += "}\n";
          
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出成JS格式";
            saveFileDialog.Filter = "JS文件(*.js)|*.js";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".js";
            saveFileDialog.InitialDirectory = getOutputDirectory();
            saveFileDialog.FileName = "怪物数据导出.js";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, content);
                    MessageBox.Show("已成功导出怪物数据至" + saveFileDialog.FileName, "导出成功", MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee);
                    MessageBox.Show("保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出成Excel格式";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.InitialDirectory = getOutputDirectory();
            saveFileDialog.FileName = "怪物数据导出.xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                label2.Text = "正在导出，请稍后... 这可能需要几十秒";
                try
                {
                    ExcelHelper excelHelper = new ExcelHelper();
                    excelHelper.addLine(new []
                    {
                        "编号", "名称", "MaxHP", "MaxSP", "力量", "灵巧", "速度", "魔力", "攻击力", "防御力",
                        "魔法防御", "回避修正", "特殊属性", "金币", "经验"
                    });
                    enemies.ForEach(enemy =>
                    {
                        excelHelper.addLine(new object[]
                        {
                            enemy.id, enemy.name, enemy.maxhp, enemy.maxsp, enemy.strength, enemy.dexterity, enemy.speed, 
                            enemy.magic, enemy.atk, enemy.def, enemy.mdef, enemy.dodge, enemy.special, enemy.money, enemy.experience
                        });
                    });
                    excelHelper.save(saveFileDialog.FileName);
                    MessageBox.Show("已成功导出怪物数据至" + saveFileDialog.FileName, "导出成功", MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee);
                    MessageBox.Show("保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                label2.Text = "点击可导出对应格式的怪物数据文件";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.Delete("Enemies.rxdata");
            File.Delete("System.rxdata");
        }
    }
}
