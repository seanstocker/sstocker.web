using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using sstocker.core.Helpers;
using System;
using System.Collections.Generic;

namespace sstocker.hangfire
{
    public static class HangfireSetup
    {
        public static void AddServer(IServiceCollection services)
        {
            var dbConnectionString = DatabaseHelper.GetConnectionString();
            dbConnectionString += "Database=Hangfire;";

            services.AddHangfire(c => c
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(dbConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true
                }));
            services.AddHangfireServer();
        }

        public static void AddDashboard(IApplicationBuilder app)
        {
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = false,
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        Users = new List<BasicAuthAuthorizationUser>
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = ConfigurationHelper.GetConfiguration("HangfireLogin"),
                                PasswordClear = ConfigurationHelper.GetConfiguration("HangfirePassword")
                            }
                        }
                    })
                }
            });
        }
    }
}
