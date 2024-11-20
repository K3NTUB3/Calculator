using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public class FunctionClass
    {
        private string history;
        public double MDAS(string input)
        {
            try
            {
                var dataTable = new DataTable();
                var result = dataTable.Compute(input, string.Empty);
                return Convert.ToDouble(result);
            }
            catch(OverflowException)
            {
                return double.NegativeInfinity;
            }
        }
        public double LeftAssociativity(string input)
        {
            var regex = new Regex(@"(\d+(\.\d+)?)|([\+\-\*/])");
            var matches = regex.Matches(input);
            double result = 0;
            double currentNumber = 0;
            string currentOperator = "+";

            foreach (Match match in matches)
            {
                if (double.TryParse(match.Value, out currentNumber))
                {
                    switch (currentOperator)
                    {
                        case "+":
                            result += currentNumber;
                            break;
                        case "-":
                            result -= currentNumber;
                            break;
                        case "*":
                            result *= currentNumber;
                            break;
                        case "/":
                            if (currentNumber == 0)
                            {
                                return double.PositiveInfinity;
                            }
                            result /= currentNumber;
                            break;
                    }
                }
                else
                {
                    currentOperator = match.Value;
                }
            }
            return result;
        }
        public void AddToHistory(string input)
        {
            history += input + Environment.NewLine;
        }
        public string GetHistory()
        {
            if (string.IsNullOrEmpty(history))
            {
                return "No Calculation History yet";
            }
            else
            {
                return history;
            }
        }
        public double NegateValue(double input)
        {
            return input -= (input * 2);
        }
        public string CleanEquation(string input)
        {
            input = input.Replace("=", "").Trim(); // Remove '=' and trim the string
            input = input.Replace(" ", ""); // Remove all spaces
            input = input.Replace(",", "");
            return input;
        }
        public bool DivisionError(string input)
        {
            if(string.Equals(input, "Cannot divide by zero") ||  string.Equals(input, "Value is too big"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void PlayAudio(bool isChecked)
        {
            if (isChecked)
            {
                try
                {
                    // Relative path to the audio file in the output directory
                    string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "click-button-140881.wav");

                    using (SoundPlayer player = new SoundPlayer(audioPath))
                    {
                        player.Play();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error playing audio: " + ex.Message);
                }
            }
            else
            {
                return;
            }
        }

    }
}
