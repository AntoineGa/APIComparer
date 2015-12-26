namespace APIComparer
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApiChanges
    {
        public bool NoLongerSupported { get; }
        public List<ApiChange> BreakingChanges { get; }
      
        public ApiChanges(Diff diff)
        {
            BreakingChanges = new List<ApiChange>();

            if (diff is EmptyDiff)
            {
                NoLongerSupported = true;
                return;
            }
           
            BreakingChanges.AddRange(diff.RemovedPublicTypes().Select(ApiChange.FromRemovedType));
            BreakingChanges.AddRange(diff.TypesChangedToNonPublic().Select(td=>ApiChange.FromRemovedType(td.LeftType)));


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
                BreakingChanges.Add(new ApiChange(typeDiff));
            }
        }
    }
}