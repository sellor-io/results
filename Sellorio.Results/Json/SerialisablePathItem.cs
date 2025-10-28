using Sellorio.Results.Messages;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Sellorio.Results.Json;

internal class SerialisablePathItem
{
    [JsonPropertyName("v")]
    public required JsonElement Value { get; init; }

    [JsonPropertyName("t")]
    public required ResultMessagePathItemType Type { get; init; }
}
