using Lazurite.Shared;
using System;

namespace LazuriteMobile.App.Controls
{
    public interface ISelectable
    {
        bool Selected { get; set; }
        bool Selectable { get; set; }
        event EventsHandler<object> SelectionChanged;
    }
}
