using CommunityToolkit.Mvvm.Collections;

namespace AvaloniaUILoudnessMeter.DataModels;

/// <summary>
/// A non-generic wrapper for ObservableGroup to enable compiled bindings in XAML.
/// </summary>
public class ChannelConfigurationGroup : ObservableGroup<string, ChannelConfigurationItem>
{
    public ChannelConfigurationGroup(IGrouping<string, ChannelConfigurationItem> grouping)
        : base(grouping)
    {
    }
}
