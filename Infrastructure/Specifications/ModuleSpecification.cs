using System;
using System.Linq.Expressions;
using FourTwenty.Core.Data.Specifications;
using FourTwenty.IoT.Connect.Entities;

namespace Infrastructure.Specifications
{
    public abstract class ModuleSpecification : BaseSpecification<GrowBoxModule>
    {
    }

    public class ModuleByIdSpecification : ModuleSpecification
    {
        private readonly Guid _id;

        public ModuleByIdSpecification(Guid id)
        {
            _id = id;
        }
        public override Expression<Func<GrowBoxModule, bool>> ToExpression()
        {
            return (d) => d.Id == _id;
        }
    }

    public class ModuleWithRulesSpecification : ModuleSpecification
    {

        public ModuleWithRulesSpecification()
        {
            AddInclude(d => d.Rules);
        }
        public override Expression<Func<GrowBoxModule, bool>> ToExpression()
        {
            return All.ToExpression();
        }
    }
}
