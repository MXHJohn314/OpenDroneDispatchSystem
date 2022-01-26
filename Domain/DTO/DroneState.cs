using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO.DroneDispatchCommunication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO;

public enum DroneState
{
    Ready,
    Delivering,
    Returning,
    Dead,
    Charging,
    Unititialized,
    Assigned
}
public static class DroneStateExtensions
{
    private static readonly Dictionary<DroneState, string> StateToColorDict = new(){
        { DroneState.Ready, "#13FF00" },
        { DroneState.Delivering, "#98f5ff" },
        { DroneState.Returning, "#8A2BE2" },
        { DroneState.Dead, "#FF0000" },
        { DroneState.Charging, "#FF9A08" },
        { DroneState.Unititialized, "#964B00" },
        { DroneState.Assigned, "#FF9A08" }
    };
    private static readonly Dictionary<DroneState, string> StateToStringDict = new(){
        { DroneState.Ready, "Ready" },
        { DroneState.Delivering, "Delivering" },
        { DroneState.Returning, "Returning" },
        { DroneState.Dead, "Dead" },
        { DroneState.Unititialized, "Unititialized" },
        { DroneState.Charging, "Charging" },
        { DroneState.Assigned, "Assigned" }
    };
    public static string GetColor(this DroneState state) => StateToColorDict[state];
    public static string ToString(this DroneState state) => StateToStringDict[state];
}
