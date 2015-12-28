namespace APIComparer
{
    public class ObsoleteInfo
    {
        public ObsoleteInfo(bool asError, string message)
        {
            AsError = asError;
            RawMessage = message;
        }
        public bool AsError { get;}

        public string RawMessage { get; }

        public string Message
        {
            get
            {
                var trimStart = RawMessage.IndexOf(REMOVE_IN_VERSION);

                if (trimStart > 0)
                {
                    return RawMessage.Substring(0, trimStart).Trim();
                }

                return RawMessage;
            }
        }

        const string REMOVE_IN_VERSION = "Will be removed in version";
    }
}