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
    }
}
