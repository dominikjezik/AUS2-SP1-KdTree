using System;
using System.Collections.Generic;
using AUS.DataStructures.GeoArea;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class CreateAreaObjectWindow : Window
{
    public AreaObjectDTO AreaObject { get; private set; } = new();
    
    public Dictionary<AreaObjectType, string> ObjectTypes { get; set; } = new()
    {
        { AreaObjectType.Parcel, "Parcel" },
        { AreaObjectType.RealEstate, "Real Estate" }
    };
    
    public List<char> CoordinateXDirection { get; set; } = new() { 'E', 'W' };
    
    public List<char> CoordinateYDirection { get; set; } = new() { 'N', 'S' };
    
    public event EventHandler<AreaObjectDTO>? CreateAreaObject;
    
    public CreateAreaObjectWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CreateAreaObject?.Invoke(this, AreaObject);
        Close();
    }
}
