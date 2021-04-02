using System;

namespace Common.Messaging.Events
{
    public class ActionRejected : IActionRejected
    {
        public ActionRejected(string code, string reason)
        {
            Code = code;
            Reason = reason;
        }

        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Code { get; }
        public string Reason { get; }
    }
}