using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SecretsManager.Model;
using Dte.Common.Lambda.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ReferenceDataApi.Extensions
{
    public static class AwsSecretsConfigurationBuilderExtensions
    {
        private const string AwsSecretManagerSecretName = "AWS_SECRET_MANAGER_SECRET_NAME";

        public static IWebHostBuilder AddAwsSecrets(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                if (hostingContext.HostingEnvironment.IsDevelopment()) return;

                configBuilder.AddAwsSecrets();
            });
        }

        public static IHostBuilder AddAwsSecrets(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                if (hostingContext.HostingEnvironment.IsDevelopment()) return;

                configBuilder.AddAwsSecrets();
            });
        }

        public static void AddAwsSecrets(this IConfigurationBuilder configurationBuilder)
        {
            var awsSecretsName = Environment.GetEnvironmentVariable(AwsSecretManagerSecretName);

            if (string.IsNullOrWhiteSpace(awsSecretsName))
            {
                throw new Exception($"The {AwsSecretManagerSecretName} environment variable has not been set");
            }
            
            var allowedSecretNames = new[] { awsSecretsName };

            configurationBuilder.AddSecretsManager(configurator: opts =>
            {
                opts.SecretFilter = entry => HasValue(allowedSecretNames, entry);
                opts.KeyGenerator = (entry, key) => GenerateKey(allowedSecretNames, key);
            });
        }

        // Only load entries that start with any of the allowed prefixes
        private static bool HasValue(IEnumerable<string> allowedSecretNames, SecretListEntry entry)
        {
            return allowedSecretNames.Any(prefix => string.Equals(prefix, entry.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        // Strip the prefix and replace '__' with ':'
        private static string GenerateKey(IEnumerable<string> allowedSecretNames, string entryName)
        {
            return entryName[(allowedSecretNames.First(x => entryName.StartsWith(x, StringComparison.CurrentCultureIgnoreCase)).Length + 1)..].Replace("__", ":");
        }
    }
}