namespace APIComparer
{
    using System.Collections.Generic;

    public class ChangedType
    {
        public ChangedType(TypeDiff typeDiff)
        {
            Name = typeDiff.LeftType.GetName();
            RemovedMembers = new List<RemovedMember>();

            foreach (var removedMethod in typeDiff.PublicMethodsRemoved())
            {
                RemovedMembers.Add(new RemovedMember
                {
                    IsField = false,
                    Name = removedMethod.GetName(),
                });
            }
            foreach (var matchingMember in typeDiff.MethodsChangedToNonPublic())
            {
                RemovedMembers.Add(new RemovedMember
                {
                    IsField = false,
                    Name = matchingMember.Right.GetName(),
                });
            }
            foreach (var matchingMember in typeDiff.PublicMethodsObsoleted())
            {
                RemovedMembers.Add(new RemovedMember
                {
                    IsField = false,
                    Name = matchingMember.Right.GetName(),
                });
            }

            foreach (var removedField in typeDiff.PublicFieldsRemoved())
            {
                RemovedMembers.Add(new RemovedMember
                {
                    IsField = true,
                    Name = removedField.GetName(),
                });
            }
            foreach (var matchingMember in typeDiff.FieldsChangedToNonPublic())
            {
                RemovedMembers.Add(new RemovedMember
                {
                    IsField = true,
                    Name = matchingMember.Right.GetName(),
                });
            }
            foreach (var matchingMember in typeDiff.PublicFieldsObsoleted())
            {
                RemovedMembers.Add(new RemovedMember
                {
                    IsField = true,
                    Name = matchingMember.Right.GetName(),
                });
            }
           
        }
        
        public List<RemovedMember> RemovedMembers { get; }
        public string Name { get; private set; }

        public class RemovedMember
        {
            public bool IsField { get; set; }

            public string Name { get; set; }
            
            public bool IsMethod => !IsField;
        }
    }
}