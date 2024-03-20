namespace Pitstop.Models
{
    public class HomepageModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        public List<Section1Media> Section1Medias { get; set; }
        public List<Section2Media> Section2Medias { get; set; }
        public List<FeaturedItem> FeaturedItems { get; set; }
        public List<ProductMedia> ProductMedias { get; set; }
        public List<OurStorys> OurStorys { get; set; }
        public List<Testimonial> Testimonial{ get; set; }
        public List<Cart> Cart{ get; set; }
    }
}
