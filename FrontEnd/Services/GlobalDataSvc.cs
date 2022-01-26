using System;
using System.Collections.Generic;
using Domain.Entities;

namespace FrontEnd.Services;

public class GlobalDataSvc
{
    public DroneRecord currDrone { get; set; }
    public Order currOrder { get; set; }
    public List<object> cart { get; set; }

    public event Action OnChange;

    private void NotifyDataChanged()
    {
        OnChange?.Invoke();
    }
}