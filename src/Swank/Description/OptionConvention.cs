using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Extensions.Compatibility;
using FieldInfo = System.Reflection.FieldInfo;

namespace FubuMVC.Swank.Description
{
    public class OptionConvention : IDescriptionConvention<FieldInfo, OptionDescription>
    {
        public virtual OptionDescription GetDescription(FieldInfo field)
        {
            var description = field.GetCustomAttribute<DescriptionAttribute>();
            return new OptionDescription {
                    Name = description.WhenNotNull(x => x.Name).Otherwise(field.Name),
                    Comments = description.WhenNotNull(x => x.Comments)
                        .Otherwise(field.GetCustomAttribute<CommentsAttribute>()
                                        .WhenNotNull(x => x.Comments).OtherwiseDefault())
                };
        }
    }
}