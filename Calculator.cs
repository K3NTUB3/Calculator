using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Calculator
{
    public partial class CalcMain : Form
    {
        FunctionClass operate = new FunctionClass();
        private bool operatorClicked = false;
        private bool equalsClicked = false;
        private bool zeroHandled = false;
        private bool taskPerformed = false;
        private double result;
        private double parsedValue;
        public CalcMain()
        {
            InitializeComponent();
            ViewHistory.Visible = false;
            txtValue.Text = "0";
            txtValue.Text = txtValue.Text;
            this.KeyPress += new KeyPressEventHandler(Calculator_KeyPressed);
            this.KeyDown += new KeyEventHandler(Calculator_KeyDown);
        }
        private void Calculator_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && e.Shift) // Check for Shift + Backspace
            {
                Button clearButton = btnClear;

                if (clearButton != null)
                {
                    clearButton.BackColor = Color.Firebrick;

                    Clear_Click(sender, e);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        clearButton.Invoke((Action)(() => clearButton.BackColor = Color.IndianRed));
                    });
                }

                e.SuppressKeyPress = true; // Suppress the key press event
            }

        }         //Combined keypress
        private void Calculator_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar)) // allows numbers from 0 to 9
            {
                string buttonName = "btn" + e.KeyChar;
                Button numberButton = this.Controls.Find(buttonName, true).FirstOrDefault() as Button;

                if (numberButton != null)
                {
                    numberButton.BackColor = Color.Silver;
                    NumberDecimalValue_Click(numberButton, e);
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        numberButton.Invoke((Action)(() => numberButton.BackColor = SystemColors.ButtonFace));
                    });
                }
            }
            else if (e.KeyChar == '+' || e.KeyChar == '-' || e.KeyChar == '/' || e.KeyChar == '*') // Allow basic operators
            {
                Button operatorButton = null;

                switch (e.KeyChar)
                {
                    case '+':
                        operatorButton = btnAdd;
                        break;
                    case '-':
                        operatorButton = btnSubtract;
                        break;
                    case '/':
                        operatorButton = btnDivide;
                        break;
                    case '*':
                        operatorButton = btnMultiply;
                        break;
                }

                if (operatorButton != null)
                {
                    operatorButton.BackColor = Color.Blue;

                    OperatorValue_Click(operatorButton, e);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        operatorButton.Invoke((Action)(() => operatorButton.BackColor = SystemColors.Highlight));
                    });
                }
            }
            else if (string.Equals(e.KeyChar, '.')) //allow decimal point (.)
            {
                Button decimalButton = btnDot;
                decimalButton.Text = ".";
                decimalButton.BackColor = Color.LightGray;

                NumberDecimalValue_Click(decimalButton, e); //call the existing decimal handler for input validation

                _ = Task.Run(async () =>
                {
                    await Task.Delay(100); // Short delay
                    decimalButton.Invoke((Action)(() => decimalButton.BackColor = SystemColors.Control));
                });
            }
            else if (string.Equals(e.KeyChar, '%'))
            {
                Button percentButton = btnPercent;

                if (percentButton != null)
                {
                    percentButton.BackColor = Color.Blue;

                    Percent_Click(percentButton.Text, e);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        percentButton.Invoke((Action)(() => percentButton.BackColor = SystemColors.Highlight));
                    });
                }
            }
            else if (string.Equals(e.KeyChar, '`')) //allow decimal point (.)
            {
                Button negateButton = btnNegate;

                if(negateButton != null)
                {
                    negateButton.BackColor = Color.LightGray;

                    NegateValue_Click(negateButton.Text, e);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        negateButton.Invoke((Action)(() => negateButton.BackColor = SystemColors.Control));
                    });
                }
            }
            else if (string.Equals(e.KeyChar, '=' )) //allow equal sign (=) or Enter key to perform calculation
            {
                Button equalsButton = btnEquals;

                if (equalsButton != null)
                {
                    equalsButton.BackColor = Color.Blue;

                    Equals_Click(equalsButton.Text, e);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        equalsButton.Invoke((Action)(() => equalsButton.BackColor = SystemColors.Highlight));
                    });
                }
            }
            else if (string.Equals(e.KeyChar, (char)Keys.Back)) //allow backspace to remove last character input
            {
                Button backSpaceButton = btnBackSpace;

                if (backSpaceButton != null)
                {
                    backSpaceButton.BackColor = Color.Firebrick;

                    BackSpace_Click(backSpaceButton.Text, e);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        backSpaceButton.Invoke((Action)(() => backSpaceButton.BackColor = Color.IndianRed));
                    });
                }
            }
            else if (char.ToUpperInvariant(e.KeyChar) == 'H') //allow decimal point (.)
            {
                Button historyButton = btnHistory;

                if (historyButton != null)
                {
                    historyButton.BackColor = SystemColors.GradientInactiveCaption;

                    History_Click(historyButton.Text, e);

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(100); // Short delay
                        historyButton.Invoke((Action)(() => historyButton.BackColor = Color.Transparent));
                    });
                }
            }
            else // Disallow any other characters
            {
                e.Handled = true;
            }
        } //Single keypress
        private void NumberDecimalValue_Click(object sender, EventArgs e)
        {
            Button numberdecimal = (Button)sender; //handle the numbers 0-9.
            if (operate.DivisionError(txtValue.Text) || equalsClicked)
            {
                ReturnToDefault();
            }
            if (operatorClicked) // checks if the operator is clicked
            {
                txtValue.Text = "";
                operatorClicked = false;
            }
            if (!zeroHandled && numberdecimal.Text != "0") //will replace the value of the text box if the initial value is 0.
            {
                txtValue.Text = "";
                zeroHandled = true;
            }
            if (string.IsNullOrEmpty(txtValue.Text) && string.Equals(numberdecimal.Text, ".")) //checks if the user press the decimal without inputting a number first.
            {
                txtValue.Text += "0" + numberdecimal.Text;
            }
            if (string.Equals(numberdecimal.Text, ".") && txtValue.Text.Contains(".")) //checks if the textbox already has decimal.
            {
                return;
            }
            if (string.Equals(numberdecimal.Text,"0")) //checks the 0 inputs so that it will not append uncontrollably.
            {
                if (txtValue.Text.Equals("0"))
                {
                    return;
                }
            }
            if (numberdecimal.Text == ".") // When appending a number or decimal
            {
                if (!txtValue.Text.EndsWith(".")) // If the last character is already a decimal, do nothing
                {
                    txtValue.Text += numberdecimal.Text;
                }
            }
            else
            {
                if (txtValue.Text.Contains(".")) // If the current value contains a decimal, append the number normally
                {
                    txtValue.Text += numberdecimal.Text;
                }
                else
                {
                    string tempValue = txtValue.Text.Replace(",", "");
                    tempValue += numberdecimal.Text; // Append the new number

                    if (double.TryParse(tempValue, out double parsedValue)) // Try to parse the modified string to double
                    {
                        txtValue.Text = parsedValue.ToString("#,0.##"); // Format the number with commas and up to 7 decimal places
                    }
                }
            }
        }      //Accepts value 0-9 and decimal point
        private void OperatorValue_Click(object sender, EventArgs e)
        {
            Button operation = (Button)sender; //handles the inputs +, -, /, *
            if (operate.DivisionError(txtValue.Text))
            {
                ReturnToDefault();
            }
            if (equalsClicked) //checks if equals is clicked
            {
                txtEquation.Clear();
                equalsClicked = false;
            }
            if (operatorClicked) //checks if the operator has been clicked and replace the current operator.
            {
                try
                {
                    txtEquation.Text = txtEquation.Text.Substring(0, txtEquation.Text.Length - 1) + operation.Text;
                    return;
                }
                catch
                {
                    return;
                }
            }
            if (double.TryParse(txtValue.Text, out parsedValue))
            {
                if (RB_MDAS.Checked)
                {
                    txtEquation.Text += " " + parsedValue.ToString("#,0.#######") + " " + operation.Text;
                }
                if (RB_LtoR.Checked)
                {
                    txtEquation.Text += " " + parsedValue.ToString("#,0.#######");
                    result = operate.LeftAssociativity(operate.CleanEquation(txtEquation.Text));
                    txtEquation.Text = result.ToString("#,0.#######") + " " + operation.Text;
                }
            }
            zeroHandled = false;
            operatorClicked = true;
            taskPerformed = false;
        }           //Accepts operators (+, -, /, and *)
        private void Percent_Click(object sender, EventArgs e)
        {
            if (operatorClicked)
            {
                operatorClicked = false;
            }
            
            if (equalsClicked) //checks if equals is clicked
            {
                txtEquation.Clear();
                equalsClicked = false;
            }
            double percent = double.Parse(txtValue.Text) / 100;
            txtValue.Text = percent.ToString("#,0.#######");
        }                 //Convert current number into percentage value
        private void Equals_Click(object sender, EventArgs e)
        {
            if (taskPerformed) //checks if the equals is performed once or value is only "-"
            {
                return;
            }
            if (!equalsClicked) //appends the current value to the equation
            {
                if (txtValue.Text.EndsWith("."))
                {
                    txtValue.Text = txtValue.Text.Replace(".", "");
                    return;
                }
                if (double.TryParse(txtValue.Text, out parsedValue))
                {
                    // Append the formatted value with equals sign to txtEquation
                    txtEquation.Text += " " + parsedValue.ToString("#,0.##") + " = ";
                }
                equalsClicked = true;
            }
            if (txtEquation.Text.Contains("/ 0")) //checks if user has /0 on the equation
            {
                txtValue.Text = "Cannot divide by zero";
                return;
            }
            if (RB_MDAS.Checked) //checks if the radiobutton is currently on MDAS
            {
                result = operate.MDAS(operate.CleanEquation(txtEquation.Text));
            }
            if (RB_LtoR.Checked) //checks if the radiobutton is currently on Left Associative
            {
                result = operate.LeftAssociativity(operate.CleanEquation(txtEquation.Text));
            }
            if (result == double.NegativeInfinity) //handles OverflowException
            {
                txtValue.Text = "Value is too big";
            }
            else
            {
                operatorClicked = false;
                taskPerformed = true;
                txtValue.Text = result.ToString("#,0.##");
                operate.AddToHistory(txtEquation.Text); //adds the equation to the history
                operate.AddToHistory(result.ToString()); // added the total to the history
                operate.AddToHistory(" "); //added a space to the history.

                //NOTE: AddToHistory() has been called thrice in here so that it will append just like in the laptop calculator
            }
        }                  //Perform Calculation
        private void LeftAssociative_Click(object sender, EventArgs e)
        {
            if (equalsClicked) //Checks if the equals click calculation is already done
            {
                if(string.Equals(txtValue.Text, "Cannot divide by zero")) //check if the textbox contains DivideZero message
                {
                    return;
                }
                result = operate.LeftAssociativity(operate.CleanEquation(txtEquation.Text));
                txtValue.Text = result.ToString("#,0.##");
            }
        }         //Sequential Calculation for RadioButton
        private void MDAS_Click(object sender, EventArgs e)
        {
            if (equalsClicked) //Checks if the equals click calculation is already done
            {
                if (string.Equals(txtValue.Text, "Cannot divide by zero"))
                {
                    return;
                }
                result = operate.MDAS(operate.CleanEquation(txtEquation.Text));
                txtValue.Text = result.ToString("#,0.##");
                if (result == double.NegativeInfinity) //Checks for the OverflowException
                {
                    txtValue.Text = "Value is too big";
                }
            }
        }                    //MDAS Calculation for RadioButton
        private void History_Click(object sender, EventArgs e)
        {
            ViewHistory.Visible = !ViewHistory.Visible;
            ViewHistory.SetHistory(operate);
        }                 //Shows the history UserControl
        private void BackSpace_Click(object sender, EventArgs e)
        {
            if (operate.DivisionError(txtValue.Text))
            {
                ReturnToDefault();
            }
            if (txtValue.Text.Length > 0) // Check if there is text in the textbox.
            {
                txtValue.Text = txtValue.Text.Remove(txtValue.Text.Length - 1, 1);
                // Attempt to parse the unformatted value
                if (double.TryParse(txtValue.Text, out parsedValue))
                {
                    txtValue.Text = parsedValue.ToString("#,0.#######"); // Format it back to a string with commas if there's still a number
                }
            }
            if (string.IsNullOrEmpty(txtValue.Text) || string.Equals(txtValue.Text, "0") || string.Equals(txtValue.Text, "-")) // Ensure the textbox does not become empty and reset to "0".
            {
                ReturnToDefault();
            }
        }               //Removes single character at a time
        private void Clear_Click(object sender, EventArgs e)
        {
            ReturnToDefault();
        }                   //Clear all and revert to orinal state
        private void NegateValue_Click(object sender, EventArgs e)
        {
            if (string.Equals(txtValue.Text, "0")) 
            {
                return;
            }
            else
            {
                txtValue.Text = operate.NegateValue(double.Parse(txtValue.Text)).ToString("#,0.#######");
            }
        }             //Changes the txtValue to negative or revert back to positive
        private void ReturnToDefault()
        {
            txtValue.Text = "0";
            txtEquation.Clear();
            operatorClicked = false;
            equalsClicked = false;
            zeroHandled = false;
            taskPerformed = false;
        }                                         //Function to reverts everything back to original state
    }
}