﻿using Microsoft.Extensions.Logging;
using Oiski.School.THOP.App.Services;
using Oiski.School.THOP.App.ViewModels;
using Oiski.School.THOP.App.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Oiski.School.THOP.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp(true)
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<CacheService>();
            builder.Services.AddSingleton<HumidexMauiService>();
            builder.Services.AddSingleton<PeripheralMauiService>();
            builder.Services.AddSingleton<HTTPService>();
            builder.Services.AddSingleton<GraphPage>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<GraphViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}