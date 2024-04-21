using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalendar.Tests
{
    public class Sv1
    {
        private static int nextId = 0;

        public int Id { get; init; } = ++nextId;

        public Sv1() { }
    }

    public class Sv2
    {
        private static int nextId = 0;

        public int Id { get; init; } = ++nextId;

        public readonly Sv1? Sv1;

        public Sv2() { Sv1 = default; }
        public Sv2(Sv1 sv1) { Sv1 = sv1; }

        public bool HasSv1()
        {
            return Sv1 != null;
        }
    }

    public class Sv3
    {
        private static int nextId = 0;

        public int Id { get; init; } = ++nextId;

        public readonly Sv1? Sv1;
        public readonly Sv2? Sv2;

        public Sv3() { Sv1 = default; Sv2 = default; }
        public Sv3(Sv1 sv1) { Sv1 = sv1; Sv2 = default; }
        public Sv3(Sv2 sv2) { Sv1 = default; Sv2 = sv2; }
        public bool HasSv1()
        {
            return Sv1 != null;
        }
        public bool HasSv2()
        {
            return Sv2 != null;
        }
    }

    public class UnitTestDI
    {
        [Fact]
        public void DITest()
        {
            var ioc = new Ioc();
            ioc.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<Sv1>()
                .AddTransient<Sv2>()
                .AddTransient<Sv3>(provider => new Sv3(provider.GetService<Sv1>()!))
                .BuildServiceProvider()
                );
            var sv2 = ioc.GetService<Sv2>();
            Assert.NotNull(sv2);
            Assert.True(sv2.HasSv1());
            Assert.Equal(1, sv2.Id);
            Assert.Equal(1, sv2.Sv1?.Id);
            var sv2_2 = ioc.GetService<Sv2>();
            Assert.NotNull(sv2_2);
            Assert.Equal(2, sv2_2.Id);
            Assert.Equal(1, sv2_2.Sv1?.Id);
            var sv2_3 = ioc.GetService<Sv2>();
            Assert.NotNull(sv2_3);
            Assert.Equal(3, sv2_3.Id);
            Assert.Equal(1, sv2_3.Sv1?.Id);
            var sv3 = ioc.GetService<Sv3>();
            Assert.NotNull(sv3);
            Assert.True(sv3.HasSv1());
            Assert.False(sv3.HasSv2());
        }
    }
}
