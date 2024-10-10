using System;
using AUS.GUI.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class GenerateOperationsWindow : Window
{
    public AreaObjectForm AreaObjectForm { get; private set; } = new();
    
    // event property for the SaveButton_OnClick event
    public event EventHandler<GenerateOperationsModel>? GenerateOperations;
    
    public GenerateOperationsWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO take from GUI
        GenerateOperations?.Invoke(this, new() { Count = 10, ProbabilityOfInsert = 1});
        Close();
    }
}
