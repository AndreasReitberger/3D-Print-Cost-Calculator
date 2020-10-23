namespace PrintCostCalculator3d
{
    public class ApplicationViewInfo
    {
        public ApplicationName Name { get; set; }
        public bool IsVisible { get; set; }

        public ApplicationViewInfo()
        {

        }

        public ApplicationViewInfo(ApplicationName name)
        {
            Name = name;
            IsVisible = true;
        }
    }
}
