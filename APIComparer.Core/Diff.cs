namespace APIComparer
{
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    public class Diff
    {
        public IEnumerable<TypeDiff> TypesChangedToNonPublic()
        {
            return MatchingTypeDiffs.Where(x => !x.RightType.IsPublic && (x.LeftType.IsPublic && !x.LeftType.HasObsoleteAttribute()));
        }

        public IEnumerable<TypeDefinition> RemovedPublicTypes()
        {
            return LeftOrphanTypes.Where(x => x.IsPublic && !x.HasObsoleteAttribute());
        }

        public List<TypeDefinition> LeftOrphanTypes;
        public List<TypeDiff> MatchingTypeDiffs;
        public List<TypeDefinition> RightOrphanTypes;
    }

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
                .Select(r=>new RemovedType
                {
                    Name = r.Name,
                    IsBreaking = true,
                    MadeInternal = false
                }));
            RemovedTypes.AddRange(diff.TypesChangedToNonPublic()
                   .Select(r => new RemovedType
                   {
                       Name = r.LeftType.Name,
                       IsBreaking = true,
                       MadeInternal = true
                   }));
        }

        public bool NoLongerSupported { get; set; }

        public List<RemovedType> RemovedTypes { get; set; }

        public List<ChangedType> ChangedTypes { get; set; }

    }
    public class ApiChange
    {
        public bool IsBreaking { get; set; }
    }

    public class ChangedType : ApiChange
    {
        public bool Obsoleted { get; set; }
        public ObsoleteInfo ObsoleteDetails { get; set; }

        public List<TypeChange> TypeChanges { get; set; }
        public string Name { get; set; }

        public class TypeChange
        {   
            public bool IsField { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }

        public class ObsoleteInfo
        {
            public bool AsError { get; set; }
            public string Message { get; set; }

            //todo: we can parse the message and give version info if possible (obsoleteex)
        }
    }


    public class RemovedType : ApiChange
    {
        public bool MadeInternal { get; set; }
        public string Name { get; set; }
    }

    public class AddedType : ApiChange
    {
    }
}