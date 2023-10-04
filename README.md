# Blackbird.io Microsoft Outlook

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Microsoft Outlook is a personal information manager software system that allows users to manage their emails, calendars, and contacts in one integrated platform.

## Before setting up

Before you can connect you need to make sure that you have a Microsoft 365 account.

## Connecting

1. Navigate to apps and search for Microsoft Outlook. If you cannot find Microsoft Outlook then click _Add App_ in the top right corner, select Microsoft Outlook and add the app to your Blackbird environment.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My organization'.
4. Click _Authorize connection_.
5. Follow the instructions that Microsoft gives you, authorizing Blackbird.io to act on your behalf. 
6. When you return to Blackbird, confirm that the connection has appeared and the status is _Connected_.

![Connecting](image/README/connecting.png)

## Actions

### Mail

- **List most recent messages** returns messages received during past hours. If number of hours is not specified, messages received during past 24 hours are listed. To retrieve messages from specific mail folder (e.g. inbox), specify the respective parameter.
- **Get message** retrieves specific email from your mailbox.
- **List attached files** retrieves a list of files attached to a message.
- **List mail folders**.
- **Create draft message** creates a draft of a new message. This action is useful when you need to make updates to your email later in the flow (for example, adding attachments in a loop) or when you need it to be reviewed by person later.
- **Attach file to draft message**.
- **Update draft message subject**.
- **Update draft message body**.
- **Add recipients to draft message** adds one or more email recipients to an existing recipients list of a draft message.
- **Remove recipients from draft message** removes one or more email recipients from an existing recipients list of a draft message.
- **Send draft message**.
- **Send new message** creates a new message and send it in one action.
- **Forward message**.
- **Reply to a message**.
- **Delete message** deletes sent or draft message.

### Calendar

- **List calendars** returns a list of current user's calendars.
- **Get calendar** retrieves a calendar. If calendar is not specified, default calendar is returned.
- **Create calendar**.
- **Get users' schedule information** returns the free/busy availability information for a collection of users in specified time period.
- **Rename calendar** renames a calendar. If calendar is not specified, default calendar is renamed.
- **Delete calendar**.
- **List events** retrieves a list of events in a calendar. If calendar is not specified, default calendar's events are listed.
- **Get event**. 
- **List occurrences of event** returns the occurrences of an event for a specified time range.
- **List recently created events** retrieves a list of events created during past hours. If number of hours is not specified, events created during past 24 hours are listed. If calendar is not specified, default calendar's events are listed.
- **List recently updated events** retrieves a list of events updated during past hours. If number of hours is not specified, events updated during past 24 hours are listed. If calendar is not specified, default calendar's events are listed.
- **Create event in a calendar** create a new event in a calendar. If calendar is not specified, the event is created in the default calendar. If the event is an online meeting, a Microsoft Teams meeting is automatically created. To create a recurring event specify recurrence pattern and interval which can be in days, weeks or months, depending on recurrence pattern type. If interval is not specified, it is set to 1. For weekly or monthly patterns provide days of week on which the event occurs.
- **Cancel event** sends a cancellation message and cancels the event. Can be performed only by organizer or a meeting. 
- **Cancel event occurrence** sends a cancellation message and cancels an occurrence of a recurring meeting. Can be performed only by organizer or a meeting. 
- **Forward event** forwards the meeting request to a new recipient.
- **Update event**.
- **Update event occurrence**.
- **Delete event**.

### Contacts

- **List contacts**.
- **Get contact**.
- **Create contact**.
- **Update contact**.
- **Add email to contact**.
- **Remove email from contact**.
- **Add business phone number**.
- **Remove business phone number**
- **Delete contact**.

## Events

### Mail

- **On email created** is triggered when a new email is created in specified mail folder.
- **On email updated** is triggered when an email is updated in specified mail folder.
- **On email received** is triggered when a new email is received in inbox folder.
- **On email with files attached received** is triggered when an email with file attachments is received in inbox folder.

### Calendar

- **On event created** is triggered when a new event is created in specified calendar.

## Example

![example](image/README/example.png)

Here, whenever an email is received we perform email content classification using one of Cohere's models and then, based on its prediction, forward the message to a specific customer support specialist.

<!-- end docs -->
