using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            GraphData.RefreshAll();
            var data = textBox1.Text;
            var splitString = data.Split('\n');
            GraphData.GraphInit(splitString[0].Replace(" ", "").Length - 1);
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

            Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
