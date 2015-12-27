namespace APIComparer
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApiChanges
    {
        public bool NoLongerSupported { get; }
        public List<RemovedType> RemovedTypes { get; }
        public List<ChangedType> ChangedTypes { get; }

        public ApiChanges(Diff diff)
        {
            RemovedTypes = new List<RemovedType>();
            ChangedTypes = new List<ChangedType>();

            if (diff is EmptyDiff)
            {
                NoLongerSupported = true;
                return;
            }
           
            RemovedTypes.AddRange(diff.RemovedPublicTypes().Select(td=>new RemovedType(td)));
            RemovedTypes.AddRange(diff.TypesChangedToNonPublic().Select(td=>new RemovedType(td.LeftType)));
         

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
                if (typeDiff.TypeObsoleted())
                {
                    RemovedTypes.Add(new RemovedType(typeDiff.RightType));
                }
                else
                {
                    ChangedTypes.Add(new ChangedType(typeDiff));
                }
            }
        }
    }
}