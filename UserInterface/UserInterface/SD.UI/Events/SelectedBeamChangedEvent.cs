using Prism.Events;
using SD.Core.Shared.Models;

namespace SD.UI.Events;
public class SelectedBeamChangedEvent : PubSubEvent<UlsResult?> { }
