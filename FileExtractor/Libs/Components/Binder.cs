using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Components
{
    /// <summary>
    /// Mvvm Bind Model
    /// Created by LHD on 2022-6-1 14:47:35
    /// Modify by LHD on 2022-6-16 19:20:37  增加对私有属性和公有私有字段的绑定支持
    /// </summary>
    public class BindModel
    {
        public BindModel(object bindObj, string bindMember)
        {
            BindObj = bindObj ?? throw new ArgumentNullException(nameof(bindObj));
            BindMember = bindMember;
        }

        public BindModel() { }

        public Type GetBindType()
        {
            return BindObj.GetType();
        }

        public object BindObj { get; set; }
        /// <summary>
        /// 绑定对象中的成员
        /// </summary>
        public string BindMember { get; set; }
    }
    /// <summary>
    /// Mvvm Binder
    /// Created by LHD on 2022-6-1 14:47:35
    /// </summary>
    public class Binder
    {
        public List<BindModel> BindModelList { get; set; } = new List<BindModel>();


        private static BindingFlags _memberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// 绑定对象成员
        /// </summary>
        /// <param name="bindObj"></param>
        /// <param name="bindMember"></param>
        /// <returns></returns>
        public Binder BindBase(object bindObj, string bindMember)
        {
            if (BindModelList.Count(x => x.BindObj == bindObj && x.BindMember == bindMember) > 0)
                throw new Exception($"不能重复绑定 => {bindObj} , {bindMember}");
            BindModelList.Add(new BindModel(bindObj, bindMember));
            return this;
        }

        /// <summary>
        /// 绑定对象成员
        /// </summary>
        /// <typeparam name="TBindObj"></typeparam>
        /// <typeparam name="TBindMember"></typeparam>
        /// <param name="bindObj"></param>
        /// <param name="bindMemberSelector"></param>
        /// <returns></returns>
        public Binder Bind<TBindObj, TBindMember>(TBindObj bindObj, Expression<Func<TBindObj, TBindMember>> bindMemberSelector) => BindBase(bindObj, GetSelectMemberName(bindMemberSelector));

        /// <summary>
        /// 同步绑定的值，以第一个绑定对象值为范本
        /// </summary>
        /// <returns></returns>
        public Binder Sync()
        {
            var updateBind = BindModelList.FirstOrDefault();
            if (updateBind is null) return this;
            return SyncBase(updateBind.BindObj, updateBind.BindMember);
        }

        /// <summary>
        /// 按照范本同步绑定的值
        /// </summary>
        /// <returns></returns>
        public Binder Sync<TBindObj, TBindMember>(TBindObj bindObj, Expression<Func<TBindObj, TBindMember>> bindMemberSelector) => SyncBase(bindObj, GetSelectMemberName(bindMemberSelector));

        /// <summary>
        /// 按照范本同步绑定的值
        /// </summary>
        /// <returns></returns>
        public Binder SyncBase(object bindObj, string bindMember)
        {
            var updateBind = GetBindModel(bindObj, bindMember);
            if (updateBind is null) return this;
            BindModelList = BindModelList.Where(x => x.BindObj != null).ToList();
            NotifyChange(updateBind);
            return this;
        }

        /// <summary>
        /// 改变对象的成员值，触发所有绑定对象的变更
        /// </summary>
        /// <param name="bindObj"></param>
        /// <param name="bindMember"></param>
        /// <param name="value"></param>
        public Binder ChangeBase(object bindObj, string bindMember, object value)
        {
            var updateBind = GetBindModel(bindObj, bindMember);
            ChangeOne(updateBind, value);
            BindModelList.Where(x => x != null && x != updateBind).ToList().ForEach(x => ChangeOne(x, value));
            return this;
        }

        /// <summary>
        /// 改变对象的成员值，触发所有绑定对象的变更
        /// </summary>
        /// <typeparam name="TBindObj"></typeparam>
        /// <typeparam name="TBindMember"></typeparam>
        /// <param name="bindObj"></param>
        /// <param name="bindMemberSelector"></param>
        /// <param name="value"></param>
        public Binder Change<TBindObj, TBindMember>(TBindObj bindObj, Expression<Func<TBindObj, TBindMember>> bindMemberSelector, TBindMember value) => ChangeBase(bindObj, GetSelectMemberName(bindMemberSelector), value);


        /// <summary>
        /// 获取成员选择表达式中的成员名
        /// </summary>
        /// <typeparam name="TBindObj"></typeparam>
        /// <typeparam name="TBindMember"></typeparam>
        /// <param name="bindMemberSelector"></param>
        /// <returns></returns>
        private string GetSelectMemberName<TBindObj, TBindMember>(Expression<Func<TBindObj, TBindMember>> bindMemberSelector)
        {
            var memberName = ((MemberExpression)bindMemberSelector.Body).Member.Name;
            if (memberName == null || memberName.Length < 1) throw new Exception("找不到成员信息");
            return memberName;
        }

        /// <summary>
        /// 通知值发生变化，以具体绑定的对象和成员值为范本，同步其他所有绑定项的值
        /// </summary>
        /// <param name="bindObj"></param>
        /// <param name="bindMember"></param>
        public void NotifyChange(object bindObj, string bindMember)
        {
            var updateBind = GetBindModel(bindObj, bindMember);
            NotifyChange(updateBind);
        }
        /// <summary>
        /// 通知值发生变化，以具体一个绑定模型为范本，同步其他所有绑定项的值
        /// </summary>
        /// <param name="updateBind"></param>
        private void NotifyChange(BindModel updateBind)
        {
            var value = GetBindValue(updateBind);
            BindModelList.Where(x => x != null && x != updateBind).ToList().ForEach(x => ChangeOne(x, value));
        }

        /// <summary>
        /// 通过绑定对象和绑定的成员，从当前绑定器中检索并获取其绑定模型
        /// </summary>
        /// <param name="bindObj"></param>
        /// <param name="bindMember"></param>
        /// <returns></returns>
        private BindModel GetBindModel(object bindObj, string bindMember)
        {
            var updateBind = BindModelList.FirstOrDefault(x => x.BindObj == bindObj && x.BindMember == bindMember);
            if (updateBind == null) throw new Exception($"没有该绑定项 => {bindObj} , {bindMember}");
            return updateBind;
        }

        /// <summary>
        /// 通过绑定模型获取绑定的值
        /// </summary>
        /// <param name="bind"></param>
        /// <returns></returns>
        private static object GetBindValue(BindModel bind)
        {
            var type = bind.GetBindType();
            var propInfo = type.GetProperty(bind.BindMember, _memberBindingFlags);
            if (propInfo == null)
            {
                var fieldInfo = type.GetField(bind.BindMember, _memberBindingFlags);
                if (fieldInfo == null)
                    throw new Exception($"绑定的类型 {type} 没有 {bind.BindMember} 成员");
                return fieldInfo.GetValue(bind.BindObj);
            }
            return propInfo.GetValue(bind.BindObj);
        }

        /// <summary>
        /// 改变绑定对象中的一个项，不会触发其他绑定对象的同步
        /// </summary>
        /// <param name="bind"></param>
        /// <param name="value"></param>
        private static void ChangeOne(BindModel bind, object value)
        {
            var type = bind.GetBindType();
            var propInfo = type.GetProperty(bind.BindMember, _memberBindingFlags);
            if (propInfo == null)
            {
                var fieldInfo = type.GetField(bind.BindMember, _memberBindingFlags);
                if (fieldInfo == null)
                    throw new Exception($"绑定的类型 {type} 没有 {bind.BindMember} 成员");
                fieldInfo.SetValue(bind.BindObj, value);
                return;
            }
            propInfo.SetValue(bind.BindObj, value, null);
        }
    }
}
