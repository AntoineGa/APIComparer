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

            if (apiChanges.BreakingChanges.Any())
            {
                writer.WriteLine();
                writer.WriteLine("# Breaking changes");
                writer.WriteLine();

            }

            if (apiChanges.BreakingChanges.Any())
            {
                foreach (var perReasonGroup in apiChanges.BreakingChanges.GroupBy(ac=>ac.Reason))
                {
                    writer.WriteLine();
                    writer.WriteLine($"## {perReasonGroup.Key}");
                    writer.WriteLine();
                    foreach (var changedType in perReasonGroup)
                    {
                        WriteChangedType(writer, changedType);
                    }

                }
            }

            //var nonBreakingChanges = apiChanges.BreakingChanges.Where(ct => !ct.IsBreaking).ToList();
            //if (nonBreakingChanges.Any())
            //{
            //    writer.WriteLine();
            //    writer.WriteLine("# Non breaking changes");
            //    writer.WriteLine();

            //    foreach (var changedType in nonBreakingChanges)
            //    {
            //        WriteChangedType(writer, changedType);
            //    }
            //}
        }

        static void WriteChangedType(TextWriter writer, ApiChange apiChange)
        {
            writer.WriteLine($"### {HttpUtility.HtmlEncode(apiChange.Name)}");

            if (apiChange.Obsoleted)
            {
                var obsoleteType = apiChange.ObsoleteDetails.AsError ? "Error" : "Warning";

                writer.WriteLine($"Obsoleted with {obsoleteType} - {apiChange.ObsoleteDetails.Message}");
            }

            var removedFields = apiChange.TypeChanges.Where(tc => tc.IsField)
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

            var removedMethods = apiChange.TypeChanges.Where(tc => tc.IsMethod)
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