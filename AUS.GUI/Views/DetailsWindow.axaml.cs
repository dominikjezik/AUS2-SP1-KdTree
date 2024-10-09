using System;
using AUS.GUI.Models;
using AUS.GUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class DetailsWindow : Window
{
    public AreaObjectForm AreaObjectForm => (DataContext as DetailsWindowViewModel)!.AreaObjectForm;
    
    // event property for the SaveButton_OnClick event
    public event EventHandler<AreaObjectForm>? CreateAreaObject;
    
    public DetailsWindow()
    {
        InitializeComponent();
        DataContext = new DetailsWindowViewModel();
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CreateAreaObject?.Invoke(this, AreaObjectForm);
        Close();
    }
}