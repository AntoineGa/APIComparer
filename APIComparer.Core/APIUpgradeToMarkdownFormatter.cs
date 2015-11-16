namespace APIComparer
{
    using System.IO;
    using System.Linq;
    using System.Web;

    public class APIUpgradeToMarkdownFormatter
    {
        public void WriteOut(ApiChanges apiChanges, TextWriter writer, FormattingInfo info)
        {
            if (apiChanges.NoLongerSupported)
            {
                writer.WriteLine("No longer supported");
                return;
            }

            if (apiChanges.RemovedTypes.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## The following public types is no longer available");
                writer.WriteLine();
                foreach (var type in apiChanges.RemovedTypes)
                {
                    writer.WriteLine($"- `{type.Name}`");
                }
                writer.WriteLine();
            }

            if (apiChanges.ChangedTypes.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## The following types have differences.");
                writer.WriteLine();
                foreach (var changedType in apiChanges.ChangedTypes)
                {
                    writer.WriteLine();
                    writer.Write("### {0}", HttpUtility.HtmlEncode(changedType.Name));

                    if (changedType.Obsoleted)
                    {
                        writer.Write(" - Obsoleted");
                    }

                    writer.WriteLine();

                    if (changedType.Obsoleted)
                    {
                        writer.WriteLine(changedType.ObsoleteDetails.Message);
                    }

                    foreach (var typeChange in changedType.TypeChanges)
                    {
                        writer.WriteLine();
                        writer.WriteLine($"- `{typeChange.Name}` - {typeChange.Description}");
                        writer.WriteLine();
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}