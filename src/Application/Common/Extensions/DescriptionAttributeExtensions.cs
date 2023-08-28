namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class DescriptionAttributeExtensions
{
    public static string GetDescription(this Enum e)
    {
        string name = e.ToString();
        MemberInfo memberInfo = e.GetType()
                                 .GetMember(name)[0];
        object[] descriptionAttributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descriptionAttributes.Any())
        {
            return ((DescriptionAttribute)descriptionAttributes.First()).Description;
        }

        return name;
    }

    public static string GetMemberDescription<T, TProperty>(this T t, Expression<Func<T, TProperty>> property) where T : class
    {
        if (t is null)
        {
            t = Activator.CreateInstance<T>();
        }

        string memberName = ((MemberExpression)property.Body).Member.Name;
        MemberInfo? memberInfo = typeof(T).GetMember(memberName)
                                          .FirstOrDefault();
        if (memberInfo != null)
        {
            object[] descriptionAttributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttributes.Any())
            {
                return ((DescriptionAttribute)descriptionAttributes.First()).Description;
            }
        }

        return memberName;
    }

    public static string GetClassDescription<T>(this T t) where T : class
    {
        if (t is null)
        {
            t = (T)Activator.CreateInstance(typeof(T))!;
        }

        object[] descriptionAttributes = t.GetType()
                                          .GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descriptionAttributes.Any())
        {
            return (descriptionAttributes.First() as DescriptionAttribute)!.Description;
        }

        return nameof(t);
    }
}