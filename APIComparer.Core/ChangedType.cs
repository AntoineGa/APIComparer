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

            foreach (var removedMethod in typeDiff.PublicMethodsRemoved())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = false,
                    Name = removedMethod.GetName(),
                    Description = "Method removed"
                });
            }
            foreach (var matchingMember in typeDiff.MethodsChangedToNonPublic())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = false,
                    Name = matchingMember.Right.GetName(),
                    Description = "Method no longer public"
                });
            }
            foreach (var removedField in typeDiff.PublicFieldsRemoved())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = true,
                    Name = removedField.GetName(),
                    Description = "Field removed"
                });
            }
            foreach (var matchingMember in typeDiff.FieldsChangedToNonPublic())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = true,
                    Name = matchingMember.Right.GetName(),
                    Description = "Field no longer public"
                });
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

            public bool IsMethod => !IsField;
        }
    }

    public class ObsoleteInfo
    {
        public bool AsError { get; set; }

        public string Message { get; set; }

        //todo: we can parse the message and give version info if possible (obsoleteex)
    }
}