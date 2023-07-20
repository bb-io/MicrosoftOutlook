using Blackbird.Applications.Sdk.Common;
using Microsoft.Graph.Models;

namespace Apps.MicrosoftOutlook.Dtos;

public class ContactDto
{
    public ContactDto(Contact contact)
    {
        ContactId = contact.Id;
        Name = contact.GivenName;
        MiddleName = contact.MiddleName;
        Surname = contact.Surname;
        Nickname = contact.NickName;
        Birthday = contact.Birthday?.DateTime;
        Title = contact.Title;
        JobTitle = contact.JobTitle;
        CompanyName = contact.CompanyName;
        DepartmentName = contact.Department;
        MobilePhone = contact.MobilePhone;
        BusinessPhones = contact.BusinessPhones;
        Emails = contact.EmailAddresses.Select(address => address.Address);
    }
    
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
    public IEnumerable<string>? BusinessPhones { get; set; }
    
    public IEnumerable<string>? Emails { get; set; }
}