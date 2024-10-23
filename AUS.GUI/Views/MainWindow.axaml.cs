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
    }
    
    private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var generateWindow = new GenerateOperationsWindow();
        generateWindow.GenerateOperations += OnGenerateOperations;
        generateWindow.ShowDialog(this);
    }
    
    private void OnGenerateOperations(object? sender, GenerateOperationsModel generateOperationsModel)
    {
        _geoAreaService.GenerateAreaObjects(
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
            // Opatovne nacitanie (treba mat k dispozicii asociovane objekty)
            var selectedAreaObject = _geoAreaService.Find(selected.Type, selected.CoordinateA, selected.InternalId!.Value);
            viewModel.SelectedAreaObject = selectedAreaObject;
        }
    }

    private void SaveToFileButton_OnClick(object? sender, RoutedEventArgs e) => ShowSaveToFileDialog();
    
    public async Task ShowSaveToFileDialog()
    {
        var folderDialogResult = await StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "Vyberte priečinok pre uloženie súborov",
            AllowMultiple = false
        });
        
        if (folderDialogResult.Count == 0)
        {
            return;
        }

        var folder = folderDialogResult[0].Path;
        
        _geoAreaService.SaveToFolder(folder);
    }

    private void LoadFromFileButton_OnClick(object? sender, RoutedEventArgs e) => ShowLoadFromFileDialog();

    private async Task ShowLoadFromFileDialog()
    {
        var folderDialogResult = await StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "Vyberte priečinok pre načítanie súborov",
            AllowMultiple = false
        });
        
        if (folderDialogResult.Count == 0)
        {
            return;
        }

        var folder = folderDialogResult[0].Path;
        
        _geoAreaService.LoadFromFolder(folder);
        
        var viewModel = (MainWindowViewModel)DataContext!;
        
        viewModel.SelectedAreaObject = null;
        viewModel.AreaObjects.Clear();
    }

    private void SaveAreaObjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        
        var originalAreaObject = viewModel.OriginalSelectedAreaObject!;
        var modifiedAreaObject = viewModel.SelectedAreaObject!;
        
        var updatedAreaObject = _geoAreaService.Update(originalAreaObject, modifiedAreaObject);
        
        var indexOfOriginalAreaObject = viewModel.AreaObjects.IndexOf(originalAreaObject);

        if (indexOfOriginalAreaObject == -1)
        {
            return;
        }
        
        viewModel.SelectedAreaObject = updatedAreaObject;
        viewModel.AreaObjects[indexOfOriginalAreaObject] = updatedAreaObject;
        
        // Z nejakeho dovodu sa nenastavi rovnaka referencia na original pri nastaveni
        // selected vyssie, preto dodatocne nastavenie aby sedeli referencie
        viewModel.OriginalSelectedAreaObject = updatedAreaObject;
    }

    private void DeleteAreaObjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var areaObject = viewModel.OriginalSelectedAreaObject!;
        
        _geoAreaService.Delete(areaObject);
        viewModel.SelectedAreaObject = null;
        viewModel.AreaObjects.Remove(areaObject);
    }
}
