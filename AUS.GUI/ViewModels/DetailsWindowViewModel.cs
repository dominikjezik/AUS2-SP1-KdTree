using AUS.GUI.Models;

namespace AUS.GUI.ViewModels;

public partial class DetailsWindowViewModel : ViewModelBase
{
    public AreaObjectForm AreaObjectForm { get; private set; } = new();
    
    public string Title { get; private set; } = "Vytvorenie nového objektu";
}
