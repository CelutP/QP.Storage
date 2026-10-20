using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace QP.Storage.WebApp.Settings
{
    public class CorsSettings
    {
        /// <summary>
        /// Включает выдачу CORS-заголовков. По умолчанию выключено: заголовки не добавляются
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Белый список origin'ов (например, https://example.com), которым разрешены кросс-доменные запросы
        /// </summary>
        public string[] AllowedOrigins { get; set; } = [];

        /// <summary>
        /// Разрешить запросы с credentials (cookies, Authorization)
        /// </summary>
        public bool AllowCredentials { get; set; }

        /// <summary>
        /// Время кэширования preflight-ответа браузером, в секундах
        /// </summary>
        public int PreflightMaxAgeSeconds { get; set; } = 3600;

        public static CorsSettings Load(IConfiguration section)
        {
            CorsSettings settings = section.Get<CorsSettings>() ?? new CorsSettings();
            settings.Normalize();
            settings.Validate();
            return settings;
        }

        private void Normalize()
        {
            AllowedOrigins = (AllowedOrigins ?? [])
                .Select(origin => origin?.Trim().TrimEnd('/'))
                .Where(origin => !string.IsNullOrEmpty(origin))
                .ToArray();
        }

        private void Validate()
        {
            if (Enabled && AllowedOrigins.Length == 0)
            {
                throw new InvalidOperationException("Cors:Enabled is true, but Cors:AllowedOrigins is empty");
            }
        }
    }
}
