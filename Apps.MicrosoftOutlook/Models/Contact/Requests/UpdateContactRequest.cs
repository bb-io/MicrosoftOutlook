using Blackbird.Applications.Sdk.Common;

namespace Apps.MicrosoftOutlook.Models.Contact.Requests;

public class UpdateContactRequest
{
    [Display("Contact ID")]
    public string ContactId { get; set; }
    
    public string? Name { get; set; }
    
    [Display("Middle name")]
    public string? MiddleName { get; set; }
    
    public string? Surname { get; set; }
    
    [Display("Nickname")]
    public string? Nickname { get; set; }
    
    public DateTime? Birthday { get; set; }
    
    public string? Title { get; set; }
    
    [Display("Job title")]
    public string? JobTitle { get; set; }
    
    [Display("Company name")]
    public string? CompanyName { get; set; }
    
    [Display("Department name")]
    public string? DepartmentName { get; set; }
    
    [Display("Mobile phone")]
    public string? MobilePhone { get; set; }
    
    [Display("Business phones")]
    public List<string>? BusinessPhones { get; set; }
    
    public List<string>? Emails { get; set; }
}