using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace intersectoralBalance
{
    public partial class Form1 : Form
    {
        List<TextBox> increasesProcentTextBox = new List<TextBox>();
        List<Label> industryLabels = new List<Label>();
        List<Label> procentLabels = new List<Label>();
        List<int> YPosTextBox = new List<int>();
        List<int> YPosIndustryLabels = new List<int>();
        List<int> YPosProcentLabels = new List<int>();
        int countOfIndustryVal;

        int[,] matrixX;
        double[,] matrixA;
        double[,] matrixE;
        double[,] matrixESubA;
        double[,] reverseMatrixESubA;
        double[] endProdPlan;
        double[] endProd;
        double[] procent;
        int[] vectorVal;

        Label taskLb = new Label()
        {
            Text = "Найти валовый продукт при указанном увелечении конечного продукта",
            Width = 375,
            Height = 15
        };
        int taskLbYPos;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            increasesProcentTextBox.Clear();
            industryLabels.Clear();
            procentLabels.Clear();

            YPosTextBox.Clear();
            YPosIndustryLabels.Clear();
            YPosProcentLabels.Clear();

            taskLbYPos = 0;
            panel1.Controls.Clear();

            countOfIndustryVal = (int)countOfIndustry.Value;

            for (int i = 0; i < countOfIndustryVal; i++) // генерируем задачу
            {
                Label lb = new Label()
                {
                    Text = "Отрасли " + (i + 1) + " на",
                    Location = new Point(10, 10 + 30*i),
                    Height = 15,
                    Width = 70
                };

                Label lb2 = new Label()
                {
                    Text = "%",
                    Width = 20,
                    Height = 15,
                    Location = new Point(125, 10 + 30 * i),

                };
                if (i < countOfIndustry.Value - 1)
                    lb2.Text += ",";

                TextBox textBox = new TextBox()
                {
                    Width = 30, 
                    Height = 20,
                    Location = new Point(85, 10 + 30 * i)
            };

                if (i == 0)
                {
                    lb.Text = "Конечный продут Отрасли 1 необходимо увелисеть на";
                    lb.Width = 285;
                    textBox.Location = new Point(300, 10 + 30 * i);
                    lb2.Location = new Point(340, 10 + 30 * i);
                } 

                increasesProcentTextBox.Add(textBox);
                industryLabels.Add(lb);
                procentLabels.Add(lb2);

                YPosTextBox.Add(increasesProcentTextBox[i].Location.Y);
                YPosIndustryLabels.Add(industryLabels[i].Location.Y);
                YPosProcentLabels.Add(procentLabels[i].Location.Y);

                panel1.Controls.Add(increasesProcentTextBox[i]);
                panel1.Controls.Add(industryLabels[i]);
                panel1.Controls.Add(procentLabels[i]);
            }


            taskLb.Location = new Point(10, 10 + 30 * countOfIndustryVal);
            taskLbYPos = 10 + 30 * countOfIndustryVal;
            panel1.Controls.Add(taskLb);

            if (taskLbYPos - 100 < 0)
                vScrollBar1.Maximum = 0;
            else
                vScrollBar1.Maximum = taskLbYPos - 100;



            // генерируем матрицу прямых затрат
            materialCosts.Columns.Clear();
            materialCosts.Columns.Add(new DataGridViewTextBoxColumn() {
                Name = "fstColumn",
                HeaderText = "Производящие отрасли",
                ReadOnly = true,
                Width = 100
            });
            for (int i = 0; i < countOfIndustryVal; i++)
            {
                materialCosts.Columns.Add(new DataGridViewTextBoxColumn() {
                    Name = "Otr" + (i + 1),
                    HeaderText = "Отрасль" + (i + 1),
                    Width = 75
                });
                materialCosts.Rows.Add("Отрасль" + (i + 1));
            }



            // генерируем вектор
            vectorGrossProduct.Rows.Clear();
            for (int i = 0; i < countOfIndustryVal; i++)
                vectorGrossProduct.Rows.Add((i+1).ToString());


            // добавляем строки в таблицу ответов
            resultTable.Rows.Add(countOfIndustryVal);
            resultTable.ReadOnly = true;

        }
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

            //label2.Location = new Point(23, 20-vScrollBar1.Value);
            for (int i = 0; i < increasesProcentTextBox.Count; i++)
            {
                increasesProcentTextBox[i].Location = new Point(
                    increasesProcentTextBox[i].Location.X,
                    YPosTextBox[i] - vScrollBar1.Value);
                industryLabels[i].Location = new Point(
                    industryLabels[i].Location.X,
                    YPosIndustryLabels[i] - vScrollBar1.Value);
                procentLabels[i].Location = new Point(
                    procentLabels[i].Location.X,
                    YPosProcentLabels[i] - vScrollBar1.Value);
            }
            taskLb.Location = new Point(taskLb.Location.X, taskLbYPos - vScrollBar1.Value);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            matrixX = new int[countOfIndustryVal, countOfIndustryVal];
            matrixA = new double[countOfIndustryVal, countOfIndustryVal];
            matrixE = new double[countOfIndustryVal, countOfIndustryVal];
            matrixESubA = new double[countOfIndustryVal, countOfIndustryVal];

            endProd = new double[countOfIndustryVal];
            procent = new double[countOfIndustryVal];
            vectorVal = new int[countOfIndustryVal];

            for (int i = 0; i < matrixE.GetLength(0); i++) // заполняем единичную матрицу E
                for (int j = 0; j < matrixE.GetLength(0); j++)
                    if (i == j)
                        matrixE[i, j] = 1;
                    else
                        matrixE[i, j] = 0;



            for (int i = 0; i < countOfIndustryVal; i++)
            {

                try
                {
                    vectorVal[i] = Int32.Parse(vectorGrossProduct.Rows[i].Cells[1].Value.ToString());
                } catch
                {
                    MessageBox.Show("Неправильный формат данных введен в вектор валового продукта");

                }

                int sumCosts = 0;
                for (int j = 0; j < countOfIndustryVal; j++)
                {
                    try
                    {
                        matrixX[i,j] = Int32.Parse(materialCosts.Rows[i].Cells[j + 1].Value.ToString());
                        sumCosts += Int32.Parse(materialCosts.Rows[i].Cells[j + 1].Value.ToString());
                    } catch
                    {
                        MessageBox.Show("Неправильный формат данных введен в матрицу затрат");
                        return;
                    }
                }

                double h = Int32.Parse(vectorGrossProduct.Rows[i].Cells[1].Value.ToString()) // валовый продукт 
                - sumCosts;                                                       // - Сумма расходов по строке


                resultTable.Rows[i].Cells[0].Value = h;
                endProd[i] = h;
                
                }

            for (int i = 0; i < countOfIndustryVal; i++) // заполняем матрицу А зачениями и матрицу E-A

                for (int j = 0; j < countOfIndustryVal; j++)
                {
                    matrixA[i, j] = (double)matrixX[i, j] / (double)vectorVal[j];

                    matrixESubA[i, j] = matrixE[i, j] - matrixA[i, j];
                }


            for (int i = 0; i < countOfIndustryVal; i++)
                try
                {
                    procent[i] = (double.Parse(increasesProcentTextBox[i].Text)+100) / 100;
                } catch
                {
                    MessageBox.Show("Неправильный формат ввода процентов в задаче");
                }


            try
            {
                reverseMatrixESubA = Matrix.GetReversMatrix(matrixESubA);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            endProdPlan = new double[countOfIndustryVal];
            for (int i = 0; i < countOfIndustryVal; i++)
            {
                endProdPlan[i] = endProd[i] * procent[i];
                resultTable.Rows[i].Cells[2].Value = endProdPlan[i];
            }



            for (int i = 0; i < countOfIndustryVal; i++)
            {
                double sum = 0;
                for (int j = 0; j < countOfIndustryVal; j++)
                {
                    sum += reverseMatrixESubA[i,j] * endProdPlan[j];

                }
                resultTable.Rows[i].Cells[1].Value = sum;
            }

        }
    }
}
