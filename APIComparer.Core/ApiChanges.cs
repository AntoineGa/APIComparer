namespace APIComparer
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApiChanges
    {
        public static ApiChanges FromDiff(Diff diff)
        {
            var removedTypes = diff.LeftOrphanTypes
                .Where(t => t.IsPublic &&
                            !t.IsObsoleteWithError())
                .Select(td => new RemovedType(td))
                .ToList();

            var typesChangedToNonPublic = diff.MatchingTypeDiffs
                .Where(td => td.LeftType.IsPublic &&
                            !td.RightType.IsPublic &&
                            !td.LeftType.IsObsoleteWithError())
                .Select(td => new RemovedType(td.RightType, td.LeftType.HasObsoleteAttribute() ? td.LeftType.GetObsoleteInfo() : null));

            var obsoletedTypes = diff.MatchingTypeDiffs
                .Where(td => td.LeftType.IsPublic &&
                             td.RightType.IsPublic &&
                             !td.LeftType.HasObsoleteAttribute() &&
                             td.RightType.HasObsoleteAttribute())
                .Select(td => new RemovedType(td.RightType, td.RightType.GetObsoleteInfo()));

            removedTypes.AddRange(typesChangedToNonPublic);
            removedTypes.AddRange(obsoletedTypes);
            return new ApiChanges(removedTypes, new List<TypeDiff>());
        }
        public bool NoLongerSupported { get; }
        public List<RemovedType> RemovedTypes { get; }
        public List<ChangedType> ChangedTypes { get; }

        public ApiChanges(List<RemovedType> removedTypes, List<TypeDiff> typeDiffs)
        {
            RemovedTypes = removedTypes;
            ChangedTypes = new List<ChangedType>();

            //if (diff is EmptyDiff)
            //{
            //    NoLongerSupported = true;
            //    return;
            //}

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