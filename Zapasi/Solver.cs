using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapasi
{
    class Solver
    {

        #region inputVars
        /// <summary>
        /// Кол-во раз моделирования
        /// </summary>
        int Iters;
        /// <summary>
        /// Кол-во видов товаров
        /// </summary>
        int N;
        /// <summary>
        /// Период времени
        /// </summary>
        int T;
        /// <summary>
        /// Кол-во стратегий
        /// </summary>
        int f;
        /// <summary>
        /// Критический уровень запаса[i, j] и-того товара для ж-той стратегии
        /// </summary>
        int[,] Kr;
        /// <summary>
        /// Предкритический уровень запаса [i, j] и-того товара для ж-той стратегии
        /// </summary>
        int[,] C;
        /// <summary>
        /// Фиксированны
        /// </summary>
        double F;

        /// <summary>
        /// затраты на заказ i-того товара
        /// </summary>
        double[] V;

        /// <summary>
        /// Ежедневные затраы на хранение еденицы итого товара
        /// </summary>
        double[] CC;
        /// <summary>
        /// Ожидаемый спрос на и товар
        /// </summary>
        int[] ED;
        #endregion


        #region resultVariables
        /// <summary>
        /// затраты на организацию поставки
        /// </summary>
        public Double[] TO;
        /// <summary>
        /// Затраты на хранение запасов
        /// </summary>
        public double[] TC;
        /// <summary>
        /// полные затраты
        /// </summary>
        public double[] TCOST;

        /// <summary>
        /// затраты на организацию поставки
        /// </summary>
        public Double[] cumTO;
        /// <summary>
        /// Затраты на хранение запасов
        /// </summary>
        public double[] cumTC;
        /// <summary>
        /// полные затраты
        /// </summary>
        public double[] cumTCOST;
        /// <summary>
        /// Лучшая стратегия Kr - крит запас и-того товара, C - предкритический 
        /// </summary>
        //public (int[] Krb, int[] Cb) strat;
        #endregion


        #region stateVariables
        /// <summary>
        /// уровень запаса [i, t] i-того товара в конце  t-той ед времени
        /// </summary>
        //int[,] IN;
        int[,] IN;
        /// <summary>
        /// Кол-во заказов на i-тый товар в течении времени T
        /// </summary>
        int[] NT;
        /// <summary>
        /// Общее число заказов
        /// </summary>
        int TN;
        #endregion


        #region randomVariables
        /// <summary>
        /// спрос[i, t] на i товар во время t
        /// </summary>
        // int[,] D;
        //  Dictionary<double, int>[] D;

        //List<double> T_list;
        #endregion

        #region otherVariables
        /// <summary>
        /// Объем заказа i-того товара
        /// </summary>
        int[] E;
        /// <summary>
        /// кол-во видов товара в одном заказе
        /// </summary>
        // int p;
        #endregion

        public Solver(
            int iters,
            int n,
            int t,
           int f,
            double F,

            int[,] kr,
            int[,] c,


            double[] v,
            double[] cc,
            int[] ed
            )
        {
            Iters = iters;
            N = n;
            T = t;
            this.f = f;
            this.F = F;

            Kr = kr ?? throw new ArgumentNullException(nameof(kr));
            C = c ?? throw new ArgumentNullException(nameof(c));

            CC = cc ?? throw new ArgumentNullException(nameof(cc));
            ED = ed ?? throw new ArgumentNullException(nameof(ed));
            V = v ?? throw new ArgumentNullException(nameof(v));

            GenerateInput();
        }


        void GenerateInput()
        {
            TN = 0;
            TO = new double[f];
            TC = new double[f];
            TCOST = new double[f];
            for (int i = 0; i < f; i++)
            {
                TO[i] = TC[i] = TCOST[i] = 0;
            }

            IN = new int[N, T + 1];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < T + 1; j++)
                    IN[i, j] = 0;

            NT = new int[N];
            for (int i = 0; i < N; i++)
                NT[i] = 0;
        }


        public int Calculate()
        {
            double min_cost = double.MaxValue;
            int best_str = -1;

            cumTC = new double[f];
            cumTCOST = new double[f];
            cumTO = new double[f];

            for (int i = 0; i < f; i++)
            {
                cumTO[i] = 0;
                cumTCOST[i] = 0;
                cumTC[i] = 0;
            }

            for (int i = 0; i < Iters; i++)
            {
                for (int strat = 0; strat < f; strat++)
                {
                    CalculateStrat(strat);
                    cumTO[strat] += TO[strat];
                    cumTC[strat] += TC[strat];
                    cumTCOST[strat] += TCOST[strat];
                }
            }

            for (int strat = 0; strat < f; strat++)
            {
                if (cumTCOST[strat] < min_cost)
                {
                    min_cost = cumTCOST[strat];
                    best_str = strat;
                }
            }

            return best_str;
        }

        /// <summary>
        /// Вычисления для одной стратегии
        /// </summary>
        /// <param name="strat">индекс стратегии</param>
        void CalculateStrat(int strat)
        {
            Random r = new Random();
            for (int t = 0; t <= T; t++)
            {
                int p = 1;
                for (int i = 0; i < N; i++)
                {
                    //генеация D
                    var D1 = r.Next(ED[i] - 5, ED[i] + 2);
                    if (t > 0)//если было до этого что-то
                    {
                        IN[i, t] = IN[i, t - 1] - D1;//пересчитали запасы

                    }
                }

                int crit_index = -1;
                //оценка запасов
                for (int i = 0; i < N; i++)
                {
                    if (IN[i, t] <= Kr[i, strat])//ищем крит
                    {
                        TN++;//заказ
                        TO[strat] += F;//организация
                        crit_index = i;
                        break;
                    }
                }
                if (crit_index != -1)//есть крит
                {
                    for (int i = 0; i < N; i++)//считаем p
                    {
                        if (i == crit_index)//пропуск критического
                            continue;
                        if (IN[i, t] <= C[i, strat])//если предкритический
                        {
                            p++;
                        }
                    }
                    for (int i = 0; i < N; i++)//считаем объем закупок и затраты
                    {
                        if (IN[i, t] <= C[i, strat])//если предкритический
                        {
                            int E = (int)Math.Sqrt((2 * ED[i] * (F / p) + V[i]) / CC[i]);

                            NT[i]++;
                            TO[strat] += V[i];
                            IN[i, t] += E;

                            if (IN[i, t] < 0)
                                throw new Exception($"IN[{i}, {t}]<0 (={IN[i, t]})");

                        }
                    }
                }

                for (int i = 0; i < N; i++)//затраты на хранение
                {
                    TC[strat] += CC[i] * IN[i, t];
                }
            }
            TCOST[strat] = TO[strat] + TC[strat];

        }

    }
}



