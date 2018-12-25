using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Zapasi
{
    public partial class Form1 : Form
    {
        struct data
        {
            public string name;
            public int ed;
            public double cc;
            public double v;
        }

        string projDir;

        Solver solver;
        int T = 36;
        int N = 20;
        double F = 3000;
        int[] ED;
        double[] CC;
        double[] V;
        string[] names;
        int f = 3;
        int[,] Kr;
        int[,] C;
        DataGridView[] stratDGVs;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SolvePrimer();
                OutResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        bool LoadPrimer()
        {
            if (!double.TryParse(tbF.Text, out F))
            {
                MessageBox.Show("Введите F");
                return false;
            }

            if (!int.TryParse(tbT.Text, out T))
            {
                MessageBox.Show("Введите T");
                return false;
            }
            return true;
        }

        bool LoadData()
        {
            OpenFileDialog ofd1 = new OpenFileDialog
            {
                Title = "Выберите файл данных",
                InitialDirectory = projDir,
                Filter = "CSV файлы | *.csv"
            };
            if (ofd1.ShowDialog() != DialogResult.OK)
                return false;

            StreamReader sr = new StreamReader(ofd1.FileName);

            sr.ReadLine();//пропуск заголовка
            DataTable dt = new DataTable();
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                dt.Columns.Add(dataGridView1.Columns[i].HeaderText);

            List<data> datal = new List<data>();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                var strs = line.Split(';');
                data d;
                d.name = strs[0];
                d.ed = int.Parse(strs[1]);
                d.cc = double.Parse(strs[2]);
                d.v = double.Parse(strs[3]);
                datal.Add(d);
                dt.Rows.Add(d.name, d.ed, d.cc, d.v);
            }

            N = datal.Count;
            names = new string[N];
            ED = new int[N];
            CC = new double[N];
            V = new double[N];

            for (int i = 0; i < N; i++)
            {
                names[i] = datal[i].name;
                ED[i] = datal[i].ed;
                CC[i] = datal[i].cc;
                V[i] = datal[i].v;
            }

            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = dt;
            return true;
        }

        bool LoadStrat()
        {
            OpenFileDialog ofd2 = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите файлы стратегий",
                InitialDirectory = projDir,
                Filter = "CSV файлы | *.csv"
            };
            if (ofd2.ShowDialog() != DialogResult.OK)
                return false;

            f = ofd2.FileNames.Length;
            Kr = new int[N, f];
            C = new int[N, f];
            for (int i = 0; i < f; i++)
            {
                StreamReader sr = new StreamReader(ofd2.FileNames[i]);

                sr.ReadLine();//заголовок

                for (int j = 0; j < N; j++)
                {
                    var strs = sr.ReadLine().Split(';');
                    Kr[j, i] = int.Parse(strs[1]);
                    C[j, i] = int.Parse(strs[2]);
                }
            }

            if (stratDGVs.Length == f)
            {
                for (int i = 0; i < f; i++)
                {
                    DataTable dt = new DataTable();
                    for (int j = 0; j < stratDGVs[i].ColumnCount; j++)
                        dt.Columns.Add(stratDGVs[i].Columns[j].HeaderText);

                    for (int j = 0; j < N; j++)
                    {
                        dt.Rows.Add(names[j], Kr[j, i], C[j, i]);
                    }
                    stratDGVs[i].Columns.Clear();
                    stratDGVs[i].DataSource = dt;
                }

            }

            return true;
        }

        void SolvePrimer()
        {
            int iters;
            if(!int.TryParse(textBox1.Text, out iters))
            {
                MessageBox.Show("Введите кол-во итераций");
                return;
            }
            solver = new Solver(iters, N, T, f, F, Kr, C, V, CC, ED);
            best_strat = solver.Calculate();
        }
        int best_strat = -1;

        void OutResults()
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < dataGridView2.ColumnCount; i++)
                dt.Columns.Add(dataGridView2.Columns[i].HeaderText);

            for (int i = 0; i < f; i++)
            {
                dt.Rows.Add("стратегия" + (i + 1), solver.cumTO[i], solver.cumTC[i], solver.cumTCOST[i]);
            }

            dataGridView2.Columns.Clear();

            dataGridView2.DataSource = dt;
            dataGridView2.Rows[best_strat].DefaultCellStyle.Font =
                new Font("Arial", 12, FontStyle.Bold);

        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LoadPrimer())
                    return;
                if (!LoadData())
                    return;
                if (!LoadStrat())
                    return;
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stratDGVs = new DataGridView[] {
                    dataGridView3,
                    dataGridView4,
                    dataGridView5,
                    dataGridView6,
                    dataGridView7,
        };

            var exedit = Directory.GetCurrentDirectory();
            var tmp = new DirectoryInfo(exedit);

            projDir = tmp.Parent.Parent.Parent.FullName;

        }
    }
}
