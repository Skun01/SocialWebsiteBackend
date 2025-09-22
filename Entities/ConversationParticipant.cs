using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWebsite.Entities;

public class ConversationParticipant
{
  public Guid UserId { get; set; }
  [ForeignKey("UserId")]
  public User? User { get; set; }
  public Guid ConversationId { get; set; }
  [ForeignKey("ConversationId")]
  public Conversation? Conversation { get; set; }
}
