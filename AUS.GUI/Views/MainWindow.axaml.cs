using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AUS.DataStructures.GeoArea;
using AUS.GUI.Models;
using AUS.GUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class MainWindow : Window
{
    private readonly GeoAreaService _geoAreaService = new();
    
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
    
    private void OnNewAreaObject(object? sender, AreaObjectForm areaObjectForm)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var areaObject = areaObjectForm.ToAreaObject();
        
        _geoAreaService.Insert(areaObject);
        viewModel.AreaObjects.Add(areaObject);
        viewModel.RefreshSelectedAreaObject();
    }
    
    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var generateWindow = new GenerateOperationsWindow();
        generateWindow.GenerateOperations += OnGenerateOperations;
        generateWindow.ShowDialog(this);
    }
    
    private void OnGenerateOperations(object? sender, GenerateOperationsModel generateOperationsModel)
    {
        _geoAreaService.GenerateOperations(
            generateOperationsModel.Count, 
            generateOperationsModel.ProbabilityOfInsert, 
            generateOperationsModel.MinX, 
            generateOperationsModel.MaxX, 
            generateOperationsModel.MinY, 
            generateOperationsModel.MaxY, 
            generateOperationsModel.NumberOfDecimalPlaces,
            generateOperationsModel.GenerateRandomDescription
        );
    }

    private void SearchButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        viewModel.AreaObjects.Clear();
        
        List<AreaObject> areaObjects = new();

        bool searchByCoordinateAX = double.TryParse(viewModel.AreaObjectQuery.CoordinateAX, out var coordinateAX); 
        bool searchByCoordinateAY = double.TryParse(viewModel.AreaObjectQuery.CoordinateAY, out var coordinateAY);

        bool searchByCoordinateBX = double.TryParse(viewModel.AreaObjectQuery.CoordinateBX, out var coordinateBX);
        bool searchByCoordinateBY = double.TryParse(viewModel.AreaObjectQuery.CoordinateBY, out var coordinateBY);
        
        if (searchByCoordinateAX && searchByCoordinateAY && searchByCoordinateBX && searchByCoordinateBY)
        {
            areaObjects = _geoAreaService.Find(viewModel.AreaObjectQuery.Type, new GPSCoordinate(coordinateAX, coordinateAY), new GPSCoordinate(coordinateBX, coordinateBY));
        }
        else if (searchByCoordinateAX && searchByCoordinateAY)
        {
            areaObjects = _geoAreaService.Find(viewModel.AreaObjectQuery.Type, new GPSCoordinate(coordinateAX, coordinateAY));
        }
        else if (searchByCoordinateBX && searchByCoordinateBY)
        {
            areaObjects = _geoAreaService.Find(viewModel.AreaObjectQuery.Type, new GPSCoordinate(coordinateBX, coordinateBY));
        }
        else
        {
            areaObjects = _geoAreaService.Get(viewModel.AreaObjectQuery.Type);
        }

        viewModel.AreaObjects = new ObservableCollection<AreaObject>(areaObjects);
    }
    
    private void DataGrid_OnSelectionAssociatedObjectChanged(object? sender, SelectionChangedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var selected = (AreaObject)((DataGrid)sender!).SelectedItem;

        if (selected != null)
        {
            viewModel.SelectedAreaObject = selected;
        }
    }

    private void SaveToFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void LoadFromFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void SaveAreaObjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var originalAreaObject = viewModel.SelectedAreaObject!;
        var form = viewModel.SelectedAreaObjectForm!;
        
        bool correctCoordinateAX = double.TryParse(form.CoordinateAX, out var coordinateAX); 
        bool correctCoordinateAY = double.TryParse(form.CoordinateAY, out var coordinateAY);

        bool correctCoordinateBX = double.TryParse(form.CoordinateBX, out var coordinateBX);
        bool correctCoordinateBY = double.TryParse(form.CoordinateBY, out var coordinateBY);
        
        bool correctId = int.TryParse(form.Id, out var id);
        
        if (!correctCoordinateAX || !correctCoordinateAY || !correctCoordinateBX || !correctCoordinateBY)
        {
            return;
        }
        
        // Kontrola ci sa nezmenili suradnice
        if (originalAreaObject.CoordinateA == new GPSCoordinate(coordinateAX, coordinateAY) && 
            originalAreaObject.CoordinateB == new GPSCoordinate(coordinateBX, coordinateBY))
        {
            originalAreaObject.Id = id;
            originalAreaObject.Description = form.Description;
            viewModel.RefreshAreaObjects();
        }
        else
        {
            // TODO: Zmenili sa aj suradnice, treba (obe?) suradnice odstranit a na novo pridat
            throw new NotImplementedException();
        }
    }

    private void DeleteAreaObjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
