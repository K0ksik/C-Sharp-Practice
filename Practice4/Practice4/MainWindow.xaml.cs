using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Globalization;

namespace Practice4
{
    public partial class MainWindow : Window
    {
        private StringBuilder bilder = new StringBuilder();
        private bool ResultDisplay = false;
        private bool Error = false;
        private bool PowerMode = false;
        private double PowerBase = 0;

        public MainWindow()
        {
            InitializeComponent();
            ResultTextBlock.Text = "0";
        }

        private void ClearAll()
        {
            bilder.Clear();
            ExpressionTextBlock.Text = "";
            ResultTextBlock.Text = "0";
            ResultDisplay = false;
            Error = false;
            PowerMode = false;
        }

        private void Clear()
        {
            if (ResultDisplay || Error)
            {
                ClearAll();
            }
            else
            {
                ResultTextBlock.Text = "0";
            }
        }

        private void UpdateSourceExDisplay()
        {
            ExpressionTextBlock.Text = bilder.ToString();
        }

        private void UpdateResultExp(string value)
        {
            ResultTextBlock.Text = value;
        }

        private void AddToExp(string value)
        {
            if (ResultDisplay || Error)
            {
                bilder.Clear();
                ResultDisplay = false;
                Error = false;
            }
            bilder.Append(value);
            UpdateSourceExDisplay();
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            var button = (Button)sender;
            string digit = button.Content.ToString();

            if (ResultDisplay || ResultTextBlock.Text == "0")
            {
                ResultTextBlock.Text = digit;
                ResultDisplay = false;
            }
            else
            {
                ResultTextBlock.Text += digit;
            }
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            var button = (Button)sender;
            string op = button.Content.ToString();

            if (PowerMode)
            {
                double exponent = double.Parse(ResultTextBlock.Text, CultureInfo.InvariantCulture);
                UpdateResultExp(Math.Pow(PowerBase, exponent).ToString(CultureInfo.InvariantCulture));
                PowerMode = false;
                ResultDisplay = true;
                UpdateSourceExDisplay();
                return;
            }

            if (ResultTextBlock.Text != "0" && !ResultDisplay)
            {
                AddToExp(ResultTextBlock.Text);
            }

            switch (op)
            {
                case "×": op = "*"; break;
                case "÷": op = "/"; break;
            }

            AddToExp(op);
            UpdateResultExp("0");
        }

        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            var button = (Button)sender;
            string parenthesis = button.Content.ToString();

            if (parenthesis == "(")
            {
                if (bilder.Length > 0 && (char.IsDigit(bilder[bilder.Length - 1]) || bilder[bilder.Length - 1] == ')'))
                {
                    AddToExp("*");
                }
                AddToExp("(");
            }
            else if (parenthesis == ")")
            {
                int openCount = bilder.ToString().Split('(').Length - 1;
                int closeCount = bilder.ToString().Split(')').Length - 1;

                if (openCount > closeCount)
                {
                    if (ResultTextBlock.Text != "0" && !ResultDisplay)
                    {
                        AddToExp(ResultTextBlock.Text);
                        UpdateResultExp("0");
                    }
                    AddToExp(")");
                }
            }
        }

        private void FunctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            var button = (Button)sender;
            string function = button.Content.ToString();

            try
            {
                double result = 0;
                double value = double.Parse(ResultTextBlock.Text, CultureInfo.InvariantCulture);

                switch (function)
                {
                    case "sin":
                        result = Math.Sin(value * Math.PI / 180);
                        break;
                    case "cos":
                        result = Math.Cos(value * Math.PI / 180);
                        break;
                    case "tg":
                        result = Math.Tan(value * Math.PI / 180);
                        break;
                    case "x²":
                        result = Math.Pow(value, 2);
                        break;
                    case "√x":
                        result = Math.Sqrt(value);
                        break;
                    case "1/x":
                        result = 1 / value;
                        break;
                    case "|x|":
                        result = Math.Abs(value);
                        break;
                    case "n!":
                        result = Factorial((int)value);
                        break;
                    case "xʸ":
                        PowerBase = value;
                        PowerMode = true;
                        AddToExp($"{value}^");
                        UpdateResultExp("0");
                        return;
                    case "10ˣ":
                        result = Math.Pow(10, value);
                        break;
                    case "log":
                        result = Math.Log10(value);
                        break;
                    case "ln":
                        result = Math.Log(value);
                        break;
                }

                UpdateResultExp(result.ToString(CultureInfo.InvariantCulture));
                ResultDisplay = true;
            }
            catch
            {
                UpdateResultExp("Ошибка");
                Error = true;
            }
        }

        private double Factorial(int n)
        {
            if (n < 0) throw new ArgumentException();
            if (n <= 1) return 1;
            return n * Factorial(n - 1);
        }

        private void ConstantButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            var button = (Button)sender;
            string constant = button.Content.ToString();
            string constantValue = constant == "π" ? Math.PI.ToString(CultureInfo.InvariantCulture)
                                                 : Math.E.ToString(CultureInfo.InvariantCulture);

            if (ResultDisplay)
            {
                bilder.Clear();
                UpdateResultExp(constantValue);
                ResultDisplay = false;
            }
            else if (ResultTextBlock.Text == "0")
            {
                UpdateResultExp(constantValue);
            }
            else
            {
                AddToExp(ResultTextBlock.Text + "*");
                UpdateResultExp(constantValue);
            }
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            if (!ResultTextBlock.Text.Contains("."))
            {
                if (ResultDisplay || ResultTextBlock.Text.Length == 0)
                {
                    UpdateResultExp("0.");
                }
                else
                {
                    UpdateResultExp(ResultTextBlock.Text + ".");
                }
                ResultDisplay = false;
            }
        }

        private void SignButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            if (ResultTextBlock.Text != "0")
            {
                try
                {
                    double value = double.Parse(ResultTextBlock.Text, CultureInfo.InvariantCulture);
                    value = -value;
                    UpdateResultExp(value.ToString(CultureInfo.InvariantCulture));
                }
                catch
                {
                    UpdateResultExp("Ошибка");
                    Error = true;
                }
            }
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            if (ResultDisplay)
            {
                Clear();
            }
            else if (ResultTextBlock.Text.Length > 0)
            {
                if (ResultTextBlock.Text.Length == 1 ||
                    (ResultTextBlock.Text.Length == 2 && ResultTextBlock.Text[0] == '-'))
                {
                    UpdateResultExp("0");
                }
                else
                {
                    UpdateResultExp(ResultTextBlock.Text.Substring(0, ResultTextBlock.Text.Length - 1));
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Error) return;

            try
            {
                if (PowerMode)
                {
                    double exponent = double.Parse(ResultTextBlock.Text, CultureInfo.InvariantCulture);
                    UpdateResultExp(Math.Pow(PowerBase, exponent).ToString(CultureInfo.InvariantCulture));
                    PowerMode = false;
                    ResultDisplay = true;
                    UpdateSourceExDisplay();
                    return;
                }

                if (!ResultDisplay && ResultTextBlock.Text != "0")
                {
                    AddToExp(ResultTextBlock.Text);
                }

                string expr = bilder.ToString()
                    .Replace("×", "*")
                    .Replace("÷", "/")
                    .Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture))
                    .Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

                int openCount = expr.Split('(').Length - 1;
                int closeCount = expr.Split(')').Length - 1;
                if (openCount > closeCount)
                {
                    expr += new string(')', openCount - closeCount);
                }

                var result = new DataTable().Compute(expr, null);

                UpdateResultExp(Convert.ToDouble(result).ToString(CultureInfo.InvariantCulture));
                bilder.Clear();
                ResultDisplay = true;
                UpdateSourceExDisplay();
            }
            catch
            {
                UpdateResultExp("Error");
                Error = true;
                bilder.Clear();
                UpdateSourceExDisplay();
            }
        }
    }
}