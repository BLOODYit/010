namespace Marsad.Migrations
{
    using Marsad.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Marsad.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Marsad.Models.ApplicationDbContext context)
        {
            AddStartData(context);
            AddUsers(context);

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }

        private void AddStartData(Marsad.Models.ApplicationDbContext context)
        {
            if (!context.Bundles.Any(x => x.Name.Equals("مؤشرت الخلفية العامة")))
            {
                context.Bundles.Add(new Bundle() { Name = "مؤشرت الخلفية العامة" });
                context.Bundles.Add(new Bundle() { Name = "التنمية الاجتماعية والاقتصادية" });
                context.Bundles.Add(new Bundle() { Name = "النقل" });
                context.Bundles.Add(new Bundle() { Name = "البنية التحتية" });
                context.Bundles.Add(new Bundle() { Name = "المأوي" });
                context.Bundles.Add(new Bundle() { Name = "ادارة البيئة" });
                context.Bundles.Add(new Bundle() { Name = "الادارة المحلية" });
                context.Bundles.Add(new Bundle() { Name = "نسب الرضا" });

                context.DataSourceGroups.Add(new DataSourceGroup() { Code = 1, Name = "مجموعة المصادر الخاصة بالدراسات السكانية" });

                context.Periods.Add(new Period() { Name = "سنوي", Year = 1 });
                context.Periods.Add(new Period() { Name = "شهري", Month = 1 });
                context.Periods.Add(new Period() { Name = "فصلي", Month = 3 });
                context.Periods.Add(new Period() { Name = "نصف سنوي", Month = 6 });

                context.DataSourceTypes.Add(new DataSourceType() { Name = "جهة دولية" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "تقرير دوري" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "إجتماعات مع مندوبي الإدارة" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "فاكس" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "موقع إليكتروني" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "مكتب إستشاري" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "حسابات الإستشاري" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "كتاب إحصائي" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "إسطوانة مدمجة" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "خطاب" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "إدارة حكومية" });
                context.DataSourceTypes.Add(new DataSourceType() { Name = "مسح ميداني" });


                context.IndicatorGroups.Add(new IndicatorGroup() { Code = 2128, Name = "المجموعة الاختبارية", Description = "الفقد في المياه" });
                context.IndicatorGroups.Add(new IndicatorGroup() { Code = 2129, Name = "المجموعة الجديدة" });

                context.IndicatorTypes.Add(new IndicatorType() { Name = "عالمي - رئيسي" });
                context.IndicatorTypes.Add(new IndicatorType() { Name = "عالمي - شامل" });
                context.IndicatorTypes.Add(new IndicatorType() { Name = "محلي" });

                context.SaveChanges();
            }
            context = new ApplicationDbContext();
            var dataSourceType = context.DataSourceTypes.Where(x => x.Name.Equals("جهة دولية")).FirstOrDefault();
            if (!context.DataSources.Any(x => x.Code == 1))
            {
                context.DataSources.Add(new DataSource()
                {
                    Code = 1,
                    Name = "شركاء التنمية بمجلس المرصد",
                    PublishDate = new DateTime(2018, 1, 1),
                    NoPeriod = true,
                    PeriodID = 1,
                    IsHijri = true,
                    DataSourceTypeID = dataSourceType.ID,
                });
                dataSourceType = context.DataSourceTypes.Where(x => x.Name.Equals("إدارة حكومية")).FirstOrDefault();
                context.DataSources.Add(new DataSource()
                {
                    Code = 2,
                    Name = "التربية والتعليم",
                    NoPeriod = false,
                    IsHijri = true,
                    PublishDate = new DateTime(2018, 1, 1),
                    DataSourceTypeID = dataSourceType.ID
                });
                context.DataSources.Add(new DataSource()
                {
                    Code = 3,
                    Name = "المسح الميدانى لمدن أمانة المنطقة الشرقية",
                    NoPeriod = true,
                    PeriodID = 2,
                    IsHijri = true,
                    PublishDate = new DateTime(2018, 1, 1),
                    DataSourceTypeID = dataSourceType.ID
                });
                context.DataSources.Add(new DataSource()
                {
                    Code = 4,
                    Name = "وزارة العدل",
                    NoPeriod = true,
                    PeriodID = 2,
                    IsHijri = true,
                    PublishDate = new DateTime(2018, 1, 1),
                    DataSourceTypeID = dataSourceType.ID
                });
                context.SaveChanges();
            }
            context = new ApplicationDbContext();
            var dataSource1 = context.DataSources.Where(x => x.Code == 1).FirstOrDefault();
            var dataSource2 = context.DataSources.Where(x => x.Code == 2).FirstOrDefault();
            var dataSource3 = context.DataSources.Where(x => x.Code == 3).FirstOrDefault();
            if (!context.Elements.Any(x => x.Code==1))
            {
                context.Elements.Add(new Element() { Code = 1, Name = "عدد حالات الزواج في سنة", MeasureUnit = "حالة زواج", DataSourceID = dataSource1.ID });
                context.Elements.Add(new Element() { Code = 2, Name = "عدد حالات الطلاق في سنة", MeasureUnit = "حالة طلاق", DataSourceID = dataSource1.ID });
                context.Elements.Add(new Element() { Code = 3, Name = "اجمالي الانفاق السنوي بالميزانية", MeasureUnit = "ريال", DataSourceID = dataSource1.ID });
                context.Elements.Add(new Element() { Code = 4, Name = "الانفاق السنوي علي رواتب العاملين", MeasureUnit = "ريال", DataSourceID = dataSource1.ID });
                context.Elements.Add(new Element() { Code = 5, Name = "اجمالي عدد الطلاب/الطالبات", MeasureUnit = "طالب/طالبة", DataSourceID = dataSource2.ID });
                context.Elements.Add(new Element() { Code = 6, Name = "اجمالي عدد الفصول", MeasureUnit = "فصل دراسي", DataSourceID = dataSource2.ID });
                context.Elements.Add(new Element() { Code = 7, Name = "اجمالي عدد المعلمين", MeasureUnit = "معلم", DataSourceID = dataSource2.ID });

                context.Elements.Add(new Element() { Code = 8, Name = "عدد الأسر التي يقل دخلها عن 2500 ريال شهريا", MeasureUnit = "عدد", DataSourceID = dataSource3.ID });
                context.Elements.Add(new Element() { Code = 9, Name = "إجمالي عدد الأسر", MeasureUnit = "عدد", DataSourceID = dataSource3.ID });
                context.Elements.Add(new Element() { Code = 10, Name = "عدد النساء المعيلات في المدينة", MeasureUnit = "عدد", DataSourceID = dataSource3.ID });
                context.Elements.Add(new Element() { Code = 11, Name = "إجمالي عدد النساء في المدينة", MeasureUnit = "عدد", DataSourceID = dataSource3.ID });
                context.Elements.Add(new Element() { Code = 12, Name = "اسرة المستشفيات بالمدينة", MeasureUnit = "سرير لكل نسمة", DataSourceID = dataSource3.ID });
                context.SaveChanges();
            }

        }

        private void AddUsers(Marsad.Models.ApplicationDbContext context)
        {
            context = new ApplicationDbContext();
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var role = new IdentityRole { Name = "Admin" };
                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "Officer"))
            {
                var role = new IdentityRole { Name = "Officer" };
                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "Visitor"))
            {
                var role = new IdentityRole { Name = "Visitor" };
                manager.Create(role);
            }
            context.SaveChanges();

            if (!context.Users.Any(x => x.UserName == "admin@emarsd.com"))
            {
                var result = userManager.Create(new ApplicationUser() { UserName = "admin@emarsd.com"}, "123456");
                var user = userManager.FindByName("admin@emarsd.com");
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
