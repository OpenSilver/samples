using System;
using System.Collections.Generic;
using System.Text;
using Jamesnet.Foundation;

namespace OpenSilverShowcase.MvvmTemplate.Examples
{
    public class TextBoxViewModel : ViewModelBase
    {
        private string _address;

        public string Address
        {
            get => _address;
            set=> SetProperty(ref _address, value);
        }

        public TextBoxViewModel()
        {
            Address = "3 Rue Théophile Gautier, 92200 Neuilly-sur-Seine, Prance";
        }
    }
}
