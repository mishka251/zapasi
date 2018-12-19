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
        public Form1()
        {
            InitializeComponent();
            stratDGVs = new DataGridView[] {


                    dataGridView3,
                    dataGridView4,
                    dataGridView5,
                    dataGridView6,
                    dataGridView7,
        };

        }
        Solver solver;
        private void button1_Click(object sender, EventArgs e)
        {
            SolvePrimer();
            OutResults();
        }
        struct data
        {
            public string name;
            public int ed;
            public double cc;
            public double v;
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
                MessageBox.Show("Введите F");
                return false;
            }
            return true;
        }

        bool LoadData()
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
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
            OpenFileDialog ofd2 = new OpenFileDialog();
            ofd2.Multiselect = true;

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

            if(stratDGVs.Length == f)
            {
                for(int i =0; i<f; i++)
                {
                    DataTable dt = new DataTable();
                    for (int j = 0; j < stratDGVs[i].ColumnCount; j++)
                        dt.Columns.Add(stratDGVs[i].Columns[j].HeaderText);

                    for(int j=0; j<N; j++)
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
            solver = new Solver(N, T, f, F, Kr, C, V, CC, ED);
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
                dt.Rows.Add("стратегия" + (i + 1), solver.TO[i], solver.TC[i], solver.TCOST[i]);
            }

            //   dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();

            dataGridView2.DataSource = dt;
            dataGridView2.Rows[best_strat].DefaultCellStyle.Font =
                new Font("Arial", 12, FontStyle.Bold);

        }
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


        private void button2_Click(object sender, EventArgs e)
        {
            if (!LoadPrimer())
                return;
            if (!LoadData())
                return;
            if (!LoadStrat())
                return;
            button1.Enabled = true;
        }







        //void Primer()
        //{

        //    int T = 36;
        //    int N = 20;
        //    int F = 3000;

        //    double[] ED =
        //    {
        //        2, 4, 1, 2,
        //        2, 6, 5, 9,
        //        1, 2, 7, 6,
        //        9, 9, 6, 4,
        //        9, 4, 1, 4
        //    };

        //    double[] CC =
        //    {
        //        6, 5, 6, 7,
        //        4, 8, 7, 9,
        //        12, 5, 4.3, 5.3,
        //        2, 4, 3, 5,
        //        6, 7, 5, 7
        //    };



        //    double[] V =
        //    {
        //        4400, 3200, 4500, 2000,
        //        1200, 3400, 123, 245,
        //        156, 236, 10344, 1100,
        //        1000, 200, 3456, 1200,
        //        250, 300, 400, 1600
        //    };

        //    int f = 3;

        //    int[,] Kr =
        //    {
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 }
        //    };
        //    int[,] C =
        //    {
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 },
        //        {4, 5, 7 }
        //    };



        //    Solver solver = new Solver(N, T, Kr, C, f, F, V, CC, ED);
        //    var best_ind = solver.Calculate();
        //    MessageBox.Show("Лучшее " + best_ind);

        //}

    }
}
