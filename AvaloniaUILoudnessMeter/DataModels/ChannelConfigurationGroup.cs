using System.Collections.ObjectModel;
using System.Linq;

namespace AvaloniaUILoudnessMeter.DataModels;

/// <summary>
/// A group of channel configuration items with a key, enabling compiled bindings in XAML.
/// </summary>
public class ChannelConfigurationGroup : ObservableCollection<ChannelConfigurationItem>
{
    public string Key { get; }

    public ChannelConfigurationGroup(IGrouping<string, ChannelConfigurationItem> grouping)
        : base(grouping)
    {
        Key = grouping.Key;
    }
}
