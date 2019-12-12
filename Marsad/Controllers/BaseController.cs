using Marsad.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();
        protected void Log(LogAction action, object entity, string notes = "")
        {
            try
            {
                string _action = "";
                if (action == LogAction.Create)
                    _action = "إنشاء";
                else if (action == LogAction.Update)
                    _action = "تعديل";
                else if (action == LogAction.Delete)
                    _action = "حذف";
                string _entity = "";
                string _data = "";
                if (entity.GetType() == typeof(Bundle))
                {
                    _entity = "حزمة";
                    _data = string.Format("م: {0} - إسم: {1}", ((Bundle)entity).ID, ((Bundle)entity).Name);
                }
                else if (entity.GetType() == typeof(Case))
                {
                    _entity = "قضية";
                    _data = string.Format("م: {0} - إسم: {1}", ((Case)entity).ID, ((Case)entity).Name);
                }
                else if (entity.GetType() == typeof(DataSourceGroup))
                {
                    _entity = "مجموعة مصادر بيانات";
                    _data = string.Format("م: {0} - إسم: {1}", ((DataSourceGroup)entity).ID, ((DataSourceGroup)entity).Name);
                }
                else if (entity.GetType() == typeof(DataSource))
                {
                    _entity = "مصدر بيانات";
                    _data = string.Format("م: {0} - إسم: {1}", ((DataSource)entity).ID, ((DataSource)entity).Name);
                }
                else if (entity.GetType() == typeof(DataSourceType))
                {
                    _entity = "نوع مصدر بيانات";
                    _data = string.Format("م: {0} - إسم: {1}", ((DataSourceType)entity).ID, ((DataSourceType)entity).Name);
                }
                else if (entity.GetType() == typeof(Element))
                {
                    _entity = "عنصر";
                    _data = string.Format("م: {0} - إسم: {1}", ((Element)entity).ID, ((Element)entity).Name);
                }
                else if (entity.GetType() == typeof(Entity))
                {
                    _entity = "جهة";
                    _data = string.Format("م: {0} - إسم: {1}", ((Entity)entity).ID, ((Entity)entity).Name);
                }
                else if (entity.GetType() == typeof(Equation))
                {
                    _entity = "معادلة";
                    _data = string.Format("م: {0} - المؤشر: {1} لسنوات {2}", ((Equation)entity).ID, ((Equation)entity).Indicator.Name, string.Join(",", ((Equation)entity).EquationYears.Select(x => x.Year.ToString())));
                }
                
                else if (entity.GetType() == typeof(GeoArea))
                {
                    if (((GeoArea)entity).Type == "Bundle")
                    {
                        _entity = "نطاق جغرافي معرف";
                        _data = string.Format("م: {0} - إسم: {1}", ((GeoArea)entity).ID, ((GeoArea)entity).Name);
                    }
                    else
                    {
                        _entity = "نطاق جغرافي";
                        _data = string.Format("م: {0} - إسم: {1}", ((GeoArea)entity).ID, ((GeoArea)entity).Name);
                    }                                        
                }
                else if (entity.GetType() == typeof(IndicatorGroup))
                {
                    _entity = "مجموعة مؤشرات";
                    _data = string.Format("م: {0} - إسم: {1}", ((IndicatorGroup)entity).ID, ((IndicatorGroup)entity).Name);
                }
                else if (entity.GetType() == typeof(Indicator))
                {
                    _entity = "مؤشر";
                    _data = string.Format("م: {0} - إسم: {1}", ((Indicator)entity).ID, ((Indicator)entity).Name);
                }
                else if (entity.GetType() == typeof(Period))
                {
                    _entity = "فترة";
                    _data = string.Format("م: {0} - إسم: {1}", ((Period)entity).ID, ((Period)entity).Name);
                }
                else if (entity.GetType() == typeof(ApplicationUser))
                {
                    _entity = "مستخدم";
                    _data = string.Format("إسم المستخدم: {0} - إسم: {1}", ((ApplicationUser)entity).UserName, ((ApplicationUser)entity).Name);
                }

                string log = string.Format("تم {0} {1}", _action, _entity);
                if (!string.IsNullOrWhiteSpace(notes))
                    log = log + " - " + notes;
                var userId = User.Identity.GetUserId();
                db.SystemLogs.Add(new SystemLog()
                {
                    ActionDate = DateTime.Now,
                    Details = _data,
                    Log = log,
                    UserName = User.Identity.GetUserName() + " - " + User.GetName()
                });
                db.SaveChanges();
            }
            catch
            {                
            }
            
        }

        public enum LogAction
        {
            Create,
            Update,
            Delete
        }
    }
}