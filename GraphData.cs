using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.CodeDom;
using System.Drawing;
using static System.Windows.Forms.AxHost;
using System.IO;
namespace Lab3
{   //Найти цикл, проходящий не более чем через две вершины центра графа
    static class GraphData
    {
        public static int[,] graph;

        public static List<List<int>> cycles;
 
        public static List<int> center;


        public delegate void ReRenderMessageHandler();
        public static event ReRenderMessageHandler MessageSent;

        public static void SendReRenderingMessage()
        {
            MessageSent?.Invoke();
        }

        static GraphData()
        {
           cycles = new List<List<int>>();
        }

        public static void GraphInit(int m)
        {
            graph = new int[m,m];
            
        }
        public static void FindAllCycles()
        {
            int m = graph.GetLength(0);
            FindCenterWithFloydWarshall(m);
            for (int i = 0; i < m-1; i++)
            {
                for (int j = i+1; j < m; j++)
                {
                    FindСycles(i,j);


                }


            }
           

        }
        public static void FindCenterWithFloydWarshall(int m)
        {
            int n = m;
            int[,] dist = new int[n, n];
            int[] eccentricities = new int[n];
            int minEccentricity = int.MaxValue;

            // Инициализация и первоначальное заполнение матрицы расстояний
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        dist[i, j] = 0;
                    else if (graph[i, j] == 0)
                        dist[i, j] = int.MaxValue; // Предполагаем, что 0 означает отсутствие пути
                    else
                        dist[i, j] = graph[i, j];
                }
                eccentricities[i] = int.MaxValue; // Инициализация эксцентриситетов
            }

            // Выполнение алгоритма Флойда-Уоршелла
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[i, k] != int.MaxValue && dist[k, j] != int.MaxValue && dist[i, j] > dist[i, k] + dist[k, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                        }
                    }
                }
            }

            // Расчет эксцентриситета для каждой вершины и нахождение минимального эксцентриситета
            for (int i = 0; i < n; i++)
            {
                int maxDist = 0;
                for (int j = 0; j < n; j++)
                {
                    if (dist[i, j] > maxDist && dist[i, j] != int.MaxValue)
                        maxDist = dist[i, j];
                }
                eccentricities[i] = maxDist;
                if (maxDist < minEccentricity)
                    minEccentricity = maxDist;
            }

            // Идентификация центра графа как вершин с минимальным эксцентриситетом
            center = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if (eccentricities[i] == minEccentricity)
                    center.Add(i);
            }
        }

        public static void FindСycles(int startV, int endV) 
        {   
            var startCycle = new List<int>() { startV };
            Stack<List<int>> allCycles = new Stack<List<int>>();
            allCycles.Push(startCycle);

            while (allCycles.Count > 0)
            {
                var currCycle = allCycles.Pop();
                var adjacentVertices = AdjacentVertices(currCycle);
                if (adjacentVertices.Count == 0)
                    continue;
                else
                {
                    foreach (var item in adjacentVertices)
                    {
                        if (item == endV)
                        {
                            var copyCurrCycle = new List<int>(currCycle);
                            copyCurrCycle.Add(item);
                            if (CheckCycle(copyCurrCycle) && graph[startV,endV] > 0 && copyCurrCycle.Count > 2)
                            {
                                copyCurrCycle.Add(startV);
                                if (!ContainsCyclicPermutation(cycles, copyCurrCycle))
                                {
                                    cycles.Add(copyCurrCycle);
                                    SendReRenderingMessage();
                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        else
                        {
                            var copyCurrCycle = new List<int>(currCycle.ToArray());
                            copyCurrCycle.Add(item);
                            allCycles.Push(copyCurrCycle); 
                        }
                    }
                }
            }
        }

        public static List<int> AdjacentVertices(List<int> arr)
        {
            var list = new List<int>();
            for (int i = 0; i < graph.GetLength(0); i++)
                if (!arr.Contains(i) && graph[arr[arr.Count-1], i] == 1)
                    list.Add(i);
            return list;
        }

        public static bool ContainsCyclicPermutation(List<List<int>> listOfCycles, List<int> targetCycle)
        {
            // Убедиться, что длины списков, за исключением последних элементов, совпадают
            foreach (var list in listOfCycles)
            {
                if (list.Count - 1 != targetCycle.Count - 1)
                    continue;

                // Создать копии списков без последнего элемента
                var trimmedList = list.Take(list.Count - 1).ToList();
                var trimmedTarget = targetCycle.Take(targetCycle.Count - 1).ToList();

                // Сортировка обоих списков
                trimmedList.Sort();
                trimmedTarget.Sort();

                // Проверка равенства отсортированных списков
                if (Enumerable.SequenceEqual(trimmedList, trimmedTarget))
                    return true;
            }

            return false;
        }

        public static void RefreshAll()
        {
            cycles.Clear();
            graph = null;

        }
        

        public static void PaintGraph(Graphics g, int Height, int Width)
        {
            Pen pen = new Pen(Color.Black,3);
            g.Clear(Color.DarkGray);
            ConnectAllVertices(g, Height, Width, pen);

            pen.Color = Color.Red;

            if (cycles.Count > 0)
                PrintCycle(g, Height, Width, pen, cycles[cycles.Count - 1]);


            pen.Color = Color.LimeGreen;

            pen.Color = Color.Black;
            DrawVertices(g, Height, Width);    
        }

        public static void DrawVertices(Graphics g, int Height, int Width)
        {
            int cx = Width / 2;
            int cy = Height / 2;
            int radius = 350;
            int n = GraphData.graph.GetLength(0);
            double angle = 2 * Math.PI / n;
            int circleDiam = 30;

            Pen pen = new Pen(Color.Black, 3);
            Brush EllipseBrush = new SolidBrush(Color.LightGray);
            Brush StringBrush = new SolidBrush(Color.Black);
            StringFormat format = new StringFormat() { 
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center 
            };

            Font font = new Font("Calibri", 20, FontStyle.Regular);


            for (int i = 0; i < n; i++)
            {   if (center.Contains(i)) 
                    EllipseBrush = new SolidBrush(Color.LightBlue);
                else 
                    EllipseBrush = new SolidBrush(Color.LightGray);
                var x = (int)(cx + radius * Math.Cos(angle * i));
                var y = (int)(cy - radius * Math.Sin(angle * i));
                var rect = new Rectangle(x + circleDiam
                    , y - circleDiam
                    , circleDiam
                    , circleDiam);
                g.DrawEllipse(pen, rect);
                g.FillEllipse(EllipseBrush, rect);
                g.DrawString(i.ToString(), font, StringBrush, rect, format);
            }
        }

        public static void ConnectVertices(Graphics g, int Height, int Width, int first, int second, Pen pen)
        {
            int cx = Width / 2;
            int cy = Height / 2;
            int radius = 350;
            int n = GraphData.graph.GetLength(0);
            double angle = 2 * Math.PI / n;
            g.DrawLine(pen
                ,cx + (int)(radius * Math.Cos(first * angle)) + 45
                , cy - (int)(radius * Math.Sin(first * angle)) - 15
                , cx + (int)(radius * Math.Cos(second * angle)) + 45
                , cy - (int)(radius * Math.Sin(second * angle)) - 15);
        }

        public static void ConnectAllVertices(Graphics g, int Height, int Width, Pen pen)
        {
            int n = GraphData.graph.GetLength(0);
       
            for (int i = 0; i < n; i++)
                for (int j = i; j < n; j++)
                    if (graph[i, j] == 1)
                        ConnectVertices(g, Height, Width, i, j, pen);
        }

        public static bool CheckCycle(List<int> cycle)
        {
            int centerCount = 0;
            foreach (int vertex in center)
            {
                if (cycle.Contains(vertex))
                {
                    centerCount++;
                }
                if (centerCount > 2)
                {
                    return false;
                }
            }

            return true;
        }


        public static void PrintCycle(Graphics g, int Height, int Width, Pen pen, List<int> cycle)
        {
            for (int i = 0; i < cycle.Count-1; i++)
                ConnectVertices(g, Height, Width, cycle[i], cycle[i+1],pen);
        }
    }
}
