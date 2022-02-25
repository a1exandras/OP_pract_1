using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        Stopwatch st = new Stopwatch();

        static string codeword = "gachimuchi";
        static int attempts = 0;

        static int[] delayArr = new int[codeword.Length];
        static double[] mathSr = new double[codeword.Length];
        static double[] dispers = new double[codeword.Length];
        static double[] studKoef = new double[codeword.Length];

        public Window2()
        {
            InitializeComponent();
            codeWordText.Text = codeword;
            allCounter.Text = codeword.Length.ToString();

            inputBox.IsReadOnly = true;

            File.Delete(@"C:\temp_op\sem2prac1test1.txt");
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow nw = new MainWindow();
            Hide();
            nw.Show();
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (attempts == 0)
            {
                inputBox.IsReadOnly = true;
                inputBox.Text = "";
                return;
            }

            int len = inputBox.Text.Length;
            //inputCounter.Text = inputBox.Text;

            if(e.Key == Key.Return)
            {
                if (len == codeword.Length)
                {
                    inputBox.IsReadOnly = true;
                    Thread.Sleep(2000);
                    inputBox.IsReadOnly = false;

                    attempts--;

                    inputBox.Text = "";

                    mathThing();
                    addToFile(@"C:\temp_op\sem2prac1test1.txt");
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
                inputCounter.Text = (len + 1).ToString();

                st.Restart();  
            }
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

        private void numberOfTries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selItem = (TextBlock)numberOfTries.SelectedItem;
            attempts = Int32.Parse(selItem.Text.ToString());
            inputBox.IsReadOnly = false;
        }
    }
}
