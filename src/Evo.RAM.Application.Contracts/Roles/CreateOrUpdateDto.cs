namespace Evo.RAM.Roles
{
    public class CreateOrUpdateDto 
    {
     
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public virtual double? Sort { get; set; }

        public RoleType Type { get; set; }

        public string Description { get; set; }
    }
}
