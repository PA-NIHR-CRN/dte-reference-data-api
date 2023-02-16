namespace ReferenceDataApi.Requests.Participants
{
    public class CreateParticipantAddressRequest
    {
        public string NameNumber { get; set; }
        public string Street { get; set; }
        public string LocalityTown { get; set; }
        public string PostTown { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
    }
}