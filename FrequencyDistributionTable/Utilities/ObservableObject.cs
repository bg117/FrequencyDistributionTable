using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrequencyDistributionTable.Utilities;

public class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChangedEvent(string propertyName)
    {
        // only invoke PropertyChanged if not null
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = default)
    {
        if (propertyName == null)
            return;

        // do not waste energy setting if value is the same as field
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        RaisePropertyChangedEvent(propertyName);
    }
}