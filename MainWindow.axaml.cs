using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Calculator_Can_KESKIN;

public partial class MainWindow : Window
{
    private double firstNumber = 0;
    private string currentOperator = "";
    private bool waitingForSecondNumber = false;
    private bool hasDecimal = false;

    public MainWindow()
    {
        InitializeComponent();

        DisplayText.Text = "0";
        OperationText.Text = "";
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        string value = button.Content?.ToString() ?? "";

        // Chiffres
        if (double.TryParse(value, out _))
        {
            EnterNumber(value);
            return;
        }

        switch (value)
        {
            case ".":
                EnterDecimal();
                break;

            case "+":
            case "−":
            case "×":
            case "÷":
                SetOperator(value);
                break;

            case "=":
                CalculateResult();
                break;

            case "AC":
                Clear();
                break;

            case "+/−":
                ToggleSign();
                break;

            case "%":
                Percentage();
                break;
        }
    }

    // NOMBRES

    private void EnterNumber(string number)
    {
        if (DisplayText.Text == "0" || waitingForSecondNumber)
        {
            DisplayText.Text = number;
            waitingForSecondNumber = false;
        }
        else
        {
            DisplayText.Text += number;
        }

        UpdateDisplaySize();
    }

    // VIRGULE

    private void EnterDecimal()
    {
        if (waitingForSecondNumber)
        {
            DisplayText.Text = "0.";
            waitingForSecondNumber = false;
            hasDecimal = true;
            return;
        }

        if (!DisplayText.Text!.Contains("."))
        {
            DisplayText.Text += ".";
            hasDecimal = true;
        }
    }

    // OPERATEUR

    private void SetOperator(string operation)
    {
        if (!double.TryParse(
                DisplayText.Text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double number))
        {
            return;
        }

        if (!string.IsNullOrEmpty(currentOperator) && !waitingForSecondNumber)
        {
            CalculateResult();
        }

        firstNumber = double.Parse(
            DisplayText.Text!,
            CultureInfo.InvariantCulture);

        currentOperator = operation;
        waitingForSecondNumber = true;
        hasDecimal = false;

        OperationText.Text = $"{FormatNumber(firstNumber)} {operation}";
    }

    // CALCUL

    private void CalculateResult()
    {
        if (string.IsNullOrEmpty(currentOperator))
            return;

        if (!double.TryParse(
                DisplayText.Text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double secondNumber))
        {
            return;
        }

        double result;

        switch (currentOperator)
        {
            case "+":
                result = firstNumber + secondNumber;
                break;

            case "−":
                result = firstNumber - secondNumber;
                break;

            case "×":
                result = firstNumber * secondNumber;
                break;

            case "÷":

                if (secondNumber == 0)
                {
                    DisplayText.Text = "Erreur";
                    OperationText.Text = "Division par zéro";
                    currentOperator = "";
                    return;
                }

                result = firstNumber / secondNumber;
                break;

            default:
                return;
        }

        OperationText.Text =
            $"{FormatNumber(firstNumber)} {currentOperator} {FormatNumber(secondNumber)} =";

        DisplayText.Text = FormatNumber(result);

        firstNumber = result;
        currentOperator = "";
        waitingForSecondNumber = true;
        hasDecimal = DisplayText.Text.Contains(".");

        UpdateDisplaySize();
    }

    // POURCENTAGE

    private void Percentage()
    {
        if (!double.TryParse(
                DisplayText.Text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double number))
        {
            return;
        }

        number /= 100;

        DisplayText.Text = FormatNumber(number);

        UpdateDisplaySize();
    }

    // + / -

    private void ToggleSign()
    {
        if (!double.TryParse(
                DisplayText.Text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double number))
        {
            return;
        }

        number *= -1;

        DisplayText.Text = FormatNumber(number);

        UpdateDisplaySize();
    }

    // AC
    private void Clear()
    {
        firstNumber = 0;
        currentOperator = "";
        waitingForSecondNumber = false;
        hasDecimal = false;

        DisplayText.Text = "0";
        OperationText.Text = "";
        
        UpdateDisplaySize();
    }

    // FORMATAGE

    private string FormatNumber(double number)
    {
        if (double.IsNaN(number) || double.IsInfinity(number))
            return "Erreur";

        if (number == Math.Truncate(number))
            return number.ToString(
                "N0",
                CultureInfo.InvariantCulture);

        return number.ToString(
            "0.##########",
            CultureInfo.InvariantCulture);
    }

    // TAILLE AUTOMATIQUE DU TEXTE

    private void UpdateDisplaySize()
    {
        if (DisplayText.Text == null)
            return;

        int length = DisplayText.Text.Length;

        if (length <= 7)
            DisplayText.FontSize = 64;

        else if (length <= 9)
            DisplayText.FontSize = 54;

        else if (length <= 11)
            DisplayText.FontSize = 46;

        else if (length <= 14)
            DisplayText.FontSize = 38;

        else
            DisplayText.FontSize = 32;
    }
}