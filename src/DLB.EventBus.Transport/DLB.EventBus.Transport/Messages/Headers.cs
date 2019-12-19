namespace DLB.EventBus.Transport.Messages
{
    /// <summary>
    /// Headers.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        /// Id of the message. Either set the ID explicitly when sending a message, or assign one to the message.
        /// </summary>
        public const string MessageId = "transport-msg-id";

        public const string MessageName = "transport-msg-name";

        public const string Group = "transport-msg-group";

        /// <summary>
        /// Message value .NET type
        /// </summary>
        public const string Type = "transport-msg-type";

        public const string CorrelationId = "transport-corr-id";

        public const string SentTime = "transport-senttime";

        public const string Exception = "transport-exception";
    }
}
