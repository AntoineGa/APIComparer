namespace APIComparer
{
    using System.IO;
    using System.Linq;

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
                writer.WriteLine("## The following types are no longer available");

                foreach (var removedType in apiChanges.RemovedTypes)
                {
                    WriteRemovedType(writer, removedType);
                }
            }

            if (apiChanges.ChangedTypes.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## Types with removed members");
                writer.WriteLine();

                foreach (var changedType in apiChanges.ChangedTypes)
                {
                    WriteChangedType(writer, changedType);
                }
            }
        }

        static void WriteRemovedType(TextWriter writer, RemovedType removedType)
        {
            writer.WriteLine($"### {removedType.Name}");

            var upgradeInstructions = removedType.UpgradeInstructions ?? "No upgrade instructions provided.";

            writer.WriteLine(upgradeInstructions);
        }

        static void WriteChangedType(TextWriter writer, ChangedType changedType)
        {
            writer.WriteLine($"### {changedType.Name}");

            var removedFields = changedType.RemovedMembers.Where(tc => tc.IsField)
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

            var removedMethods = changedType.RemovedMembers.Where(tc => tc.IsMethod)
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