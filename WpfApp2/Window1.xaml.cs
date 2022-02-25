using System;
using System.IO;
using System.Linq;
using System.Threading;
using static System.Math;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        string path = @"C:\temp_op\sem2prac1test1.txt";

        Stopwatch st = new Stopwatch();
        Random rnd = new Random();

        static string codeword = "gachimuchi";

        static int[] delayArr = new int[codeword.Length];
        static double[] mathSr = new double[codeword.Length];
        static double[] dispers = new double[codeword.Length];
        static double[] studKoef = new double[codeword.Length];

        public Window1()
        {
            InitializeComponent();
            codeWordText.Text += codeword;
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            int len = inputBox.Text.Length;

            if (e.Key == Key.Return)
            {
                if (len == codeword.Length)
                {
                    inputBox.IsReadOnly = true;

                    mathThing();

                    if (diagnoseResults())
                    {
                        addToFile(@"C:\temp_op\sem2prac1test1.txt");
                        inputBox.Text = "Success!";
                    }
                    else
                    {
                        inputBox.Text = "Access denied";
                    }
                    return;
                }
                else
                {
                    inputBox.Text = "";
                    return;
                }
            }

            if (len > 0)
            {
                if (!inputBox.Text[len - 1].Equals(codeword[len - 1]))
                {
                    inputBox.Text = "";
                    return;
                }

                delayArr[len - 1] = (int)st.Elapsed.TotalMilliseconds;
                //inputCounter.Text = st.Elapsed.TotalMilliseconds.ToString();

                st.Restart();
            }
        }

        private bool diagnoseResults()
        {
            bool res = false;

            double[] dataArr = new double[codeword.Length];

            using (StreamReader sr = new StreamReader(path))
            {
                int i = 0;
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    dataArr[i] = Convert.ToDouble(line.Split(' ')[1]);
                    i++;
                }

                int ja = rnd.Next(codeword.Length);

                double maxEl = Max(Pow(dataArr[ja], 2), Pow(dispers[ja], 2));
                double minEl = Min(Pow(dataArr[ja], 2), Pow(dispers[ja], 2));

                double fishKoef = maxEl / minEl;

                if (fishKoef > 2.5)
                    return true;
            }

            return res;
        }

        private void addToFile(string path)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    for (int i = 0; i < codeword.Length; i++)
                    {
                        sw.WriteLine(mathSr[i] + " " + dispers[i] + " " + studKoef[i]);
                    }
                }
            }
            else
            {
                double[] arr = new double[3];
                string[] dataArr = new string[codeword.Length];

                string tempFile = System.IO.Path.GetTempFileName();

                using (StreamReader sr = new StreamReader(path))
                {
                    int i = 0;
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        dataArr[i] = line;
                        i++;
                    }
                }

                for (int i = 0; i < codeword.Length; i++)
                {
                    string[] tempStr = dataArr[i].Split(' ');

                    arr[0] = (mathSr[i] + Convert.ToDouble(tempStr[0])) / 2;
                    arr[1] = (dispers[i] + Convert.ToDouble(tempStr[1])) / 2;
                    arr[2] = (studKoef[i] + Convert.ToDouble(tempStr[2])) / 2;

                    using (StreamWriter sw = File.AppendText(tempFile))
                        sw.WriteLine(arr[0] + " " + arr[1] + " " + arr[2]);
                }

                File.Delete(path);
                File.Move(tempFile, path);
            }
        }

        private void mathThing()
        {
            double summ = 0;
            int len = codeword.Length;

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    if (i != j)
                    {
                        summ += delayArr[j];
                    }
                }
                mathSr[i] = summ / len;
                summ = 0;
            }

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    if (i != j)
                    {
                        summ += Math.Pow(delayArr[j] - mathSr[j], 2);
                    }
                }
                dispers[i] = Math.Sqrt(summ / (len - 1));
                summ = 0;
            }

            for (int i = 0; i < len; i++)
            {
                studKoef[i] = Math.Abs(Math.Sqrt(len) * (delayArr[i] - mathSr[i]) / dispers[i]);
            }
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mv = new MainWindow();
            Hide();
            mv.Show();
        }
    }
}
