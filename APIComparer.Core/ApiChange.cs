namespace APIComparer
{
    using System.Collections.Generic;
    using Mono.Cecil;

    public class ApiChange
    {
        ApiChange()
        {
            TypeChanges = new List<TypeChange>();
        }


        public ApiChange(TypeDiff typeDiff):this()
        {
         
            Name = typeDiff.LeftType.GetName();

            if (typeDiff.TypeObsoleted())
            {
                ObsoleteDetails = typeDiff.RightType.GetObsoleteInfo();
                Reason = ApiChangeReason.Obsoleted;
                return;
            }

            Reason = ApiChangeReason.FieldsOrMethodsRemoved;

            foreach (var removedMethod in typeDiff.PublicMethodsRemoved())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = false,
                    Name = removedMethod.GetName(),
                });
            }
            foreach (var matchingMember in typeDiff.MethodsChangedToNonPublic())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = false,
                    Name = matchingMember.Right.GetName(),
                });
            }
            foreach (var removedField in typeDiff.PublicFieldsRemoved())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = true,
                    Name = removedField.GetName(),
                });
            }
            foreach (var matchingMember in typeDiff.FieldsChangedToNonPublic())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = true,
                    Name = matchingMember.Right.GetName(),
                });
            }
            foreach (var matchingMember in typeDiff.PublicFieldsObsoleted())
            {
                TypeChanges.Add(new TypeChange
                {
                    IsField = true,
                    Name = matchingMember.Right.GetName(),
                    ObsoleteDetails = matchingMember.Right.GetObsoleteInfo()
                });
            }
           
        }

        public ApiChangeReason Reason { get; private set; }

        public enum ApiChangeReason
        {
            Removed = 0,
            Obsoleted = 1,
            FieldsOrMethodsRemoved = 2
        }

        public bool Obsoleted => ObsoleteDetails != null;
        public ObsoleteInfo ObsoleteDetails { get; }

        public List<TypeChange> TypeChanges { get; }
        public string Name { get; private set; }

        public class TypeChange
        {
            public bool IsField { get; set; }

            public string Name { get; set; }
            
            public bool IsMethod => !IsField;

            public bool Obsoleted => ObsoleteDetails != null;
            public ObsoleteInfo ObsoleteDetails { get; set; }

        }

        public static ApiChange FromRemovedType(TypeDefinition td)
        {
            return new ApiChange
            {
                Name = td.GetName(),
                Reason = ApiChangeReason.Removed
            };
        }
    }

    public class ObsoleteInfo
    {
        public bool AsError { get; set; }

        public string Message { get; set; }

        //todo: we can parse the message and give version info if possible (obsoleteex)
    }
}