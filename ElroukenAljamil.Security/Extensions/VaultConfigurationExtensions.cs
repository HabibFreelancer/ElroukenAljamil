using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace ElroukenAljamil.BuildingBlocks.Security.Extensions;

public static class VaultConfigurationExtensions
{
    public static IConfigurationBuilder AddVaultSecrets(
        this IConfigurationBuilder builder,
        string environment,
        string serviceName)
    {
        var vaultAddr  = Environment.GetEnvironmentVariable("VAULT_ADDR");
        var vaultToken = Environment.GetEnvironmentVariable("VAULT_TOKEN");

        if (string.IsNullOrWhiteSpace(vaultAddr) || string.IsNullOrWhiteSpace(vaultToken))
            return builder;

        var client = new VaultClient(new VaultClientSettings(vaultAddr,
            new TokenAuthMethodInfo(vaultToken)));

        var secrets = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in new[] { "elrouken/shared", $"elrouken/{serviceName.ToLowerInvariant()}" })
        {
            try
            {
                var result = client.V1.Secrets.KeyValue.V2
                    .ReadSecretAsync(path: path, mountPoint: "secret")
                    .GetAwaiter().GetResult();

                foreach (var kv in result.Data.Data)
                    secrets[kv.Key] = kv.Value?.ToString();
            }
            catch (VaultSharp.Core.VaultApiException ex) when (ex.StatusCode == 404) { }
        }

        // Build a temporary config to read existing connection strings (with placeholders)
        var existing = builder.Build();
        var map = MapSecrets(secrets, serviceName, existing);
        builder.AddInMemoryCollection(map);

        return builder;
    }

    private static Dictionary<string, string?> MapSecrets(
        Dictionary<string, string?> secrets,
        string serviceName,
        IConfiguration existing)
    {
        var map = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        void Set(string vaultKey, string configKey)
        {
            if (secrets.TryGetValue(vaultKey, out var val) && val is not null)
                map[configKey] = val;
        }

        // Shared
        Set("JWT_SECRET",        "Jwt:Secret");
        Set("RABBITMQ_USER",     "RabbitMQ:Username");
        Set("RABBITMQ_PASSWORD", "RabbitMQ:Password");

        // Service-specific
        Set("MINIO_ACCESS_KEY",    "MinIO:AccessKey");
        Set("MINIO_SECRET_KEY",    "MinIO:SecretKey");
        Set("BREVO_SMTP_USER",     "Brevo:SmtpUser");
        Set("BREVO_SMTP_PASSWORD", "Brevo:SmtpPassword");
        Set("BREVO_API_KEY",       "Brevo:ApiKey");
        Set("ELASTIC_USER",        "Elasticsearch:Username");
        Set("ELASTIC_PASSWORD",    "Elasticsearch:Password");
        Set("HUGGINGFACE_TOKEN",   "HuggingFace:ApiToken");

        // Rebuild connection strings — replace #{PLACEHOLDER}# tokens
        secrets.TryGetValue("DB_USER",          out var dbUser);
        secrets.TryGetValue("DB_PASSWORD",       out var dbPass);
        secrets.TryGetValue("RABBITMQ_USER",     out var mqUser);
        secrets.TryGetValue("RABBITMQ_PASSWORD", out var mqPass);

        ReplaceInConnectionString(map, existing, $"ConnectionStrings:{serviceName}Db",
            ("#{DB_USER}#", dbUser), ("#{DB_PASSWORD}#", dbPass));

        ReplaceInConnectionString(map, existing, "ConnectionStrings:RabbitMQ",
            ("#{RABBITMQ_USER}#", mqUser), ("#{RABBITMQ_PASSWORD}#", mqPass));

        return map;
    }

    private static void ReplaceInConnectionString(
        Dictionary<string, string?> map,
        IConfiguration existing,
        string key,
        params (string placeholder, string? value)[] replacements)
    {
        var cs = existing[key];
        if (string.IsNullOrEmpty(cs)) return;

        foreach (var (placeholder, value) in replacements)
            if (value is not null)
                cs = cs.Replace(placeholder, value);

        map[key] = cs;
    }
}
