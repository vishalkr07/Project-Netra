using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    public partial class Calculator : Window
    {
        public string value = "0";
        public string op = "+";
        public string mem = "";
        public string temp;

        public Calculator()
        {
            InitializeComponent();
            //Speak_Listen.Initialize(this);
            Debug.WriteLine("Calculator Opened");
        }

        public void OnLoad(object src, EventArgs e)
        {
            //Speak_Listen.Listen();
            textBox1.Focus();
        }
        public void textadd()
        {
            textbox.Text += txtShow.Text;
            if (txtShow.Text == "" && textbox.Text != "")
            {
                Int32 l = Convert.ToInt32(textbox.Text.Length) - 1;
                textbox.Text = textbox.Text.Substring(0, l);
                textbox.Text += op;

            }
            else
            {
                string temp = textbox.Text;
                textbox.Text = "(";
                textbox.Text += temp;
                textbox.Text += ")";
                textbox.Text += op;
            }
        }
        public string func()
        {
            if (op == "+")
            {
                value = Convert.ToString(Convert.ToDecimal(value.ToString()) + ((txtShow.Text == "") ? 0 : Convert.ToDecimal(txtShow.Text)));
            }
            if (op == "-")
            {
                value = Convert.ToString(Convert.ToDecimal(value.ToString()) - ((txtShow.Text == "") ? 0 : Convert.ToDecimal(txtShow.Text)));
            }
            if (op == "*")
            {
                value = Convert.ToString(Convert.ToDecimal(value.ToString()) * ((txtShow.Text == "") ? 1 : Convert.ToDecimal(txtShow.Text)));
            }
            if (op == "/")
            {
                value = Convert.ToString(Convert.ToDecimal(value.ToString()) / ((txtShow.Text == "") ? 1 : Convert.ToDecimal(txtShow.Text)));
            }
            if (op == "%")
            {
                value = Convert.ToString(Convert.ToDecimal(value.ToString()) % ((txtShow.Text == "") ? (Convert.ToDecimal(value.ToString()) + 1) : Convert.ToDecimal(txtShow.Text)));
            }
            if (op == "^")
            {
                value = Convert.ToString(Math.Pow((double)(Convert.ToDecimal(value.ToString())), ((txtShow.Text == "") ? (double)1 : (double)(Convert.ToDecimal(txtShow.Text)))));
            }
            return value;
        }
        public void Instruct(string str)
        {
            Debug.WriteLine("hello");
            if (str == "one")
            {
                txtShow.Text += "1";
            }
            if (str == "two")
            {
                txtShow.Text += "2";
            }
            if (str == "three")
            {
                txtShow.Text += "3";
            }
            if (str == "four")
            {
                txtShow.Text += "4";
            }
            if (str == "five")
            {
                txtShow.Text += "5";
            }
            if (str == "six")
            {
                txtShow.Text += "6";
            }
            if (str == "seven")
            {
                txtShow.Text += "7";
            }
            if (str == "eight")
            {
                txtShow.Text += "8";
            }
            if (str == "nine")
            {
                txtShow.Text += "9";
            }
            if (str == "zero")
            {
                txtShow.Text += "0";
            }
            if (str == "point")
            {
                txtShow.Text += ".";
            }
            if (str == "plus")
            {
                if ((op == "/" || op == "%") && txtShow.Text != "" && Convert.ToDecimal(txtShow.Text) == 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    value = func();
                    op = "+";
                    textadd();
                    txtShow.Text = "";
                }
            }
            if (str == "minus")
            {
                if ((op == "/" || op == "%") && txtShow.Text != "" && Convert.ToDecimal(txtShow.Text) == 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    value = func();
                    op = "-";
                    textadd();
                    txtShow.Text = "";
                }
            }
            if (str == "product")
            {
                if ((op == "/" || op == "%") && txtShow.Text != "" && Convert.ToDecimal(txtShow.Text) == 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    value = func();
                    op = "*";
                    textadd();
                    txtShow.Text = "";
                }
            }
            if (str == "divide")
            {
                if ((op == "/" || op == "%") && txtShow.Text != "" && Convert.ToDecimal(txtShow.Text) == 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    value = func();
                    op = "/";
                    textadd();
                    txtShow.Text = "";
                }
            }
            if (str == "power")
            {
                if ((op == "/" || op == "%") && txtShow.Text != "" && Convert.ToDecimal(txtShow.Text) == 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    value = func();
                    op = "^";
                    textadd();
                    txtShow.Text = "";
                }
            }
            if (str == "mod")
            {
                if ((op == "/" || op == "%") && txtShow.Text != "" && Convert.ToDecimal(txtShow.Text) == 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    value = func();
                    op = "%";
                    textadd();
                    txtShow.Text = "";
                }
            }
            if (str == "log")
            {
                if (txtShow.Text == "")
                {
                    if (Convert.ToDecimal(value.ToString()) != 0)
                    {
                        txtShow.Text = Convert.ToString(Math.Log10((double)Convert.ToDecimal(value.ToString())));
                    }
                    else
                    {
                        //narrate to the user about the exception
                    }
                }
                else if (Convert.ToDecimal(txtShow.Text) <= 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    txtShow.Text = Convert.ToString(Math.Log10((double)Convert.ToDecimal(txtShow.Text)));
                }
            }
            if (str == "root")
            {
                if (txtShow.Text == "")
                {
                    if (Convert.ToDecimal(value.ToString()) >= 0)
                    {
                        txtShow.Text = Convert.ToString(Math.Sqrt((double)Convert.ToDecimal(value.ToString())));
                    }
                    else
                    {
                        //narrate to the user about the exception
                    }
                }
                else if (Convert.ToDecimal(txtShow.Text) < 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    txtShow.Text = Convert.ToString(Math.Sqrt((double)Convert.ToDecimal(txtShow.Text)));
                }
            }
            if (str == "expo")
            {
                if (txtShow.Text == "")
                {
                    txtShow.Text = Convert.ToString(Math.Exp((double)Convert.ToDecimal(value.ToString())));
                }
                else
                {
                    txtShow.Text = Convert.ToString(Math.Exp((double)Convert.ToDecimal(txtShow.Text)));
                }
            }
            if (str == "square")
            {
                if (txtShow.Text == "")
                {
                    txtShow.Text = Convert.ToString(Math.Pow((double)Convert.ToDecimal(value.ToString()), 2));
                }
                else
                {
                    txtShow.Text = Convert.ToString(Math.Pow((double)Convert.ToDecimal(txtShow.Text), 2));
                }
            }
            if (str == "sine")
            {
                if (txtShow.Text == "")
                {
                    txtShow.Text = Convert.ToString(Math.Sin(((double)Convert.ToDecimal(value.ToString())) * Math.PI / 180));
                }
                else
                {
                    txtShow.Text = Convert.ToString(Math.Sin(((double)Convert.ToDecimal(txtShow.Text)) * Math.PI / 180));
                }
            }
            if (str == "cos")
            {
                if (txtShow.Text == "")
                {
                    txtShow.Text = Convert.ToString(Math.Cos(((double)Convert.ToDecimal(value.ToString())) * Math.PI / 180));
                }
                else
                {
                    txtShow.Text = Convert.ToString(Math.Cos(((double)Convert.ToDecimal(txtShow.Text)) * Math.PI / 180));
                }
            }
            if (str == "tan")
            {
                if (txtShow.Text == "")
                {
                    txtShow.Text = Convert.ToString(Math.Tan(((double)Convert.ToDecimal(value.ToString())) * Math.PI / 180));
                }
                else
                {
                    txtShow.Text = Convert.ToString(Math.Tan(((double)Convert.ToDecimal(txtShow.Text)) * Math.PI / 180));
                }
            }
            if (str == "store")
            {
                mem = txtShow.Text;
            }
            if (str == "MemC")
            {
                mem = "";
            }
            if (str == "load")
            {
                if (mem != "")
                    txtShow.Text = mem;
            }
            if (str == "equal")
            {
                if ((op == "/" || op == "%") && txtShow.Text != "" && Convert.ToDecimal(txtShow.Text) == 0)
                {
                    //narrate to the user about the exception
                }
                else
                {
                    txtShow.Text = func();
                    textbox.Text = "";
                    value = "0";
                    op = "+";
                }
            }
            if (str == "clear")
            {
                textbox.Text = "";
                op = "+";
                txtShow.Text = "";
                value = "0";
            }
            if (str == "delete")
            {
                Int32 l = Convert.ToInt32(txtShow.Text.Length) - 1;
                if (l < 0)
                {
                    txtShow.Text = "";
                }
                else
                {
                    txtShow.Text = txtShow.Text.Substring(0, l);
                }
            }

        }
    }
}
