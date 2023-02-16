using System;
using System.Collections.Generic;
using System.Security.Claims;
using Application.Settings;
using Dte.Common.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ReferenceDataApi.Acceptance.Tests
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly List<Claim> _claims;
        public IConfiguration Configuration { get; private set; }

        public ApiWebApplicationFactory()
        {
            _claims = new List<Claim>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                // Override config
                var configurationBuilder = new ConfigurationBuilder();
                config.Sources.Clear();
                
                Configuration = configurationBuilder
                    .AddJsonFile("appsettings.json")
                    .Build();
                config.AddConfiguration(Configuration);
            });

            // Shared test setup
            builder.ConfigureTestServices(services =>
            {
                // Configuration
                var awsSettings = Configuration.GetSection(AwsSettings.SectionName).Get<AwsSettings>();
                if (awsSettings == null) throw new Exception("Could not bind the aws settings, please check configuration");
                var authenticationSettings = Configuration.GetSection(AuthenticationSettings.SectionName).Get<AuthenticationSettings>();
                if (authenticationSettings == null) throw new Exception("Could not bind the authentication settings, please check configuration");

                services.AddSingleton(awsSettings);
                services.AddSingleton(authenticationSettings);
                
                services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("AnyAuthenticatedUser", policy =>
                    {
                        policy.AuthenticationSchemes.Add("Test");
                        policy.RequireAuthenticatedUser();
                    });
                });
                
                services.AddScoped(_ => new MockAuthUser(_claims));
                
                // Stub
            });
        }

        public void AddClaims(params Claim[] claims)
        {
            foreach (var claim in claims)
            {
                _claims.RemoveAll(x => x.Type == claim.Type);
                _claims.Add(claim);
            }
        }

        public void RemoveClaim(string claimType)
        {
            _claims.RemoveAll(x => x.Type == claimType);
        }

        public void ClearClaims()
        {
            foreach (var claim in _claims)
            {
                _claims.Remove(claim);
            }
        }
    }
}