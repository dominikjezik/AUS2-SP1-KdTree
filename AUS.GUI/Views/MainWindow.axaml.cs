using System;
using AUS.GUI.Models;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CreateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var detailsWindow = new DetailsWindow();
        detailsWindow.CreateAreaObject += OnNewAreaObject;
        detailsWindow.ShowDialog(this);
    }
    
    private void OnNewAreaObject(object? sender, AreaObjectForm areaObjectForm)
    {
        // Add new AreaObjectForm to the list
        throw new NotImplementedException();
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
    }
}