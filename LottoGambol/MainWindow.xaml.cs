using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LottoGambol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _threadAmount = 16;

        private Random _random = new();
        private bool _allThreadRunning = false;

        private string _output = "";
        private bool[] _running;

        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                Dispatcher.Invoke(() => { Res.Text = _output; });
            }
        }

        public MainWindow()
        {
            _running = new bool[_threadAmount + 1];
            InitializeComponent();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _threadAmount; i++)
                _running[i] = false;
            Output = "";
            int max, min, amount;

            #region Euous

            if (!Int32.TryParse(Max.Text, out max))
            {
                MessageBox.Show("Max numba is no int.", "Conversion Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Int32.TryParse(Min.Text, out min))
            {
                MessageBox.Show("Min numba is no int.", "Conversion Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Int32.TryParse(Amount.Text, out amount))
            {
                MessageBox.Show("Numbas Amount is no int.", "Conversion Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (max < min)
            {
                MessageBox.Show("Max cannot be smaller than Min", "Comparison Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (min > max)
            {
                MessageBox.Show("Min cannot be bigger than Max", "Comparison Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            #endregion

            int tAmount = amount % _threadAmount;
            amount -= tAmount;
            amount /= _threadAmount;

            for (int i = 0; i < _threadAmount; i++)
            {
                Thread gen = new Thread(() => { Output += GenNumbas(min, max, amount, i); });
                gen.IsBackground = true;
                gen.Start();
            }

            Thread gen2 = new Thread(() => { Output += GenNumbas(min, max, tAmount, _threadAmount); });
            gen2.IsBackground = true;
            gen2.Start();
        }

        private void Copy_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Res.Text);
        }

        private string GenNumbas(int min, int max, int amount, int tNum)
        {
            _running[tNum] = true;
            string s = "";
            for (int i = amount; i > 0; i--)
            {
                if (_running[tNum])
                    s += _random.Next(min, max + 1) + ",";
                else
                {
                    _running[tNum] = false;
                    return "";
                }
            }

            //remove last comma
            //s = s.Substring(0, s.Length - 2);
            _running[tNum] = false;
            return s;
        }
    }
}