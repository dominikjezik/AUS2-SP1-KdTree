using System;
using AUS.DataStructures.GeoArea;
using AUS.GUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class GenerateObjectsWindow : Window
{
    // event property for the SaveButton_OnClick event
    public event EventHandler<GenerateAreaObjectsOptions>? GenerateObjects;
    
    private readonly GenerateObjectsViewModel _viewModel;
    
    public GenerateObjectsWindow()
    {
        InitializeComponent();
        _viewModel = new GenerateObjectsViewModel();
        DataContext = _viewModel;
    }

    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        GenerateObjects?.Invoke(this, new()
        {
            CountOfParcels = _viewModel.CountOfParcels, 
            CountOfRealEstates = _viewModel.CountOfRealEstates,
            ProbabilityOfOverlay = _viewModel.ProbabilityOfOverlay,
            MinX = _viewModel.MinX,
            MaxX = _viewModel.MaxX,
            MinY = _viewModel.MinY,
            MaxY = _viewModel.MaxY,
            NumberOfDecimalPlaces = _viewModel.NumberOfDecimalPlaces,
            GenerateRandomDescription = _viewModel.GenerateRandomDescription
        });
        
        Close();
    }
}
