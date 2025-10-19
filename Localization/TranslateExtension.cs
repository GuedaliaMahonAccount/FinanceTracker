using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Finance.Localization
{
    public class TranslateExtension : MarkupExtension
    {
        public string Key { get; set; }

        public TranslateExtension()
        {
        }

        public TranslateExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Key))
                return "[Key Missing]";

            // Cr?er un binding direct vers TranslationManager avec la cl? sp?cifique
            var binding = new Binding
            {
                Source = TranslationManager.Instance,
                Path = new System.Windows.PropertyPath($"[{Key}]"),
                Mode = BindingMode.OneWay
            };

            // Si on a un target provider, retourner le binding
            var targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetProvider?.TargetObject != null)
            {
                return binding.ProvideValue(serviceProvider);
            }

            // Sinon retourner la valeur directement
            return TranslationManager.Instance[Key];
        }
    }
}
