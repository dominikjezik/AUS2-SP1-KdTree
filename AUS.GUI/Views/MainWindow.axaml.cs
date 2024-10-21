using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
    
    private void OnNewAreaObject(object? sender, AreaObjectDTO areaObject)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        
        var insertedAreaObject = _geoAreaService.Insert(areaObject);
        viewModel.AreaObjects.Add(insertedAreaObject);
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
            generateOperationsModel.CountOfOperations, 
            generateOperationsModel.ProbabilityOfOverlay, 
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
        
        var areaObjects = _geoAreaService.Find(viewModel.AreaObjectQuery);
        viewModel.AreaObjects = new ObservableCollection<AreaObjectDTO>(areaObjects);
    }
    
    private void DataGrid_OnSelectionAssociatedObjectChanged(object? sender, SelectionChangedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var selected = (AreaObjectDTO)((DataGrid)sender!).SelectedItem;

        if (selected != null)
        {
            selected.LoadAssociatedObjects();
            viewModel.SelectedAreaObject = selected;
        }
    }

    private void SaveToFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var folderDialog = StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "Vyberte priečinok pre uloženie súborov",
            AllowMultiple = false
        });
        
        folderDialog.Wait();
        
        var result = folderDialog.Result;
        
        if (result.Count == 0)
        {
            return;
        }

        var folder = result[0].Path;
        
        _geoAreaService.SaveToFolder(folder);
    }

    private void LoadFromFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var folderDialog = StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "Vyberte priečinok pre načítanie súborov",
            AllowMultiple = false
        });
        
        folderDialog.Wait();
        
        var result = folderDialog.Result;
        
        if (result.Count == 0)
        {
            return;
        }

        var folder = result[0].Path;
        
        _geoAreaService.LoadFromFolder(folder);
    }

    private void SaveAreaObjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var areaObject = viewModel.SelectedAreaObject!;
        
        var updatedAreaObject = _geoAreaService.Update(areaObject);
        
        var indexOfModifiedAreaObject = viewModel.AreaObjects.IndexOf(areaObject);
        
        // "refresh"
        viewModel.AreaObjects[indexOfModifiedAreaObject] = new AreaObjectDTO();
        viewModel.AreaObjects[indexOfModifiedAreaObject] = updatedAreaObject;
        
        viewModel.SelectedAreaObject = updatedAreaObject;
    }

    private void DeleteAreaObjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var areaObject = viewModel.SelectedAreaObject!;
        
        _geoAreaService.Delete(areaObject);
        viewModel.SelectedAreaObject = null;
        viewModel.AreaObjects.Remove(areaObject);
    }
}
