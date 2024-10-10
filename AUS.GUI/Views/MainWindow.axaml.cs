using System;
using AUS.DataStructures.GeoArea;
using AUS.GUI.Models;
using AUS.GUI.ViewModels;
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
        var detailsWindow = new CreateAreaObjectWindow();
        detailsWindow.CreateAreaObject += OnNewAreaObject;
        detailsWindow.ShowDialog(this);
    }
    
    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var generateWindow = new GenerateOperationsWindow();
        generateWindow.GenerateOperations += OnGenerateOperations;
        generateWindow.ShowDialog(this);
    }
    
    private void OnNewAreaObject(object? sender, AreaObjectForm areaObjectForm)
    {
        // Add new AreaObjectForm to the list
        throw new NotImplementedException();
    }
    
    private void OnGenerateOperations(object? sender, GenerateOperationsModel generateOperationsModel)
    {
        // Generate operations
        throw new NotImplementedException();
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
    }

    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedAreaObject = (AreaObject?)e.AddedItems[0];
        if (selectedAreaObject == null)
        {
            return;
        }
        
        ((MainWindowViewModel)DataContext).SelectedAreaObjectForm = new AreaObjectForm
        {
            Type = selectedAreaObject.Type,
            Description = selectedAreaObject.Description,
            Id = selectedAreaObject.Id.ToString(),
            CoordinateAX = selectedAreaObject.CoordinateA.X.ToString(),
            CoordinateAY = selectedAreaObject.CoordinateA.Y.ToString(),
            CoordinateBX = selectedAreaObject.CoordinateB.X.ToString(),
            CoordinateBY = selectedAreaObject.CoordinateB.Y.ToString()
        };
        
        
    }
}