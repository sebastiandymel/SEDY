namespace IMC2SpeechmapTestClient.Libraries.IMC.DataTypes
{
    class SiiData
    {
        public Sideable<int?> AidedSii { get; set; } = new Sideable<int?>();

        public Sideable<int?> UnaidedSii { get; set; } = new Sideable<int?>();
    }
}
