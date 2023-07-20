using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class CalendarDto
{
    public CalendarDto(Calendar calendar)
    {
        CalendarId = calendar.Id;
        Name = calendar.Name;
        CanShare = calendar.CanShare;
        CanEdit = calendar.CanEdit;
        CanViewPrivateItems = calendar.CanViewPrivateItems;
        IsDefaultCalendar = calendar.IsDefaultCalendar;
        Owner = new OwnerDto { OwnerEmail = calendar.Owner.Address, OwnerName = calendar.Owner.Name };
    }
    
    [Display("Calendar ID")]
    public string CalendarId { get; set; }
    
    public string Name { get; set; }

    [Display("Can share")]
    public bool? CanShare { get; set; }
    
    [Display("Can edit")]
    public bool? CanEdit { get; set; }
    
    [Display("Can view private items")]
    public bool? CanViewPrivateItems { get; set; }
    
    [Display("Is default calendar")]
    public bool? IsDefaultCalendar { get; set; }
    
    public OwnerDto Owner { get; set; }
}

public class OwnerDto
{
    [Display("Owner name")]
    public string OwnerName { get; set; }
    
    [Display("Owner email")]
    public string OwnerEmail { get; set; }
}