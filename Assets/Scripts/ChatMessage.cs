using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessage
{
    public string SenderDisplayName { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }

    public ChatMessage(string senderName, string content, DateTime created, DateTime updated)
    {
        SenderDisplayName = senderName;
        Content = content;
        Created = created;
        LastUpdated = updated;
    }
}
