﻿using Apps.MicrosoftOutlook.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.MicrosoftOutlook.Models.Mail.Requests;

public class ListAttachedFilesRequest
{
    [Display("Message ID")]
    [DataSource(typeof(MessageDataSourceHandler))]
    public string MessageId { get; set; }
}