using System;
using AUS.GUI.Models;
using AUS.GUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class GenerateOperationsWindow : Window
{
    // event property for the SaveButton_OnClick event
    public event EventHandler<GenerateOperationsModel>? GenerateOperations;
    
    private readonly GenerateOperationsViewModel _viewModel;
    
    public GenerateOperationsWindow()
    {
        InitializeComponent();
        _viewModel = new GenerateOperationsViewModel();
        DataContext = _viewModel;
    }

    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        GenerateOperations?.Invoke(this, new()
        {
            CountOfOperations = _viewModel.CountOfOperations, 
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
