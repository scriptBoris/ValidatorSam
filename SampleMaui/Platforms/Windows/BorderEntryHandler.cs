using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using SampleMaui.Controls.Atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMaui.Platforms.Windows;

public class BorderEntryHandler : EntryHandler
{
    private static readonly PropertyMapper<BorderEntry, BorderEntryHandler> BorderlessEntryMapper = new(Mapper)
    {
		[nameof(BorderEntry.BorderColor)] = MapBorderColor,
    };

    public BorderEntryHandler() : base(BorderlessEntryMapper)
    {
    }

    private static void MapBorderColor(BorderEntryHandler handler, BorderEntry entry)
    {
        handler.PlatformView.BorderBrush = entry.BorderColor.ToPlatform();
    }
}
