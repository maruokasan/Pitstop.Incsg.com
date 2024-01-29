namespace Pitstop.Models
{
    public class HomepageModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        public List<Carousel2>  LatestCarousel2{ get; set; }

    }
}