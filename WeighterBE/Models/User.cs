namespace WeighterBE.Models
{
    public class User
    {
        #region Properties
        
        public int Id { get; set; }
        
        public string? Username { get; set; }
        
        public string? Email { get; set; }
     
        public string? Password { get; set; }
     
        #endregion
    }
}
