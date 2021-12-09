using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCSC.AdminWeb.Models.Elfs
{
    public class ElfInfoViewModel
    {
        public enum ElfStatus
        {
            Unknown,
            Work,
            Break,
            OutOfOffice
        }

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
            this.Packages = source.Packages.GetPackagesInDay(DateTimeOffset.UtcNow.Date).ToList();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public TimeSpan StartWorkTime { get; set; }
        public TimeSpan EndWorkTime { get; set; }
        public ElfStatus Status
        {
            get
            {
                var dtNow = DateTimeOffset.UtcNow;
                if (dtNow.TimeOfDay < StartWorkTime || dtNow.TimeOfDay > EndWorkTime)
                    return ElfStatus.OutOfOffice;
                if (LastUpdate.Date != dtNow.Date)
                    return ElfStatus.Unknown;

                if (Packages == null || !Packages.Any())
                    return ElfStatus.Break;

                var currentPackage = Packages.OrderByDescending(p => p.StartTimestamp).First();

                if (currentPackage.IsOpen)
                    return ElfStatus.Work;
                else
                    return ElfStatus.Break;
            }
        }
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
