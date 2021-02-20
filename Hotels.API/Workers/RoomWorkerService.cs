using System.Xml.Serialization;
using System.ComponentModel;
using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Hotels.API.Contexts;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Hotels.API.Workers
{
    public class RoomWorkerService : BackgroundService
    {
        private ILogger<RoomWorkerService> _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        private HotelApiDbContext _dbContext;

        public RoomWorkerService(ILogger<RoomWorkerService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            var scope = _scopeFactory.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<HotelApiDbContext>();

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // definitions 

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_dbContext == null)
                {
                    var scope = _scopeFactory.CreateScope();
                    _dbContext = scope.ServiceProvider.GetRequiredService<HotelApiDbContext>();
                }


                var migratingrecords = await _dbContext.Rooms
                                                      .Where(room => !room.IsMigrate).ToListAsync();

                // Task.Run(() => 
                // {
                //     Thread.Sleep(5000);
                //     Console.WriteLine("Sub Task Runing");
                // });


                foreach (var record in migratingrecords)
                {
                    // do Work

                    record.IsMigrate = true;
                }

                if (_dbContext.ChangeTracker.HasChanges())
                    await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Worker Runing");



                await Task.Delay(3000, stoppingToken);

            }


        }
    }
}
