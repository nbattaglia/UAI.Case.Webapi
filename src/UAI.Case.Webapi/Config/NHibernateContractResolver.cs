﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace UAI.Case.Webapi.Config
{
    public class NHibernateContractResolver : DefaultContractResolver
    {
        //    private static readonly MemberInfo[] NHibernateProxyInterfaceMembers = typeof(INHibernateProxy).GetMembers();

        //    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        //    {
        //        var members = base.GetSerializableMembers(objectType);

        //        members.RemoveAll(memberInfo =>
        //                          (IsMemberPartOfNHibernateProxyInterface(memberInfo)) ||
        //                          (IsMemberDynamicProxyMixin(memberInfo)) ||
        //                          (IsMemberMarkedWithIgnoreAttribute(memberInfo, objectType)) ||
        //                          (IsMemberInheritedFromProxySuperclass(memberInfo, objectType)));

        //        var actualMemberInfos = new List<MemberInfo>();

        //        foreach (var memberInfo in members)
        //        {
        //            var infos = memberInfo.DeclaringType.BaseType.GetMember(memberInfo.Name);
        //            actualMemberInfos.Add(infos.Length == 0 ? memberInfo : infos[0]);
        //        }

        //        return actualMemberInfos;
        //    }

        //    private static bool IsMemberDynamicProxyMixin(MemberInfo memberInfo)
        //    {
        //        return memberInfo.Name == "__interceptors";
        //    }

        //    private static bool IsMemberInheritedFromProxySuperclass(MemberInfo memberInfo, Type objectType)
        //    {
        //        return memberInfo.DeclaringType.Assembly == typeof(INHibernateProxy).Assembly;
        //    }

        //    private static bool IsMemberMarkedWithIgnoreAttribute(MemberInfo memberInfo, Type objectType)
        //    {
        //        var infos = typeof(INHibernateProxy).IsAssignableFrom(objectType)
        //                      ? objectType.BaseType.GetMember(memberInfo.Name)
        //                      : objectType.GetMember(memberInfo.Name);

        //        return infos[0].GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0;
        //    }

        //    private static bool IsMemberPartOfNHibernateProxyInterface(MemberInfo memberInfo)
        //    {
        //        return Array.Exists(NHibernateProxyInterfaceMembers, mi => memberInfo.Name == mi.Name);
        //    }
        //}
    }
}
