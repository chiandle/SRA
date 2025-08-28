namespace SRA.Models
{
    public interface ILDAPAuthenticationService
    {
        LDAPUser Login(string userName, string password);
       
    }
    
}
