using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUILoudnessMeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string boldTitle = "AVALONIA";
    
    [ObservableProperty]
    private string regularTitle = "LOUDNESS METER";

    [ObservableProperty]
    private bool _channelConfigurationListIsOpen;
    
    [RelayCommand]
    private void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;
}