namespace APIComparer
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApiChanges
    {
        public ApiChanges(Diff diff)
        {
            RemovedTypes = new List<RemovedType>();
            ChangedTypes = new List<ChangedType>();

            if (diff is EmptyDiff)
            {
                NoLongerSupported = true;
                return;
            }

            RemovedTypes.AddRange(diff.RemovedPublicTypes()
                .Select(r => new RemovedType
                {
                    Name = r.FullName,
                    MadeInternal = false
                }));

            RemovedTypes.AddRange(diff.TypesChangedToNonPublic()
                .Select(r => new RemovedType
                {
                    Name = r.LeftType.FullName,
                    MadeInternal = true
                }));

            foreach (var typeDiff in diff.MatchingTypeDiffs)
            {
                if (!typeDiff.LeftType.IsPublic)
                {
                    continue;
                }

                if (!typeDiff.RightType.IsPublic)
                {
                    continue;
                }
                if (!typeDiff.HasDifferences())
                {
                    continue;
                }
                ChangedTypes.Add(new ChangedType(typeDiff));
            }
        }

        public bool NoLongerSupported { get; }

        public List<RemovedType> RemovedTypes { get; }

        public List<ChangedType> ChangedTypes { get; }
    }
}