namespace WeighterBE.Models
{
    public class Weight
    {
        #region Properties

        public int Id { get; set; }
        
        public double Value { get; set; }

        public DateTime WeightAt { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"Weight {{ Id = {Id}, Value = {Value}, WeightAt = {WeightAt} }}";
        }

        #endregion
    }
}
