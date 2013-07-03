namespace Trackifly.Server.Models
{
    public class BasicResponseModel
    {
        public BasicResponseModel(int error, string description)
        {
            Error = error;
            Description = description;
        }

        public BasicResponseModel()
        {
        }

        public int Error { get; set; }
        public string Description { get; set; }
    }
}