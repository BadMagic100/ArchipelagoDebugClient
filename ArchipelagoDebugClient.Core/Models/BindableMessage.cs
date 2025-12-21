using Archipelago.MultiClient.Net.MessageLog.Messages;
using System.Collections.Generic;
using System.Linq;

namespace ArchipelagoDebugClient.Models;

public class BindableMessage
{
    public IReadOnlyList<BindableMessagePart> Parts { get; }

    public BindableMessage(IEnumerable<BindableMessagePart> parts)
    {
        Parts = parts.ToList();
    }

    public BindableMessage(LogMessage msg)
    {
        Parts = msg.Parts.Select(p => new BindableMessagePart(p)).ToList();
    }
}
