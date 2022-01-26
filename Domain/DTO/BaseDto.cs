using Newtonsoft.Json;

namespace Domain.DTO;

public class BaseDto
{
    [JsonProperty("Message")] public string Message { get; set; }
}