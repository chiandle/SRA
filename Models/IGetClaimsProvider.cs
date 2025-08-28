namespace SRA.Models
{
    public interface IGetClaimsProvider
    {
        string UserId { get; }
        string Struttura { get; }
        bool Responsabile { get; }
        bool Supervisore { get; }
        bool Amministratore { get; }

    }
}
