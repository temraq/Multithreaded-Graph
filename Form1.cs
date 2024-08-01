using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//Найти цикл, проходящий не более чем через две вершины центра графа
namespace Lab3
{
    public partial class Form1 : Form
    {

        public Graphics g;
        public Form1()
        {
            InitializeComponent();
            GraphData.MessageSent += PaintGraph;
            g = CreateGraphics();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "G:\\Другие компьютеры\\Пекарня\\6 сем\\Введение в операционные системы\\Lab3";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            GraphData.RefreshAll();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = openFileDialog1.FileName;
                var data = File.ReadAllText(@path);
                var splitString = data.Split('\n');
                GraphData.GraphInit(splitString[0].Replace(" ","").Length - 1);
                int k = 0;

                foreach (var str in splitString)
                {
                    var intArray = str.Split(' ')
                        .Select(x => Convert.ToInt32(x))
                        .ToArray();
                    for (int j = 0; j < intArray.Length; j++)
                        GraphData.graph[k, j] = intArray[j];
                    k++;
                }


            }
            
            
        

        }


        private void PaintGraph()
        {
            Thread.BeginCriticalRegion();
            GraphData.PaintGraph(g, this.Height, this.Width);
            Thread.EndCriticalRegion();
        }

        private void ВводToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void найтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphData.FindAllCycles();
            g.Clear(Color.White);
            GraphData.ConnectAllVertices(g, Height, Width, new Pen(Color.Black, 2));
            GraphData.DrawVertices(g, Height, Width);
        }
    }
}
