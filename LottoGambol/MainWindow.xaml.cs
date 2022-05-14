using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private int _threadAmount = 1;

        private Random _random = new();
        private bool _stillRunningThread = false;

        private string _output = "";
        public bool[] Running { get; }

        private LoadingPopup _popup;

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
            _popup = new LoadingPopup();
            Running = new bool[_threadAmount + 1];
            InitializeComponent();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _threadAmount; i++)
                Running[i] = false;
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


            /* NOT WORKING YET
            int tAmount = amount % _threadAmount;
            amount -= tAmount;
            amount /= _threadAmount;

            for (int i = 0; i < _threadAmount; i++)
            {
                Thread gen = new Thread(() => { Output = GenNumbas(min, max, amount, i); });
                gen.IsBackground = true;
                gen.Start();
            }

            Thread gen2 = new Thread(() => { Output += GenNumbas(min, max, tAmount, _threadAmount); });
            gen2.IsBackground = true;
            gen2.Start();
            */

            Thread gen2 = new Thread(() => { Output += GenNumbas(min, max, amount, _threadAmount); });
            gen2.IsBackground = true;
            gen2.Start();

            Dispatcher.Invoke(() =>
            {
                _popup.Show();
                _popup.Topmost = true;
                _popup.WindowState = WindowState.Maximized;
                _popup.WindowStyle = WindowStyle.None;
            });
        }

        private void Copy_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Res.Text);
        }

        private string GenNumbas(int min, int max, int amount, int tNum)
        {
            Running[tNum] = true;
            string s = "";
            for (int i = amount; i > 0; i--)
            {
                if (Running[tNum])
                {
                    int r = _random.Next(min, max + 1);
                    s += r + ",";
                    Dispatcher.Invoke(() =>
                    {
                        _popup.LogBlock.Text += r + "\n";
                    });
                }
                else
                {
                    Running[tNum] = false;
                    HidePopup();
                    return "";
                }
            }

            //remove last comma
            //s = s.Substring(0, s.Length - 2);
            Running[tNum] = false;
            HidePopup();
            return s;
        }

        private void HidePopup()
        {
            if (!Running.ToList().Contains(true))
            {
                _stillRunningThread = false;
                Dispatcher.Invoke(() => { _popup.Hide(); });
            }
        }

        private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            Dispatcher.Invoke(() => { _popup.Close(); });
        }
    }
}