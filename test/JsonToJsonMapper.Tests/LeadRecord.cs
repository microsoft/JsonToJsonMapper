using System;

namespace MipUnitTest
{
  public class LeadRecord
  {
    public string Name { get; set; }
    public bool AllowEmail { get; set; }
    public dynamic LeadScore { get; set; }
    public Guid CrmId { get; set; }
  }
}
