using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ScheduleWebApp.Services;

namespace ScheduleWebApp
{
    public class Program
    {
        public static Types.ModelConfigJson.Root MainConfigJson { get; set; }





        public static WorkerExamsSiteSmolApo WorkerExamsSiteSmolApo { get; private set; }
        public static WorkerChangeScheduleSiteSmolApo WorkerChangeScheduleSiteSmolApo { get; private set; }
        public static WorkerScheduleSiteSmolApo WorkerScheduleSiteSmolApo { get; private set; }
        public static void Main(string[] args)
        {
            try
            {
                Init();

               


                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            Console.ReadLine();
        }



        private static void LoadingMainConfigJson()
        {
            var nameFileConfigJson = "Config.json";

            if (System.IO.File.Exists(nameFileConfigJson))
            {
                try
                {
                    string stringDataConfig = System.IO.File.ReadAllText(nameFileConfigJson);

                    var root = JsonConvert.DeserializeObject<Types.ModelConfigJson.Root>(stringDataConfig);

                    MainConfigJson = root;
                }
                catch (Exception error)
                {
                    DeriLibrary.Console.Worker.NotifyErrorMessageCall(error);
                }
            }
            else new Exception($"Нет файла {nameFileConfigJson}");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<KestrelServerOptions>(context.Configuration.GetSection("Kestrel"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseWebRoot("static");
                    webBuilder.UseUrls($"http://0.0.0.0:7040/");


                    webBuilder.UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Loopback, 7040); //HTTP port

                    });

                    /*webBuilder.UseKestrel(opts =>
                    {
                        opts.Listen(IPAddress.Broadcast, port: 7040);
                    });*/
                });


        private static void Init()
        {
            LibrarySchedule.Services.DateBase.Worker.Init();

            LoadingMainConfigJson();

            DeriLibrary.Console.Worker.ColorTheme = false;

            //WorkerDB = new Services.WorkerDB();




            //BackgroundWorker.ClearingAnOutdatedScheduleChange();

            //BackgroundWorker.FillDayOfWeekForPairDateTime();



            WorkerScheduleSiteSmolApo = new WorkerScheduleSiteSmolApo(MainConfigJson.WorkersSiteSmolApo.WorkerSchedule);
            //WorkerScheduleSiteSmolApo.Start();

            WorkerChangeScheduleSiteSmolApo = new WorkerChangeScheduleSiteSmolApo(MainConfigJson.WorkersSiteSmolApo.WorkerChangeSchedule);
            //WorkerChangeScheduleSiteSmolApo.Start();

            WorkerExamsSiteSmolApo = new WorkerExamsSiteSmolApo(MainConfigJson.WorkersSiteSmolApo.WorkerExams);
            //WorkerExamsSiteSmolApo.Start();

        }
    }
}
