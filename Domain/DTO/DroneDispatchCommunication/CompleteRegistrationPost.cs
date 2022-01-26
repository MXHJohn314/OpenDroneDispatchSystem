using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationRequest : BaseDto
{
    [JsonPropertyName("Record")] public DroneRecord Record { get; set; }
}