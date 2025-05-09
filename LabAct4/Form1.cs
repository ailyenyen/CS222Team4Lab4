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
        String currentValue = "";
        String equation = "";
        bool isNewInput = true;
        bool hasResult = false;
        private const int MAX_INPUT_LENGTH = 15; // Maximum digits allowed

        public frmCalculator()
        {
            InitializeComponent();
            
            // If you want to set up the fonts in code rather than designer
            lblEquation.Font = new Font("Arial", 10F, FontStyle.Regular);
            lblEquation.ForeColor = Color.Gray;
            txtInput.Font = new Font("Arial", 16F, FontStyle.Bold);
        }

        private void button_click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string buttonText = button.Text;
            
            
            // Check if we've reached maximum length
            string currentTextWithoutCommas = txtInput.Text.Replace(",", "");
            if (currentTextWithoutCommas.Length >= MAX_INPUT_LENGTH && buttonText != ".")
            {
                return; // Don't add more digits
            }
            
            if (hasResult)
            {
                // Reset everything if we have a previous result
                txtInput.Text = "";
                lblEquation.Text = "";
                equation = "";
                hasResult = false;
                isNewInput = false;
            }

            // Handle decimal point separately
            if (buttonText == ".")
            {
                if (isNewInput)
                {
                    // Starting a new number with decimal
                    txtInput.Text = "0.";
                    isNewInput = false;
                }
                else if (string.IsNullOrEmpty(txtInput.Text))
                {
                    // Empty input - start with "0."
                    txtInput.Text = "0.";
                }
                else if (!txtInput.Text.Contains("."))
                {
                    // Add decimal if none exists
                    txtInput.Text += ".";
                }
                // If decimal exists, do nothing
            }
            else // Number button
            {
                if (isNewInput || txtInput.Text == "0")
                {
                    // Replace "0" or start new input
                    txtInput.Text = buttonText;
                    isNewInput = false;
                }
                else
                {
                    // Append the number
                    txtInput.Text += buttonText;
                }
            }

            currentValue = txtInput.Text;
        }

        private void operator_click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
    
            if (hasResult)
            {
                equation = txtInput.Text;
                hasResult = false;
            }
    
            if (!isNewInput)
            {
                equation += txtInput.Text;
            }
    
            // Handle different operator symbols
            string operatorSymbol = button.Text;
            equation += " " + operatorSymbol + " ";
    
            lblEquation.Text = equation;
            isNewInput = true;
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            // If we're entering a new number, clear just that part
            if (!isNewInput && !hasResult)
            {
                if (txtInput.Text.Length > 1)
                {
                    txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
                    currentValue = txtInput.Text;
                }
                else
                {
                    txtInput.Text = "0";
                    isNewInput = true;
                }
            }
            else if (equation.Length > 0)
            {
                // If we're showing the equation, remove the last term
                string[] parts = equation.Trim().Split(' ');
                if (parts.Length > 2)
                {
                    // Remove the last operator and prepare for new input
                    equation = string.Join(" ", parts.Take(parts.Length - 2)) + " ";
                    lblEquation.Text = equation;
                    txtInput.Text = "0";
                }
                else
                {
                    // Only one term left, clear everything
                    equation = "";
                    lblEquation.Text = "";
                    txtInput.Text = "0";
                    isNewInput = true;
                }
            }
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            // Clear everything
            txtInput.Text = "0";
            lblEquation.Text = "";
            currentValue = "";
            equation = "";
            isNewInput = true;
            hasResult = false;
        }

        private void btnEqual_Click(object sender, EventArgs e)
    {
    if (!isNewInput && !hasResult)
    {
        // Remove commas before adding to equation
        equation += txtInput.Text.Replace(",", "");
    }

    if (equation.Trim().Length > 0)
    {
        lblEquation.Text = equation;

        try
        {
            DataTable dt = new DataTable();
            // Replace all operator symbols with their computational equivalents
            string computableEquation = equation
                .Replace("×", "*")
                .Replace("÷", "/")
                .Replace(",", "") // Remove commas
                .Replace(" ", ""); // Remove spaces

            // Check for division by zero explicitly
            if (computableEquation.Contains("/0") && !computableEquation.Contains("/0."))
            {
                throw new DivideByZeroException();
            }

            var result = dt.Compute(computableEquation, "");

            // Handle infinity cases (like division by zero that wasn't caught above)
            if (result.ToString() == "∞" || result.ToString() == "-∞")
            {
                throw new DivideByZeroException();
            }

            // Format the result (keep your existing formatting code here)
            if (double.TryParse(result.ToString(), out double resultValue))
            {
                // Check if the result is an integer
                if (resultValue % 1 == 0)
                {
                    // Integer result - format with commas, no decimals
                    txtInput.Text = resultValue.ToString("N0");
                }
                else
                {
                    // Decimal result - show up to 8 decimal places, trimming trailing zeros
                    string decimalResult = resultValue.ToString("0.########");
            
                    // Add thousand separators to the integer part
                    string[] parts = decimalResult.Split('.');
                    if (long.TryParse(parts[0], out long integerPart))
                    {
                        parts[0] = integerPart.ToString("N0");
                        txtInput.Text = string.Join(".", parts);
                    }
                    else
                    {
                        txtInput.Text = decimalResult;
                    }
                }
            }
            else
            {
                txtInput.Text = result.ToString();
            }

            currentValue = txtInput.Text;
            hasResult = true;
        }
        catch (DivideByZeroException)
        {
            txtInput.Text = "Error";
            lblEquation.Text = "";
            equation = "";
            isNewInput = true;
            hasResult = false;
        }
        catch (Exception)
        {
            txtInput.Text = "Error";
            lblEquation.Text = "";
            equation = "";
            isNewInput = true;
            hasResult = false;
        }
    }
}
        
        // Format numbers with commas as they're typed (optional)
        private void FormatNumberWithCommas()
        {
            if (isNewInput || string.IsNullOrEmpty(txtInput.Text) || txtInput.Text.Contains("."))
            {
                return; // Skip formatting during decimal input
            }

            // Save cursor position
            int cursorPosition = txtInput.SelectionStart;
            int commaCountBefore = txtInput.Text.Count(c => c == ',');

            // Remove existing commas
            string text = txtInput.Text.Replace(",", "");

            // Try to parse and format
            if (long.TryParse(text, out long value))
            {
                // Format with thousand separators
                string formatted = value.ToString("N0");
        
                // Only update if the formatted version is different
                if (formatted != txtInput.Text)
                {
                    txtInput.Text = formatted;
            
                    // Adjust cursor position for added/removed commas
                    int commaCountAfter = txtInput.Text.Count(c => c == ',');
                    int commaDifference = commaCountAfter - commaCountBefore;
                    int newPosition = cursorPosition + commaDifference;
            
                    // Ensure cursor stays within bounds
                    txtInput.SelectionStart = Math.Max(0, Math.Min(newPosition, txtInput.Text.Length));
                }
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            // Only format when not dealing with decimals
            if (!txtInput.Text.Contains("."))
            {
                FormatNumberWithCommas();
            }
        }

        private void lblEquation_Click(object sender, EventArgs e)
        {
            // Add any functionality you want when the equation label is clicked
            // For example, maybe copy the equation to clipboard
            if (!string.IsNullOrEmpty(lblEquation.Text))
            {
                Clipboard.SetText(lblEquation.Text);
                MessageBox.Show("Equation copied to clipboard!");
            }
        }
    }
}