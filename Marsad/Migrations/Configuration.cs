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
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "مؤشرت الخلفية العامة" });
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "التنمية الاجتماعية والاقتصادية" });
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "النقل" });
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "البنية التحتية" });
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "المأوي" });
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "ادارة البيئة" });
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "الادارة المحلية" });
            context.Bundles.AddOrUpdate(x => x.ID, new Bundle() { Name = "نسب الرضا" });

            context.DataSourceGroups.AddOrUpdate(x => x.Code, new DataSourceGroup() { Code = 1, Name = "مجموعة المصادر الخاصة بالدراسات السكانية" });

            context.Periods.AddOrUpdate(x => x.Name, new Period() { Name = "سنوي", Year = 1 });
            context.Periods.AddOrUpdate(x => x.Name, new Period() { Name = "شهري", Month = 1 });
            context.Periods.AddOrUpdate(x => x.Name, new Period() { Name = "فصلي", Month = 3 });
            context.Periods.AddOrUpdate(x => x.Name, new Period() { Name = "نصف سنوي", Month = 6 });

            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "جهة دولية" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "تقرير دوري" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "إجتماعات مع مندوبي الإدارة" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "فاكس" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "موقع إليكتروني" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "مكتب إستشاري" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "حسابات الإستشاري" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "كتاب إحصائي" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "إسطوانة مدمجة" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "خطاب" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "إدارة حكومية" });
            context.DataSourceTypes.AddOrUpdate(x => x.Name, new DataSourceType() { Name = "مسح ميداني" });


            context.IndicatorGroups.AddOrUpdate(x => x.Code, new IndicatorGroup() { Code = "2128", Name = "المجموعة الاختبارية", Description = "الفقد في المياه" });
            context.IndicatorGroups.AddOrUpdate(x => x.Code, new IndicatorGroup() { Code = "2129", Name = "المجموعة الجديدة" });

            context.IndicatorTypes.AddOrUpdate(x => x.Name, new IndicatorType() { Name = "عالمي - رئيسي" });
            context.IndicatorTypes.AddOrUpdate(x => x.Name, new IndicatorType() { Name = "عالمي - شامل" });
            context.IndicatorTypes.AddOrUpdate(x => x.Name, new IndicatorType() { Name = "محلي" });

            context.SaveChanges();
            return;
            var dataSourceType = context.DataSourceTypes.Where(x => x.Name.Equals("جهة دولية")).FirstOrDefault();

            context.DataSources.AddOrUpdate(x => x.Code, new DataSource()
            {
                Code = 1,
                Name = "شركاء التنمية بمجلس المرصد",
                PublishDate = new DateTime(2018, 1, 1),
                NoPeriod = true,
                PeriodID = 1,
                IsHijri = true,
                DataSourceTypeID = dataSourceType.ID,
            });

            /*
             * DataSourceCode1
             *   Elements = new List<Element>(){
                    new Element() {Code="1", Name="عدد حالات الزواج في سنة",MeasureUnit="حالة زواج"},
                    new Element() {Code="2", Name="عدد حالات الطلاق في سنة",MeasureUnit="حالة طلاق"},
                    new Element() {Code="3", Name="اجمالي الانفاق السنوي بالميزانية",MeasureUnit="ريال"},
                    new Element() {Code="4", Name="الانفاق السنوي علي رواتب العاملين",MeasureUnit="ريال"}
                }
             */

            dataSourceType = context.DataSourceTypes.Where(x => x.Name.Equals("إدارة حكومية")).FirstOrDefault();
            context.DataSources.AddOrUpdate(x => x.Code,
                        new DataSource()
                        {
                            Code = 2,
                            Name = "التربية والتعليم",
                            NoPeriod = false,
                            IsHijri = true
                        });

            /*
             * DataSource COde2
             *   Elements = new List<Element>()
                            {
                                new Element(){Name="اجمالي عدد الطلاب/الطالبات",MeasureUnit="طالب/طالبة" },
                                new Element(){Name="اجمالي عدد الفصول",MeasureUnit="فصل دراسي" },
                                new Element(){Name="اجمالي عدد المعلمين",MeasureUnit="معلم" }
                            }
             */

            context.DataSources.AddOrUpdate(x => x.Code,
                      new DataSource()
                      {
                          Code = 3,
                          Name = "المسح الميدانى لمدن أمانة المنطقة الشرقية",
                          NoPeriod = true,
                          PeriodID = 2,
                          IsHijri = true
                      });

            /*
             * 3
             * Elements = new List<Element>()
                            {
                                new Element(){Name="عدد الأسر التي يقل دخلها عن 2500 ريال شهريا",MeasureUnit="عدد" },
                                new Element(){Name="إجمالي عدد الأسر",MeasureUnit="عدد" },
                                new Element(){Name="عدد النساء المعيلات في المدينة",MeasureUnit="عدد" },
                                new Element(){Name="إجمالي عدد النساء في المدينة",MeasureUnit="عدد" },
                                new Element(){Name="اسرة المستشفيات بالمدينة",MeasureUnit="سرير لكل نسمة" },
                            }
             */
            context.DataSources.AddOrUpdate(x => x.Code,
                   new DataSource()
                   {
                       Code = 4,
                       Name = "وزارة العدل",
                       NoPeriod = true,
                       PeriodID = 2,
                       IsHijri = true
                   });


        }

        private void AddUsers(Marsad.Models.ApplicationDbContext context)
        {

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };
                manager.Create(role);
            }

            //if (!context.Users.Any(u => u.UserName == "admin@emarsd.com"))
            //{
            //    var store = new UserStore<ApplicationUser>(context);
            //    var manager = new UserManager<ApplicationUser>(store);
            //    var user = new ApplicationUser { UserName = "admin@emarsd.com"};


            //    manager.Create(user, "123456");
            //    manager.AddToRole(user.Id, "admin");
            //}
        }
    }
}
