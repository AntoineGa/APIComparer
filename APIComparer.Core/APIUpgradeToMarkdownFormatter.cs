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

            var breakingChanges = apiChanges.ChangedTypes.Where(ct => ct.IsBreaking).ToList();

            if (breakingChanges.Any() || apiChanges.RemovedTypes.Any())
            {
                writer.WriteLine();
                writer.WriteLine("# Breaking changes");
                writer.WriteLine();

            }

            if (apiChanges.RemovedTypes.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## Types is no longer available");
                writer.WriteLine();
                foreach (var type in apiChanges.RemovedTypes)
                {
                    writer.WriteLine($"- `{type.Name}`");
                }
                writer.WriteLine();
            }

            if (breakingChanges.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## Changed types");
                writer.WriteLine();
                foreach (var changedType in breakingChanges)
                {
                    WriteChangedType(writer, changedType);
                }
            }

            var nonBreakingChanges = apiChanges.ChangedTypes.Where(ct => !ct.IsBreaking).ToList();
            if (nonBreakingChanges.Any())
            {
                writer.WriteLine();
                writer.WriteLine("# Non breaking changes");
                writer.WriteLine();

                foreach (var changedType in nonBreakingChanges)
                {
                    WriteChangedType(writer, changedType);
                }
            }
        }

        static void WriteChangedType(TextWriter writer, ChangedType changedType)
        {
            writer.WriteLine($"### {HttpUtility.HtmlEncode(changedType.Name)}");

            if (changedType.Obsoleted)
            {
                var obsoleteType = changedType.ObsoleteDetails.AsError ? "Error" : "Warning";

                writer.WriteLine($"Obsoleted with {obsoleteType} - {changedType.ObsoleteDetails.Message}");
            }

            var removedFields = changedType.TypeChanges.Where(tc => tc.IsField)
                .ToList();

            if (removedFields.Any())
            {
                writer.WriteLine("#### Removed fields");
                writer.WriteLine();

                foreach (var typeChange in removedFields)
                {
                    writer.WriteLine($"- `{typeChange.Name}`");
                }

                writer.WriteLine();
            }

            var removedMethods = changedType.TypeChanges.Where(tc => tc.IsMethod)
              .ToList();

            if (removedMethods.Any())
            {
                writer.WriteLine("#### Removed methods");
                writer.WriteLine();

                foreach (var typeChange in removedMethods)
                {
                    writer.WriteLine($"- `{typeChange.Name}`");
                }

                writer.WriteLine();
            }
        }
    }
}