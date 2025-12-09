using System.Threading.Channels;
using AvaloniaUILoudnessMeter.DataModels;
using AvaloniaUILoudnessMeter.Services;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUILoudnessMeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    #region Private properties
    
    private IAudioInterfaceService _audioInterfaceService;

    #endregion
    
    #region  Public properties

    [ObservableProperty]
    private string _boldTitle = "AVALONIA";
    
    [ObservableProperty]
    private string _regularTitle = "LOUDNESS METER";

    [ObservableProperty]
    private bool _channelConfigurationListIsOpen;
    
    [ObservableProperty]
    private ObservableGroupedCollection<string, ChannelConfigurationItem> _channelConfigurations;
    
    #endregion
    
    #region Public properties
    
    [RelayCommand]
    private void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;
    
    #endregion

    #region Constructor
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public MainViewModel(IAudioInterfaceService audioInterfaceService)
    {
        _audioInterfaceService = audioInterfaceService;
    }
    
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public MainViewModel()
    {
        _audioInterfaceService = new DummyAudioInterfaceService();
    }
    
    #endregion
}