using System.Collections.Generic;

namespace Domain.DTO;

public enum OrderState
{
    Waiting,
    Assigned,
    Delivered
}

public static class OrderStateExtensions
{
    private static readonly Dictionary<OrderState, string> StateToStringDict = new()
    {
        {OrderState.Waiting, "Ready"},
        {OrderState.Assigned, "Assigned"},
        {OrderState.Delivered, "Delivered"}
    };

    public static string ToString(this OrderState state)
    {
        return StateToStringDict[state];
    }
}