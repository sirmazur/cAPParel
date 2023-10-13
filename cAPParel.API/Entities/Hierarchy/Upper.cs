namespace cAPParel.API.Entities.Hierarchy
{
    public abstract class Upper : Apparel
    {

        Size Size { get; set; }
    }
    public enum Size
    {
        XS,
        S,
        M,
        L,
        XL,
        XXL
    }
}
