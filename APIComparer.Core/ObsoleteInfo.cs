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

        public string TargetVersion
        {
            get
            {
                if (AsError)
                {
                    return "Current";
                }

                var start = RawMessage.IndexOf(ERROR_FROM_VERSION);

                if (start < 0)
                {
                    return "Non specified future version";
                }

                return RawMessage.Substring(start+ERROR_FROM_VERSION.Length, 6).Trim();
            }
        }

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

        const string ERROR_FROM_VERSION = "Will be treated as an error from version";
        const string REMOVE_IN_VERSION = "Will be removed in version";
    }
}