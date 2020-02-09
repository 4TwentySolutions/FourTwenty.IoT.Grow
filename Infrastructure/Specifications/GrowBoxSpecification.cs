using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FourTwenty.Core.Data.Specifications;
using FourTwenty.IoT.Connect.Entities;

namespace Infrastructure.Specifications
{
    public abstract class GrowBoxSpecification : BaseSpecification<GrowBox>
    {
    }

    public class GrowBoxWithModulesSpecification : GrowBoxSpecification
    {

        public GrowBoxWithModulesSpecification()
        {
            AddInclude(d => d.Modules);
        }
        public override Expression<Func<GrowBox, bool>> ToExpression()
        {
            return All.ToExpression();
        }
    }
}
