namespace APIComparer
{
    public class ObsoleteInfo
    {
        public bool AsError { get; set; }

        public string Message { get; set; }

        //todo: we can parse the message and give version info if possible (obsoleteex)
    }
}