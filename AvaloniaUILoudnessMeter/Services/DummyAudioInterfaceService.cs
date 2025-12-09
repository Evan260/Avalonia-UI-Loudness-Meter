using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaUILoudnessMeter.DataModels;

namespace AvaloniaUILoudnessMeter.Services;

public class DummyAudioInterfaceService : IAudioInterfaceService
{
    public Task<List<ChannelConfigurationItem>> ChannelConfigurationListAsync() =>
        Task.FromResult(new List<ChannelConfigurationItem>
        {
            new ChannelConfigurationItem("Mono Stereo Configuration", "Mono", "Mono"),
            new ChannelConfigurationItem("Mono Stereo Configuration", "Stereo", "Stereo"),
            new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, Ls, Rs, C, LFE)", "5.1 DTS"),
            new ChannelConfigurationItem("5.1 ITU", "5.1 DTS - (L, R, C, LFE, Ls, Rs)", "5.1 ITU"),
            new ChannelConfigurationItem("5.1 FILM", "5.1 DTS - (L, C, R, Ls, Rs, LFE)", "5.1 FILM")
        });
}