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
                writer.WriteLine();

                foreach (var removedType in apiChanges.RemovedTypes)
                {
                    WriteRemovedType(writer, removedType, 3);
                }
            }

            if (apiChanges.ChangedTypes.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## Types with removed members");
                writer.WriteLine();

                foreach (var changedType in apiChanges.ChangedTypes)
                {
                    WriteChangedType(writer, changedType, 3);
                }
            }

        }

        static void WriteRemovedType(TextWriter writer, RemovedType removedType, int headingSize)
        {
            writer.WriteLine($"{new string('#', headingSize)} {removedType.Name}");

            var upgradeInstructions = removedType.UpgradeInstructions ?? "No upgrade instructions provided.";

            writer.WriteLine(upgradeInstructions);
            writer.WriteLine();
        }

        static void WriteChangedType(TextWriter writer, ChangedType changedType, int headingSize)
        {
            writer.WriteLine($"{new string('#', headingSize)} {changedType.Name}");

            var removedFields = changedType.RemovedMembers.Where(tc => tc.IsField)
                .ToList();

            if (removedFields.Any())
            {
                writer.WriteLine($"{new string('#', headingSize + 1)} Removed fields");
                writer.WriteLine();

                foreach (var typeChange in removedFields)
                {
                    WriteRemovedMember(writer, typeChange);
                }

                writer.WriteLine();
            }

            var removedMethods = changedType.RemovedMembers.Where(tc => tc.IsMethod)
              .ToList();

            if (removedMethods.Any())
            {
                writer.WriteLine($"{new string('#', headingSize + 1)} Removed methods");
                writer.WriteLine();

                foreach (var typeChange in removedMethods)
                {
                    WriteRemovedMember(writer, typeChange);
                }

                writer.WriteLine();
            }
        }

        static void WriteRemovedMember(TextWriter writer, ChangedType.RemovedMember removedMember)
        {
            var upgradeInstructions = removedMember.UpgradeInstructions ?? "No upgrade instructions provided.";
            writer.WriteLine($"* `{removedMember.Name}` - {upgradeInstructions}");
        }
    }
}