namespace ModernRecrut.MVC.Interfaces.Documents
{
    public interface IVerificateurTypeFichier
    {
        public bool VerifyFileType(IFormFile fileData);
    }
}
