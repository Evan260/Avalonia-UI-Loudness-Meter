using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaUILoudnessMeter.DataModels;

namespace AvaloniaUILoudnessMeter.Services;

public interface IAudioInterfaceService
{
    /// <summary>
    /// Fetch the channel configurations.
    /// </summary>
    Task<List<ChannelConfigurationItem>> GetChannelConfigurationListAsync();
}