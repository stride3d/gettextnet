using Avalonia.Controls;
using Avalonia.Interactivity;
using GNU.Gettext;
using System;
using System.Globalization;
using System.Threading;

namespace Examples.HelloAvalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        radioEnUS.IsChecked = true;
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        SetTexts();
    }
    private void OnLocaleChanged(object sender, RoutedEventArgs e)
    {
        string locale = (sender as Button)?.Content?.ToString() ?? "en-US";

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
        SetTexts();
    }

    private void SetTexts()
    {
        var catalog = new GettextResourceManager();
        // If satellite assemblies have another base name use GettextResourceManager("Examples.HelloForms.Messages") constructor
        Title = catalog.GetString("Hello, world!");
        label1.Content = catalog.GetString("Hello, world!");
        // Manually formatted strings
        label2.Content = catalog.GetStringFmt("This program is running as process number \"{0}\".", Environment.ProcessId);
        label3.Content = string.Format(
            catalog.GetPluralString("found {0} similar word", "found {0} similar words", 1),
            1);
        label4.Content = string.Format(
            catalog.GetPluralString("found {0} similar word", "found {0} similar words", 2),
            2);
        label5.Content = string.Format(
            catalog.GetPluralString("found {0} similar word", "found {0} similar words", 5),
            5);
        label6.Content = string.Format("{0} ('computers')", catalog.GetParticularString("Computers", "Text encoding"));
        label7.Content = string.Format("{0} ('military')", catalog.GetParticularString("Military", "Text encoding"));
        label8.Content = string.Format("{0} (non-contextual)", catalog.GetString("Text encoding"));
        textBox1.Text = catalog.GetString("Here is an example of how one might continue a very long string\nfor the common case the string represents multi-line output.\n");
    }
}