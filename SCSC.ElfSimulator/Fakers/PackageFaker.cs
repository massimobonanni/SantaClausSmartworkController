using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.ElfSimulator.Fakers
{
    internal static class PackageFaker
    {
        public static PackageStartedModel PackageStarted()
        {
            var package = new PackageStartedModel();
            package.PackageId = Guid.NewGuid().ToString();
            package.Timestamp = DateTimeOffset.Now;
            package.GiftDescription = ToysFaker.Toy();
            package.KidName = $"{Faker.Name.First()} {Faker.Name.Last()}";
            return package;
        }
    }
}
