using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrequencyDistributionTable.Models;

namespace FrequencyDistributionTable.Utilities;

public class ClassChangedEventArgs : EventArgs
{
    public ClassChangedEventArgs(Class cl)
    {
        Class = cl;
    }

    public Class Class { get; }
}
