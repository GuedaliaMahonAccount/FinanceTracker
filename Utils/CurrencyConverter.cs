using System;
using System.Collections.Generic;
using System.Linq;

namespace Finance.Utils
{
    public class CurrencyConverter
    {
        private readonly Dictionary<(string fromCurrency, string toCurrency, DateTime date), decimal> _exchangeRates;
        
        // Mapping pour convertir les noms complets en codes ISO
        private readonly Dictionary<string, string> _currencyCodeMapping = new Dictionary<string, string>
        {
            { "Dollars", "USD" },
            { "Shekel", "ILS" },
            { "Euro", "EUR" },
            { "USD", "USD" },
            { "ILS", "ILS" },
            { "EUR", "EUR" },
            // Ajouter les variantes avec symboles
            { "¤ Shekel", "ILS" },
            { "¤", "ILS" },
            { "$ Dollars", "USD" },
            { "$", "USD" },
            { "€ Euro", "EUR" },
            { "€", "EUR" }
        };

        public CurrencyConverter(Dictionary<(string fromCurrency, string toCurrency, DateTime date), decimal> exchangeRates)
        {
            _exchangeRates = exchangeRates;
        }

        public decimal Convert(decimal amount, string fromCurrency, string toCurrency, DateTime date)
        {
            // Normaliser les noms de devises en codes ISO
            string fromCode = GetCurrencyCode(fromCurrency);
            string toCode = GetCurrencyCode(toCurrency);
            
            if (fromCode == toCode)
                return amount;

            // Chercher d'abord les taux <= date (taux pass?s ou ? la date exacte)
            var pastRate = _exchangeRates
                .Where(rate => rate.Key.fromCurrency == fromCode && rate.Key.toCurrency == toCode && rate.Key.date <= date)
                .OrderByDescending(rate => rate.Key.date)
                .FirstOrDefault();

            // Si un taux pass? existe, l'utiliser
            if (!pastRate.Equals(default(KeyValuePair<(string, string, DateTime), decimal>)))
                return amount * pastRate.Value;

            // Sinon, chercher le taux futur le plus proche
            var futureRate = _exchangeRates
                .Where(rate => rate.Key.fromCurrency == fromCode && rate.Key.toCurrency == toCode && rate.Key.date > date)
                .OrderBy(rate => rate.Key.date)
                .FirstOrDefault();

            // Si un taux futur existe, l'utiliser
            if (!futureRate.Equals(default(KeyValuePair<(string, string, DateTime), decimal>)))
                return amount * futureRate.Value;

            // Si aucun taux n'est disponible pour cette paire de devises
            throw new Exception($"Aucun taux de change disponible pour la conversion de '{fromCurrency}' ({fromCode}) vers '{toCurrency}' ({toCode}).");
        }
        
        private string GetCurrencyCode(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return currency;
                
            // Essayer d'abord une correspondance exacte
            if (_currencyCodeMapping.TryGetValue(currency, out string code))
            {
                return code;
            }
            
            // Essayer une correspondance insensible ? la casse
            var entry = _currencyCodeMapping.FirstOrDefault(kvp => 
                string.Equals(kvp.Key, currency, StringComparison.OrdinalIgnoreCase));
            
            if (entry.Key != null)
            {
                return entry.Value;
            }
            
            // Si le code n'est pas dans le mapping, retourner tel quel
            return currency;
        }
    }
}