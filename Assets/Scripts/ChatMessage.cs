using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessage
{
    public string MessageId { get; set; }
    public string SenderUserId { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }

    public ChatMessage(string id, string senderId, string content, DateTime created, DateTime updated)
    {
        MessageId = id;
        SenderUserId = senderId;
        Content = content;
        Created = created;
        LastUpdated = updated;
    }
}
