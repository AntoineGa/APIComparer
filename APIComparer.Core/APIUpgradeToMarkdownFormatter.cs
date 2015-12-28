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
                    WriteChangedType(writer, changedType, 3);
                }
            }

            var removedTypesInFutureVersions = apiChanges.RemovedTypes
                .Where(rt => rt.Version != "Current")
                .GroupBy(rt => rt.Version).ToList();

            var changedTypesInFutureVersions = apiChanges.ChangedTypes
                .Where(ct => ct.RemovedMembers.All(rm => rm.TargetVersion != "Current"))
                .GroupBy(rt => rt.RemovedMembers.First().TargetVersion).ToList();

            if (removedTypesInFutureVersions.Any() || changedTypesInFutureVersions.Any())
            {
                var allVersions = removedTypesInFutureVersions.Select(g => g.Key)
                    .ToList();

                allVersions.AddRange(changedTypesInFutureVersions.Select(g => g.Key));

                foreach (var version in allVersions.Distinct())
                {
                    writer.WriteLine();
                    writer.WriteLine($"## Upcoming changes in - {version}");
                    writer.WriteLine();

                    var removals = removedTypesInFutureVersions.SingleOrDefault(g => g.Key == version);

                    if (removals != null)
                    {
                        writer.WriteLine("### The following types will no longer available");
                        writer.WriteLine();
                        foreach (var removedType in removals)
                        {
                            WriteRemovedType(writer, removedType, 4);
                        }
                    }

                    var changes = changedTypesInFutureVersions.SingleOrDefault(g => g.Key == version);

                    if (changes != null)
                    {
                        writer.WriteLine("### The following types will be changed");
                        writer.WriteLine();
                        foreach (var changedType in changes)
                        {
                            WriteChangedType(writer, changedType, 4);
                        }

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