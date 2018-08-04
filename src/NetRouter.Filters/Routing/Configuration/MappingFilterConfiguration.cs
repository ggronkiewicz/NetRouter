namespace NetRouter.Filters.Routing.Configuration
{
    public class MappingFilterConfiguration
    {
        public string Type { get; set; }

        public override int GetHashCode()
        {
            return this.Type != null ? this.Type.GetHashCode() : 0;
        }
    }
}
