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

            var removedTypesInCurrentVersion = apiChanges.RemovedTypes.Where(rt => rt.Version == "Current").ToList();
            if (removedTypesInCurrentVersion.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## The following types are no longer available");
                writer.WriteLine();

                foreach (var removedType in removedTypesInCurrentVersion)
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
                    WriteChangedType(writer, changedType);
                }
            }

            var removedTypesInFutureVersions = apiChanges.RemovedTypes
                .Where(rt => rt.Version != "Current")
                .GroupBy(rt => rt.Version).ToList();

            if (removedTypesInFutureVersions.Any())
            {
                writer.WriteLine();
                writer.WriteLine("## The following will be removed in upcoming versions");
              
                foreach (var versionGroup in removedTypesInFutureVersions)
                {
                    writer.WriteLine();
                    writer.WriteLine($"### {versionGroup.Key}");
                    writer.WriteLine();

                    foreach (var removedType in versionGroup)
                    {
                        WriteRemovedType(writer, removedType, 4);
                    }
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
                    WriteRemovedMember(writer, typeChange);
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