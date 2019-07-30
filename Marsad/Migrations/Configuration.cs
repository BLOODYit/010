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
            //AddStartData(context);
            //AddUsers(context);

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }

        private void AddStartData(Marsad.Models.ApplicationDbContext context)
        {
            context.Bundles.AddRange(new List<Bundle>() {
                new Bundle(){Code="1",Name="مؤشرت الخلفية العامة"},
                new Bundle(){Code="2",Name="التنمية الاجتماعية والاقتصادية"},
                new Bundle(){Code="3",Name="النقل"},
                new Bundle(){Code="4",Name="البنية التحتية"},
                new Bundle(){Code="5",Name="المأوي"},
                new Bundle(){Code="6",Name="ادارة البيئة"},
                new Bundle(){Code="7",Name="الادارة المحلية"},
                new Bundle(){Code="8",Name="نسب الرضا"}
            });
            context.DataSourceGroups.AddOrUpdate(new DataSourceGroup()
            {
                Code = 1,
                Name = "مجموعة المصادر الخاصة بالدراسات السكانية"
            });
            context.Periods.AddRange(new List<Period>() {
                new Period(){ Name="سنوي",Year=1},
                new Period(){ Name="شهري",Month=1},
                new Period(){ Name="فصلي",Month=3},
                new Period(){ Name="نصف سنوي",Month=6}
            });

            context.SaveChanges();

            context.DataSourceTypes.AddRange(new List<DataSourceType>()
            {
                new DataSourceType(){ Name="جهة دولية"
                    ,DataSources = new List<DataSource>(){
                        new DataSource(){
                            Code=1,Name ="شركاء التنمية بمجلس المرصد",PublishDate=new DateTime(2018,1,1),IsPeriodic=true,PeriodID=1,IsHijri=true
                            ,Elements=new List<Element>(){
                                new Element() {Code="1", Name="عدد حالات الزواج في سنة",MeasureUnit="حالة زواج"},
                                new Element() {Code="2", Name="عدد حالات الطلاق في سنة",MeasureUnit="حالة طلاق"},
                                new Element() {Code="3", Name="اجمالي الانفاق السنوي بالميزانية",MeasureUnit="ريال"},
                                new Element() {Code="4", Name="الانفاق السنوي علي رواتب العاملين",MeasureUnit="ريال"}
                            }
                        }
                    }
                },
                new DataSourceType(){ Name="تقرير دوري"},
                new DataSourceType(){ Name="إجتماعات مع مندوبي الإدارة"},
                new DataSourceType(){ Name="فاكس"},
                new DataSourceType(){ Name="موقع إليكتروني"},
                new DataSourceType(){ Name="مكتب إستشاري"},
                new DataSourceType(){ Name="حسابات الإستشاري"},
                new DataSourceType(){ Name="كتاب إحصائي"},
                new DataSourceType(){ Name="إسطوانة مدمجة"},
                new DataSourceType(){ Name="خطاب"},
                new DataSourceType(){
                    Name="إدارة حكومية",
                    DataSources = new List<DataSource>(){
                        new DataSource(){
                            Code=2,Name="التربية والتعليم",IsPeriodic=false,IsHijri=true,
                            Elements = new List<Element>()
                            {
                                new Element(){Name="اجمالي عدد الطلاب/الطالبات",MeasureUnit="طالب/طالبة" },
                                new Element(){Name="اجمالي عدد الفصول",MeasureUnit="فصل دراسي" },
                                new Element(){Name="اجمالي عدد المعلمين",MeasureUnit="معلم" }
                            }
                        },
                          new DataSource(){
                            Code=3,Name="المسح الميدانى لمدن أمانة المنطقة الشرقية",IsPeriodic=true,PeriodID=2,IsHijri=true,
                            Elements = new List<Element>()
                            {
                                new Element(){Name="عدد الأسر التي يقل دخلها عن 2500 ريال شهريا",MeasureUnit="عدد" },
                                new Element(){Name="إجمالي عدد الأسر",MeasureUnit="عدد" },
                                new Element(){Name="عدد النساء المعيلات في المدينة",MeasureUnit="عدد" },
                                new Element(){Name="إجمالي عدد النساء في المدينة",MeasureUnit="عدد" },
                                new Element(){Name="اسرة المستشفيات بالمدينة",MeasureUnit="سرير لكل نسمة" },
                            }
                        },
                            new DataSource(){
                            Code=4,Name="وزارة العدل",IsPeriodic=true,PeriodID=2,IsHijri=true
                        },
                    }
                },
                new DataSourceType(){ Name="مسح ميداني"}
            });
            context.IndicatorGroups.AddRange(new List<IndicatorGroup>() {
                new IndicatorGroup(){Code="2128",Name="المجموعة الاختبارية",Description="الفقد في المياه"},
                new IndicatorGroup(){Code="2129",Name="المجموعة الجديدة"}
            });
            context.IndicatorTypes.AddRange(new List<IndicatorType>() {
            new IndicatorType(){Name="عالمي - رئيسي" },
            new IndicatorType(){Name="عالمي - شامل" },
            new IndicatorType(){Name="محلي" }

            });
            context.SaveChanges();
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

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "admin" };

                manager.Create(user, "123456");
                manager.AddToRole(user.Id, "admin");
            }
        }
    }
}
