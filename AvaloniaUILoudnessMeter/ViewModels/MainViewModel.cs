using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
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
    
    #region Public properties

    [ObservableProperty]
    private string _boldTitle = "AVALONIA";
    
    [ObservableProperty]
    private string _regularTitle = "LOUDNESS METER";

    [ObservableProperty]
    private bool _channelConfigurationListIsOpen;
    
    [ObservableProperty]
    private ObservableGroupedCollection<string, ChannelConfigurationItem> _channelConfigurations;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChannelConfigurationButtonText))]
    private ChannelConfigurationItem? _selectedChannelConfiguration;

    public string ChannelConfigurationButtonText => SelectedChannelConfiguration?.ShortText ?? "Select Channel";
    
    #endregion
    
    #region Public commands
    
    [RelayCommand]
    private void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;

    [RelayCommand]
    private void ChannelConfigurationItemPressed(ChannelConfigurationItem item)
    {
        SelectedChannelConfiguration = item;
        
        // Close the menu
        _channelConfigurationListIsOpen = false;
    }

    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        var channelConfigurations = await _audioInterfaceService.GetChannelConfigurationListAsync();
        
        // Create a grouping from the flat data
        ChannelConfigurations = new ObservableGroupedCollection<string, ChannelConfigurationItem>(
            channelConfigurations.GroupBy( item => item.Group));
    }
    
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