namespace PieChart
{
    public interface IPieSlice
    {
        double Value { get; }
        string Name { get; }
        string ToolTip { get; }
    }
}
