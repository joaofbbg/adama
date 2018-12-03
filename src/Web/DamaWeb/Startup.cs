using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using DamaWeb.Interfaces;
using DamaWeb.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Text;
using AutoMapper;
using ApplicationCore;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace DamaWeb
{
    public class Startup
    {
        private IServiceCollection _services;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // use in-memory database
            //ConfigureTestingServices(services);

            // use real database            
            services.AddDbContext<DamaContext>(options => 
                options.UseMySql(Configuration.GetConnectionString("DamaShopConnection")));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityConnection")));

            ConfigureServices(services);

        }
        public void ConfigureTestingServices(IServiceCollection services)
        {
            // use in-memory database
            services.AddDbContext<DamaContext>(c =>
                c.UseInMemoryDatabase("Catalog"));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            // use real database            
            services.AddDbContext<DamaContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DamaShopConnection")));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityConnection")));

            //services.AddHttpsRedirection(options =>
            //{
            //    options.HttpsPort = 443;
            //});

            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;                   
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];                    
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "damanojornalApp";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.LoginPath = "/Account/Signin";
                options.LogoutPath = "/Account/Signout";                
                options.Cookie.IsEssential = true;
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;               
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });


            services.AddAutoMapper();

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

            services.AddScoped<ICatalogService, CachedCatalogService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IBasketViewModelService, BasketViewModelService>();
            services.AddScoped<ICustomizeViewModelService, CustomizeViewModelService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<CatalogService>();
            services.AddScoped<IShopService,ShopService>();
            services.Configure<CatalogSettings>(Configuration);
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            services.AddSingleton<IUriComposer>(new UriComposer(Configuration.Get<CatalogSettings>()));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));           
            services.AddScoped<IMailChimpService, MailChimpService>();
            services.AddTransient<IEmailSender, EmailSender>();

            // Add memory cache services
            services.AddMemoryCache();

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Order");
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                    options.Conventions.AuthorizePage("/Basket/Checkout");
                    options.Conventions.AddPageRoute("/Category/Index", "{id}/");
                    options.Conventions.AddPageRoute("/Category/Type/Index", "{cat}/{type}");
                    options.Conventions.AddPageRoute("/Product/Index", "produto/{id}");
                    options.Conventions.AddPageRoute("/Tag/Index", "tag/{tagName}");
                    options.Conventions.AddPageRoute("/Search/Index", "procurar/{q?}");
                    options.Conventions.AddPageRoute("/Customize/Index", "personalizar/");
                    options.Conventions.AddPageRoute("/Customize/Step2", "personalizar/passo2");
                    options.Conventions.AddPageRoute("/Customize/Step3", "personalizar/passo3");
                    options.Conventions.AddPageRoute("/Customize/Result", "personalizar/resultado");
                    options.Conventions.AddPageRoute("/Privacy", "privacidade");
                    options.Conventions.AddPageRoute("/NotFound", "pagina-nao-encontrada");
                    options.Conventions.AddPageRoute("/Error", "erro");
                    options.Conventions.AddPageRoute("/Account/Signin", "conta/entrar");
                    options.Conventions.AddPageRoute("/Account/ConfirmEmail", "conta/confirmar-email");
                    options.Conventions.AddPageRoute("/Account/ForgotPassword", "conta/esqueceu-password");
                    options.Conventions.AddPageRoute("/Account/ForgotPasswordConfirmation", "conta/confirmacao-password");
                    options.Conventions.AddPageRoute("/Account/ResetPassword", "conta/recuperar-password");
                    options.Conventions.AddPageRoute("/Account/ResetPasswordConfirmation", "conta/confirmacao-recuperacao-password");
                    options.Conventions.AddPageRoute("/Account/Manage/Index", "conta/perfil");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            _services = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.UsePathBase("/loja");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();                
            }
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-PT"),
                // Formatting numbers, dates, etc.
                SupportedCultures = new[] { new CultureInfo("pt-PT") },
                // UI strings that we have localized.
                SupportedUICultures = new[] { new CultureInfo("pt-PT") }
            });

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/NotFound";
                    await next();
                }
            });

            app.UseMvc();
        }

        private void ListAllRegisteredServices(IApplicationBuilder app)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }
    }
}
