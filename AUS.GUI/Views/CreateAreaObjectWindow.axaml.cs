using System;
using System.Collections.Generic;
using AUS.DataStructures.GeoArea;
using AUS.GUI.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AUS.GUI.Views;

public partial class CreateAreaObjectWindow : Window
{
    public AreaObjectForm AreaObjectForm { get; private set; } = new();
    
    public Dictionary<AreaObjectType, string> ObjectTypes { get; set; } = new()
    {
        { AreaObjectType.Parcel, "Parcela" },
        { AreaObjectType.RealEstate, "Nehnuteľnosť" }
    };
    
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
