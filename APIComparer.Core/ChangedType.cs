namespace APIComparer
{
    using System.Collections.Generic;

    public class ChangedType
    {
        public ChangedType(TypeDiff typeDiff)
        {
            TypeChanges = new List<TypeChange>();

            Name = typeDiff.LeftType.FullName;

            if (typeDiff.TypeObsoleted())
            {
                ObsoleteDetails = typeDiff.RightType.GetObsoleteInfo();
                IsBreaking = ObsoleteDetails.AsError;
            }
            else
            {
                IsBreaking = true;
            }
        }

        public bool IsBreaking { get; set; }

        public bool Obsoleted => ObsoleteDetails != null;
        public ObsoleteInfo ObsoleteDetails { get; }

        public List<TypeChange> TypeChanges { get; }
        public string Name { get; }

        public class TypeChange
        {
            public bool IsField { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }


    }

    public class ObsoleteInfo
    {
        public bool AsError { get; set; }

        public string Message { get; set; }

        //todo: we can parse the message and give version info if possible (obsoleteex)
    }
}