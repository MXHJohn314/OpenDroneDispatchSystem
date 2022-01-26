using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class RecoveryRequest : BaseDto
{
    public DroneRecord Record { get; set; } 
}