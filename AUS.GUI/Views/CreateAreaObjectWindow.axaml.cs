using System;
using AUS.GUI.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class CreateAreaObjectWindow : Window
{
    public AreaObjectForm AreaObjectForm { get; private set; } = new();
    
    // event property for the SaveButton_OnClick event
    public event EventHandler<AreaObjectForm>? CreateAreaObject;
    
    public CreateAreaObjectWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CreateAreaObject?.Invoke(this, AreaObjectForm);
        Close();
    }
}
