using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using System;

namespace NewCalc
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected TextView calcText;

        private string[] numbers = new string[2];

        private string _operator;   //current value displayed in TextView

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            //assign var calcText to textview in the interface
            calcText = FindViewById<TextView>(Resource.Id.calculator_text_view);
        }

        //same method name as "<android:onClick">ButtonClick<...>"
        //requires adding Mono.Android.Export to References folder
        [Java.Interop.Export("ButtonClick")]
        public void ButtonClick (View v)
        {
            //cast var v to button
            Button button = (Button)v;

            //if button.Text contains a "0-9; operators; =; or ." it can be added into the calculation
            if ("0123456789.".Contains(button.Text))
                AddNumberOrDecimalPoint(button.Text);
            else if ("÷×+−±".Contains(button.Text))
                AddOperator(button.Text);
            else if ("±" == button.Text)
               Negative(button.Text);
            else if ("=" == button.Text)
                Calculate();
            else
                Erase();
        }

        private void AddNumberOrDecimalPoint(string value)
        {
            //if the current operator (displayed number) is null,
            //work with the first [0] operator from the array
            //if is not, work with the second [1] operator
            int index = _operator == null ? 0 : 1;

            //return if input is a decimal point (.)
            //and the value is already contain a decimal point (.)
            if (value == "." && numbers[index].Contains("."))
                return;
            //otherwise, assing current index to value (TextView)
            numbers[index] += value;

            UpdateCalculatorText();
        }

        private void AddOperator(string value)
        {
            //checking if the second number (numbers[1]) contains something already in it
            //if it is, it execute calculation first, and that add a second operator
            if (numbers[1] != null)
            {
                Calculate(value);
                return;
            }

            _operator = value;
            UpdateCalculatorText();
        }

        private void Negative(string value)
        {
            int index = _operator == null ? 0 : 1;
            numbers[index] = $"({"-"} {numbers[index]})";

            UpdateCalculatorText();
        }

        //"string newOperator = null"- makes operator optional (don't interfere with ButtonClick operator)
        //this has to be done because this Calculate method is called from two places
        private void Calculate(string newOperator = null)
        {
            //double? - let to assign "double" value type to null (double cannot be nuable by default)
            double? result = null;
            //assign first/second operator to null
            double? first = numbers[0] == null ? null : (double?)double.Parse(numbers[0]);
            double? second = numbers[1] == null ?  null : (double?)double.Parse(numbers[1]);

            switch (_operator)
            {
                case "÷":
                    result = first / second;
                    break;
                case "×":
                    result = first * second;
                    break;
                case "+":
                    result = first + second;
                    break;
                case "−":
                    result = first - second;
                    break;
            }

            if (result != null)
            {
                //numbers[0] show the result
                numbers[0] = result.ToString();
                _operator = newOperator;
                numbers[1] = null;
                UpdateCalculatorText();
            }
        }

        private void Erase()
        {
            numbers[0] = null;
            numbers[1] = null;
            _operator = null;
            UpdateCalculatorText();
        }

        private void UpdateCalculatorText()
        {
            calcText.Text = $"{numbers[0]} {_operator} {numbers[1]}";
        }
    }
}
