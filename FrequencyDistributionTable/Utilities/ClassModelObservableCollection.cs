using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrequencyDistributionTable.Models;

namespace FrequencyDistributionTable.Utilities;

public class ClassModelObservableCollection : ObservableCollection<ClassModel>
{
    public event EventHandler<ClassChangedEventArgs>? ClassChanged;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        base.OnCollectionChanged(args);
        // if any items in this.Items.Class change then raise the ClassChanged event
        if (args.NewItems != null)
            foreach (ClassModel item in args.NewItems)
                item.Class.PropertyChanged += item_PropertyChanged;

        if (args.OldItems != null)
            foreach (ClassModel item in args.OldItems)
                item.Class.PropertyChanged -= item_PropertyChanged;

        RecomputeCumulativeFrequencies();
    }

    private void OnItemPropertyChanged(Class sender, PropertyChangedEventArgs args)
    {
        ClassChanged?.Invoke(this, new ClassChangedEventArgs(sender));
    }

    private void item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RecomputeCumulativeFrequencies();
        OnItemPropertyChanged((Class)sender, e);
    }

    private void RecomputeCumulativeFrequencies()
    {
        // recompute cumulative frequencies
        var total = this.Select(x => x.Class.Frequency).Sum();
        var cumulativeFrequencyLess = 0;
        var cumulativeFrequencyGreater = total;
        var freqRev = this.Reverse().ToList();

        for (var i = 0; i < freqRev.Count; i++)
        {
            cumulativeFrequencyLess += Items[i].Class.Frequency;

            Items[i].CumulativeFrequencyLess = cumulativeFrequencyLess;
            Items[i].CumulativeFrequencyGreater = cumulativeFrequencyGreater;

            cumulativeFrequencyGreater -= freqRev[i].Class.Frequency;
        }

    }
}
