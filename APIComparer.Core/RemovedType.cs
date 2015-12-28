namespace APIComparer
{
    using Mono.Cecil;

    public class RemovedType
    {
        public RemovedType(TypeDefinition typeDefinition)
        {         
            Name = typeDefinition.GetName();

            if (typeDefinition.HasObsoleteAttribute())
            {
                var obsoleteInfo = typeDefinition.GetObsoleteInfo();
                UpgradeInstructions = obsoleteInfo.Message;
                Version = obsoleteInfo.TargetVersion;
            }
            else
            {
                Version = "Current";
            }
        }
        public string Name { get; }
        public string UpgradeInstructions { get; }

        public string Version { get; }

    }
}