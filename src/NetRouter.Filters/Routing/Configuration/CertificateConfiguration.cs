namespace NetRouter.Configuration.Routing
{
    public class CertificateConfiguration
    {
        public string FilePath { get; set; }

        public string Password { get; set; }

        public override int GetHashCode()
        {
            return string.Concat(this.FilePath, this.Password).GetHashCode();
        }
    }
}