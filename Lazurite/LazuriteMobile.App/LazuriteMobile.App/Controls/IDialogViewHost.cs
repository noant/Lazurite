namespace LazuriteMobile.App.Controls
{
    public interface IDialogViewHost
    {
        int Column { get; }
        int Row { get; }
        int ColumnSpan { get; }
        int RowSpan { get; }
    }
}