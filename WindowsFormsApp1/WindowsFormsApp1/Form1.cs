using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabActivity3
{
    public partial class frmCalculator : Form
    {
        String result = "";
        String operation = "";
        bool isPerformed = false;

        public frmCalculator()
        {
            InitializeComponent();
        }

        private void button_click(object sender, EventArgs e)
        {
            if ((txtInput.Text == "0") || (isPerformed))
            {
                txtInput.Clear();
            }
            isPerformed = false;
            Button button = (Button)sender;

            if (button.Text == ".")
            {
                if (!txtInput.Text.Contains("."))
                {
                    txtInput.Text += button.Text;
                }
            }
            else
            {
                txtInput.Text += button.Text;
            }
        }

        private void operator_click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (result != "")
            {
                btnEqual.PerformClick();
                operation = button.Text;
                isPerformed = true;
            }
            else
            {
                operation = button.Text;
                result = txtInput.Text;
                isPerformed = true;
            }
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            // Delete the last character from input
            if (txtInput.Text.Length > 1)
            {
                txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
            }
            else
            {
                txtInput.Text = "0";
            }
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            // Clear everything
            txtInput.Text = "0";
            result = "";
            operation = "";
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            double num1, num2, res;
            if (double.TryParse(result, out num1) && double.TryParse(txtInput.Text, out num2))
            {
                switch (operation)
                {
                    case "+":
                        res = num1 + num2;
                        txtInput.Text = res.ToString();
                        break;
                    case "-":
                        res = num1 - num2;
                        txtInput.Text = res.ToString();
                        break;
                    case "*":
                        res = num1 * num2;
                        txtInput.Text = res.ToString();
                        break;
                    case "/":
                        if (num2 != 0)
                        {
                            res = num1 / num2;
                            txtInput.Text = res.ToString();
                        }
                        else
                        {
                            txtInput.Text = "Error";
                        }
                        break;
                    default:
                        break;
                }
            }
            result = txtInput.Text;
        }
    }
}
