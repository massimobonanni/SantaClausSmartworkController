using SCSC.Core.Models;
using System;
using System.Collections.Generic;

namespace SCSC.AdminWeb.Models.Elfs
{
    public class ElfInfoViewModel
    {
        public ElfInfoViewModel()
        {

        }

        public ElfInfoViewModel(ElfInfoModel source)
        {
            this.Id = source.Id;
            this.Name = source.Name;
            this.LastUpdate = source.LastUpdate;
            this.StartWorkTime = source.StartWorkTime;
            this.EndWorkTime = source.EndWorkTime;
            this.Packages = source.Packages;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public TimeSpan StartWorkTime { get; set; }
        public TimeSpan EndWorkTime { get; set; }
        public List<PackageInfoModel> Packages { get; set; }

        public double LastHourProductivity
        {
            get => this.Packages.CalculateAverageSpeed(this.StartWorkTime, this.EndWorkTime);
        }
        public double DailyProductivity
        {
            get => this.Packages.CalculateAverageSpeed(this.StartWorkTime, this.EndWorkTime,
                null, this.EndWorkTime.Subtract(this.StartWorkTime));
        }

        public int PackagesToday
        {
            get => this.Packages.CountPackagesInDay(DateTimeOffset.Now.Date);
        }
    }
}
