namespace APIComparer
{
    using System.Collections.Generic;

    public class ChangedType
    {
        public List<RemovedMember> RemovedMembers { get; }
        public string Name { get; private set; }

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
                var obsoleteInfo = matchingMember.Right.GetObsoleteInfo();

                RemovedMembers.Add(new RemovedMember
                {
                    IsField = false,
                    Name = matchingMember.Right.GetName(),
                    UpgradeInstructions = obsoleteInfo.Message,
                    TargetVersion = obsoleteInfo.TargetVersion
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
                var obsoleteInfo = matchingMember.Right.GetObsoleteInfo();

                RemovedMembers.Add(new RemovedMember
                {
                    IsField = true,
                    Name = matchingMember.Right.GetName(),
                    UpgradeInstructions = obsoleteInfo.Message,
                    TargetVersion = obsoleteInfo.TargetVersion
                });
            }
           
        }
      
        public class RemovedMember
        {
            public RemovedMember()
            {
                TargetVersion = "Current";
            }
            public string UpgradeInstructions { get; set; }

            public bool IsField { get; set; }

            public string Name { get; set; }
            
            public bool IsMethod => !IsField;

            public string TargetVersion { get; set; }
        }
    }
}