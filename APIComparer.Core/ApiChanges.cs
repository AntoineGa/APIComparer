namespace APIComparer
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApiChanges
    {
        public static List<ApiChanges> FromDiff(Diff diff)
        {
            //if (diff is EmptyDiff)
            //{
            //    return new List<ApiChanges>(new ApiChanges());
            //}

            var removedTypes = diff.LeftOrphanTypes
                .Where(t => t.IsPublic &&
                            !t.IsObsoleteWithError())
                .Select(td => new RemovedType(td))
                .ToList();

            var typesChangedToNonPublic = diff.MatchingTypeDiffs
                .Where(td => td.LeftType.IsPublic &&
                            !td.RightType.IsPublic &&
                            !td.LeftType.IsObsoleteWithError())
                .Select(td => new RemovedType(td.RightType, td.LeftType.HasObsoleteAttribute() ? td.LeftType.GetObsoleteInfo() : null))
                .ToList();

            var publicTypeDiffs = diff.MatchingTypeDiffs
                .Where(td => td.LeftType.IsPublic &&
                             td.RightType.IsPublic)
                .ToList();

            var obsoletedTypes = publicTypeDiffs
                .Where(td => !td.LeftType.IsObsoleteWithError() &&
                             td.RightType.HasObsoleteAttribute())
                .Select(td => td.RightType)
                .ToList();

            var typesWithDiffs = publicTypeDiffs
              .Where(td => !td.LeftType.IsObsoleteWithError() &&
                            !td.RightType.HasObsoleteAttribute() &&
                            td.HasDifferences())
              .ToList();

            //var currentDiffs = publicTypeDiffs
            //  .Where(td => !td.PublicMethodsRemoved())
            //  .ToList();



            var currentObsoletes = obsoletedTypes
                .Where(o => o.IsObsoleteWithError())
                .Select(td => new RemovedType(td, td.GetObsoleteInfo()))
                .ToList();

            removedTypes.AddRange(typesChangedToNonPublic);
            removedTypes.AddRange(currentObsoletes);

            var result = new List<ApiChanges>();
            result.Add(new ApiChanges("Current", removedTypes, new List<TypeDiff>()));

            var futureObsoletes = obsoletedTypes
                .Where(o => !o.IsObsoleteWithError())
                .Select(td =>new
                {
                    Version = td.GetObsoleteInfo().TargetVersion,
                    RemovedType = new RemovedType(td, td.GetObsoleteInfo())
                })
                .GroupBy(rt=>rt.Version);

            foreach (var futureObsolete in futureObsoletes)
            {
                result.Add(new ApiChanges(futureObsolete.Key, futureObsolete.Select(fo=>fo.RemovedType).ToList(), new List<TypeDiff>()));
            }

            return result;
        }

        public string Version { get; }
        public bool NoLongerSupported { get; }
        public List<RemovedType> RemovedTypes { get; }
        public List<ChangedType> ChangedTypes { get; }

        ApiChanges(string version, List<RemovedType> removedTypes, List<TypeDiff> typeDiffs)
        {
            Version = version;
            RemovedTypes = removedTypes;
            ChangedTypes = new List<ChangedType>();

            foreach (var typeDiff in typeDiffs)
            {
                if (!typeDiff.HasDifferences())
                {
                    continue;
                }

                ChangedTypes.Add(new ChangedType(typeDiff));
            }
        }
    }
}