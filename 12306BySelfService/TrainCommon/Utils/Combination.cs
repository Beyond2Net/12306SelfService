using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon
{
    public class Combination
    {
        static int MONEY = 0;//购买金额
        static int N = 0;    //购买水果种类
        static int[] Weights;//存放水果价格
        static int[] Plan;   //存放组合结果
        static int total = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("请输入水果种类");
            N = int.Parse(Console.ReadLine());
            Weights = new int[N];
            Plan = new int[N + 1];
            Console.WriteLine(String.Format("请输入{0}种水果的价格并以，逗号分隔", N));
            string[] prices = Console.ReadLine().Split(',');
            if (prices.Length != N)
            {
                Console.WriteLine("水果种类与水果价格不匹配。");
                Console.ReadKey();
                return;
            }
            for (int i = 0; i < prices.Length; i++)
            {
                Weights[i] = int.Parse(prices[i]);
            }
            Array.Sort(Weights);//对数组进行排序(从小到大)

            Console.WriteLine("请输入购买金额");
            MONEY = int.Parse(Console.ReadLine());
            for (int i = 0; i < Weights.Length; i++)
            {
                Console.Write(" " + Weights[i] + " ");
            }
            Console.WriteLine();
            Stopwatch sw = new Stopwatch();
            sw.Start();//计时开始
            Search(0, 0);
            sw.Stop(); //计时结束
            Console.WriteLine(String.Format("总共 {0} 种组合", total));
            Console.WriteLine(String.Format("运行总共耗时 {0} 毫秒", sw.ElapsedMilliseconds));
            Console.ReadKey();
        }

        /// <summary>
        /// 使用递归 [徐坤] 2018-03-11 18:10
        /// </summary>
        /// <param name="n"></param>
        /// <param name="current_money"></param>
        private static void Search(int n, int current_money)
        {
            if (current_money == MONEY)
            {
                total++;
                PrintPlan(n);
                return;
            }
            if (n >= N)
            {
                return;
            }
            for (int i = 0; i * Weights[n] + current_money <= MONEY; i++)
            {
                Plan[n + 1] = i;//节点标记
                Search(n + 1, current_money + i * Weights[n]);//继续往下递归寻找下一个节点
            }
        }

        /// <summary>
        /// 输出符合条件的组合
        /// </summary>
        /// <param name="n"></param>
        private static void PrintPlan(int n)
        {
            if (n < N)
            {
                n = N;
            }
            for (int i = 1; i <= n; i++)
            {
                Console.Write(String.Format(" {0} ", Plan[i]));
            }
            Console.WriteLine();
        }
    }
}
