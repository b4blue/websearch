using Common.Models;
using Omu.ValueInjecter;

namespace Common.Mapping
{
    public static class Mapper
    {
        public static TOutput MapFrom<TInput, TOutput>(TInput input, TOutput output)
        {
            return (TOutput)output.InjectFrom(input);
        }

        public static TOutput MapFrom<TInput, TOutput>(TInput input) where TOutput : new()
        {
            return MapFrom(input, new TOutput());
        }
        
        public static PropertyMnmgt MapFrom(PropertyInput input)
        {   var output = MapFrom<PropertyInput, PropertyMnmgt>(input);
            output.TypeId = input.PropertyId.ToString();
            return output;
        }

        public static PropertyMnmgt MapFrom(ManagementInput input)
        {
            var output = MapFrom<ManagementInput, PropertyMnmgt>(input);
            output.TypeId = input.MgmtId.ToString();
            return output;
        }
        
        public static SearchResult MapFrom(PropertyMnmgt input)
        {
            var output = MapFrom<PropertyMnmgt, SearchResult>(input);
            return output;
        }
    }
}