using System.Collections.Generic;

namespace CFNReader;

public class ChannelCollection(IDictionary<Channel, ChannelInfo> channels) : Dictionary<Channel, ChannelInfo>(channels)
{ }