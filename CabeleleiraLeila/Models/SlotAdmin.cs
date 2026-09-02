using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CabeleleiraLeila.Models
{
    public partial class SlotAdmin : ObservableObject
    {
        [ObservableProperty] private string hora;
        [ObservableProperty] private bool isOcupado;
        [ObservableProperty] private bool isSelected;

        public Color CorFundo => IsSelected ? Color.FromArgb("#6C8EAD") : (IsOcupado ? Color.FromArgb("#FEF2F2") : Colors.Transparent);
        public Color CorTexto => IsSelected ? Colors.White : (IsOcupado ? Color.FromArgb("#EF4444") : Color.FromArgb("#263238"));
        public Color CorBorda => IsSelected ? Color.FromArgb("#6C8EAD") : (IsOcupado ? Color.FromArgb("#EF4444") : Color.FromArgb("#6C8EAD"));

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(IsSelected) || e.PropertyName == nameof(IsOcupado))
            {
                OnPropertyChanged(nameof(CorFundo));
                OnPropertyChanged(nameof(CorTexto));
                OnPropertyChanged(nameof(CorBorda));
            }
        }
    }
}
