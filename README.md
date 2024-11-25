# Blackbird.io Microsoft 365 Email (Outlook)

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Microsoft 365 Email is a personal information manager software system that allows users to manage their emails.

## Before setting up

Before you can connect you need to make sure that you have a Microsoft 365 account.

## Connecting

1. Navigate to apps and search for Microsoft 365 Email.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My organization'.
4. Click _Authorize connection_.
5. Follow the instructions that Microsoft gives you, authorizing Blackbird.io to act on your behalf. 
6. When you return to Blackbird, confirm that the connection has appeared and the status is _Connected_.

![Connecting](image/README/connecting.png)

## Actions

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

## Events

- **On email created** is triggered when a new email is created in specified mail folder.
- **On email updated** is triggered when an email is updated in specified mail folder.
- **On emails received** is triggered when a new emails are received in inbox folder.
- **On emails with files attached received** is triggered when emails with file attachments are received in inbox folder.

## Example

![example](image/README/example.png)

Here, whenever an email is received we perform email content classification using one of Cohere's models and then, based on its prediction, forward the message to a specific customer support specialist.

<!-- end docs -->
